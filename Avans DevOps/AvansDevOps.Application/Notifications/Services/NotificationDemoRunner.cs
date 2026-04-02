using Avans_DevOps.AvansDevOps.Application.Notifications.Contracts;
using Avans_DevOps.AvansDevOps.Application.Notifications.Handlers;
using Avans_DevOps.AvansDevOps.Application.Notifications.Observers;
using Avans_DevOps.AvansDevOps.Application.Notifications.Publishers;
using Avans_DevOps.AvansDevOps.Application.Repositories.Fakes;
using Avans_DevOps.AvansDevOps.Application.Services;
using Avans_DevOps.AvansDevOps.Application.Notifications.Strategies;
using Avans_DevOps.AvansDevOps.Domain.Entities;
using Avans_DevOps.AvansDevOps.Infrastructure.Notifications.Adapters;
using Avans_DevOps.AvansDevOps.Infrastructure.Notifications.Clients;

namespace Avans_DevOps.AvansDevOps.Application.Notifications.Services
{
    public class NotificationDemoRunner
    {
        public void Run()
        {
            Console.WriteLine($"[{DateTime.Now:HH:mm:ss.fff}] [NotificationDemoRunner] Run");

            var mailProvider = new SendGridMailAdapter(new ExternalMailClient());
            var slackProvider = new SlackApiAdapter(new SlackSdk());
            var smsProvider = new TwilioSmsAdapter(new SmsSdk());

            var EmailStrategy = new EmailNotificationStrategy(mailProvider);
            var SlackStrategy = new SlackNotificationStrategy(slackProvider);
            var SmsStrategy = new SmsNotificationStrategy(smsProvider);

            var strategies = new List<INotificationStrategy>
            {
                EmailStrategy,
                SlackStrategy,
                SmsStrategy
            };

            var context = new NotificationContext(EmailStrategy);
            var publisher = new NotificationPublisher();
            var backlogHandler = new BacklogItemNotificationHandler(publisher);
            var sprintHandler = new SprintNotificationHandler(publisher);
            var discussionHandler = new DiscussionNotificationHandler(publisher);

            publisher.Subscribe(new BacklogItemNotificationObserver(context));
            publisher.Subscribe(new SprintNotificationObserver(context));
            publisher.Subscribe(new DiscussionNotificationObserver(context));

            var userRepository = new FakeUserRepository();
            var sprintRepository = new FakeSprintRepository();
            var backlogRepository = new FakeBacklogItemRepository();
            var discussionRepository = new FakeDiscussionRepository();

            var sprintService = new SprintService(sprintRepository, userRepository, sprintHandler);
            var backlogItemService = new BacklogItemService(backlogRepository, userRepository, backlogHandler);
            var discussionService = new DiscussionService(discussionRepository, userRepository, discussionHandler);

            // var createdSprint = sprintService.Create(new Sprint
            // {
            //     Name = "Sprint 1",
            //     StartDate = DateTime.Today,
            //     EndDate = DateTime.Today.AddDays(14)
            // });
            // sprintService.FinishSprint(createdSprint.Id);
            // sprintService.NotifyReleaseResult(createdSprint.Id, releaseSucceeded: true);

            var backlogItemId = backlogItemService.Create(new BacklogItem());
            backlogItemService.MarkReadyForTesting(backlogItemId, "BacklogItem 1");

            // var discussionId = discussionService.Create(new DiscussionThread(), "Sprint retrospective");
            // discussionService.Reply(discussionId, "Sprint retrospective");
        }
    }
}