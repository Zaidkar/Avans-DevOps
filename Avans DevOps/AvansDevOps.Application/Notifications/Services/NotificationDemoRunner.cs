using Avans_DevOps.AvansDevOps.Application.Notifications.Contracts;
using Avans_DevOps.AvansDevOps.Application.Notifications.Handlers;
using Avans_DevOps.AvansDevOps.Application.Notifications.Observers;
using Avans_DevOps.AvansDevOps.Application.Notifications.Publishers;
using Avans_DevOps.AvansDevOps.Application.Notifications.Strategies;
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

            var recipients = new List<Avans_DevOps.AvansDevOps.Domain.Entities.User>
            {
                new Avans_DevOps.AvansDevOps.Domain.Entities.User { Name = "Alice", Email = "alice@example.com" },
                new Avans_DevOps.AvansDevOps.Domain.Entities.User { Name = "Bob", Email = "bob@example.com" },
                new Avans_DevOps.AvansDevOps.Domain.Entities.User { Name = "Charlie", Email = "charlie@example.com" }
            };

            backlogHandler.NotifyReadyForTesting("1", recipients);
            // backlogHandler.NotifyBackToTodo("1", recipients);
            // sprintHandler.NotifySprintFinished("1", recipients);
            // sprintHandler.NotifyReleaseSuccess("1", recipients);
            // discussionHandler.NotifyDiscussionReply("Backlogitem 1", recipients);
        }
    }
}