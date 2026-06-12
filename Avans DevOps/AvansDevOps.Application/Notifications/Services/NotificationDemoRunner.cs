using Avans_DevOps.AvansDevOps.Application.Pipeline;
using Avans_DevOps.AvansDevOps.Application.Notifications.Simple;
using Avans_DevOps.AvansDevOps.Application.Notifications.Simple.Strategies;
using Avans_DevOps.AvansDevOps.Domain.Entities;
using Avans_DevOps.AvansDevOps.Domain.Enum;
using Avans_DevOps.AvansDevOps.Infrastructure.Notifications.Clients;

namespace Avans_DevOps.AvansDevOps.Application.Notifications.Services
{
    public class NotificationDemoRunner
    {
        private List<User> users =
        [
            new User { Id = Guid.NewGuid(), Name = "Alice",  Email = "alice@example.com", SlackChannel = "#dev", PhoneNumber = "+31610000001" },
            new User { Id = Guid.NewGuid(), Name = "Bob", Email = "bob@example.com", SlackChannel = "#qa", PhoneNumber = "+31610000002" },
            new User { Id = Guid.NewGuid(), Name = "Charlie", Email = "charlie@example.com", SlackChannel = "#scrum", PhoneNumber = "+31610000003" }
        ];
        public void Run()
        {
            Console.WriteLine($"[{DateTime.Now:HH:mm:ss.fff}] [NotificationDemoRunner] Run");

            var strategyFactory = new NotificationStrategyFactory(
                new ExternalMailClient(),
                new SlackSdk(),
                new SmsSdk());
            var eventManager = new EventManager();
            var pipelineFactory = new PipelineFactory();

            var productOwner = users[0];
            var tester = users[1];
            var scrumMaster = users[2];
            
            
            var sprint = new Sprint(
                Guid.NewGuid(),
                "Backlog Notification Sprint",
                DateOnly.FromDateTime(DateTime.UtcNow),
                DateOnly.FromDateTime(DateTime.UtcNow.AddDays(14)),
                SprintGoalType.Release,
                eventManager);

            sprint.AddMember(productOwner, SprintRole.ProductOwner);
            sprint.AddMember(tester, SprintRole.Tester);
            sprint.AddMember(scrumMaster, SprintRole.ScrumMaster);
            var allChannels = new[] { ChannelType.Email, ChannelType.Slack, ChannelType.Sms };
            var emailChannel = new [] { ChannelType.Email };
            eventManager.Subscribe(NotificationEventNames.ReadyForTesting, new BacklogItemListener(
                sprint,
                strategyFactory,
                [SprintRole.Tester],
                emailChannel));
            eventManager.Subscribe(NotificationEventNames.TestFailure, new BacklogItemListener(
                sprint,
                strategyFactory,
                [SprintRole.ScrumMaster],
                emailChannel));
            eventManager.Subscribe(NotificationEventNames.ReleaseSuccess, new SprintNotificationListener(
                sprint,
                strategyFactory,
                emailChannel,
                [SprintRole.ScrumMaster, SprintRole.ProductOwner]));
            eventManager.Subscribe(NotificationEventNames.ReleaseFailure, new SprintNotificationListener(
                sprint,
                strategyFactory,
                emailChannel,
                [SprintRole.ScrumMaster]));
            eventManager.Subscribe(NotificationEventNames.ReleaseCancelled, new SprintNotificationListener(
                sprint,
                strategyFactory,
                emailChannel,
                [SprintRole.ScrumMaster, SprintRole.ProductOwner]));
            eventManager.Subscribe(NotificationEventNames.SprintFinished, new SprintNotificationListener(
                sprint,
                strategyFactory,
                emailChannel,
                [SprintRole.Developer, SprintRole.Tester, SprintRole.ScrumMaster, SprintRole.ProductOwner]));
            eventManager.Subscribe(NotificationEventNames.DiscussionCreated, new DiscussionNotificationListener(
                sprint,
                strategyFactory,
                emailChannel));
            eventManager.Subscribe(NotificationEventNames.DiscussionReply, new DiscussionNotificationListener(
                sprint,
                strategyFactory,
                emailChannel));

            Console.WriteLine("[Demo] Backlog notifications");
           
            var backlogItem = new BacklogItem(Guid.NewGuid(), "BacklogItem 1", "Demo item", 3, eventManager);
            backlogItem.AssignToSprint(sprint.Id);
            backlogItem.AssignDeveloper(productOwner);
            backlogItem.MarkReadyForTesting();
            backlogItem.StartTesting();
            backlogItem.MarkTested();
            backlogItem.ReturnToReadyForTesting();
            backlogItem.StartTesting();
            backlogItem.ReturnToTodo();
    
            
            sprint.Start();
            
            // sprintService.Start(backlogSprintId); // Start the sprint to trigger notifications for the discussion
            Console.WriteLine("[Demo] Discussion notifications");
            var discussion = new DiscussionThread(Guid.NewGuid(), backlogItem.Id, "Sprint retrospective", eventManager);
            var post = new DiscussionPost(Guid.NewGuid(), scrumMaster, "Precies ja", DateTime.UtcNow);
            discussion.AddPost(post);
            // discussionService.Reply(discussionId, new DiscussionPost(Guid.NewGuid(), scrumMaster, "Mee eens", DateTime.UtcNow));
            //
            // Console.WriteLine("[Demo] Sprint finished notification");
            // sprint.Finish();
            
            Console.WriteLine("[Demo] Pipeline success notification");
            
            var pipeline = pipelineFactory.CreateDeploymentPipeline("Sprint deployment pipeline");
       
            sprint.AssignPipeline(pipeline);
            
            sprint.Finish();
            sprint.BeginRelease();
            
            
            // sprint.ExecuteReleasePipeline();
            //
            Console.WriteLine("[Demo] Pipeline failure notification");
            sprint.ReleaseFailed();
            sprint.RetryRelease();
            sprint.ReleaseFailed();
            sprint.CancelRelease();
          

            Console.WriteLine("[Demo] Notification demo completed");
        }
    }
}