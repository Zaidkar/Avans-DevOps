using Avans_DevOps.AvansDevOps.Application.Notifications.Contracts;
using Avans_DevOps.AvansDevOps.Application.Notifications.Handlers;
using Avans_DevOps.AvansDevOps.Application.Notifications.Observers;
using Avans_DevOps.AvansDevOps.Application.Notifications.Publishers;
using Avans_DevOps.AvansDevOps.Application.Repositories.Fakes;
using Avans_DevOps.AvansDevOps.Application.Services;
using Avans_DevOps.AvansDevOps.Application.Notifications.Strategies;
using Avans_DevOps.AvansDevOps.Domain.Entities;
using Avans_DevOps.AvansDevOps.Domain.Enum;
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
            var backlogRepository = new FakeBacklogItemRepository(sprintRepository);
            var discussionRepository = new FakeDiscussionRepository();

            var sprintService = new SprintService(sprintRepository, userRepository, sprintHandler);
            var backlogItemService = new BacklogItemService(backlogRepository, sprintRepository, backlogHandler, userRepository);
            var discussionService = new DiscussionService(discussionRepository, userRepository, discussionHandler);

           var sprint = sprintService.Create(new Sprint(Guid.NewGuid(), "Sprint 1",DateOnly.FromDateTime(DateTime.UtcNow), DateOnly.FromDateTime(DateTime.UtcNow.AddDays(14)), SprintGoalType.Review));
       
            var backlogItemId = backlogItemService.Create(new BacklogItem(Guid.NewGuid(), "BacklogItem 1", "Demo item", 3));
            sprintService.AddMemberToSprint(sprint.Id, userRepository.GetAll().First().Id, SprintRole.Developer);
            sprintService.AddMemberToSprint(sprint.Id, userRepository.GetAll().ElementAt(1).Id, SprintRole.Tester);
            sprintService.AddMemberToSprint(sprint.Id, userRepository.GetAll().ElementAt(2).Id, SprintRole.ScrumMaster);
            sprintService.AddBacklogItem(sprint.Id, backlogItemId);
            Console.WriteLine($"{string.Join(", ", sprintService.GetAll().Select(s => s.Name))}");
            backlogItemService.AssignDeveloper(backlogItemId, userRepository.GetAll().First().Id);
            
            backlogItemService.MarkReadyForTesting(backlogItemId);
            backlogItemService.StartTesting(backlogItemId);
            backlogItemService.MarkTested(backlogItemId);
            backlogItemService.ReturnToReadyForTesting(backlogItemId);
            backlogItemService.StartTesting(backlogItemId);
            backlogItemService.ReturnToTodo(backlogItemId);
            backlogItemService.AssignDeveloper(backlogItemId, userRepository.GetAll().ElementAt(0).Id);
            backlogItemService.MarkReadyForTesting(backlogItemId);
            backlogItemService.StartTesting(backlogItemId);
            backlogItemService.MarkTested(backlogItemId);
            backlogItemService.ApproveDone(backlogItemId);


            // var discussion = new DiscussionThread(Guid.NewGuid(), Guid.NewGuid(), "Sprint retrospective");
            // var discussionId = discussionService.Create(discussion);
            // var author = userRepository.GetAll().First();
            // discussionService.Reply(discussionId, new DiscussionPost(Guid.NewGuid(), author, "Mee eens", DateTime.UtcNow));
        }
    }
}