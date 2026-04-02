using Avans_DevOps.AvansDevOps.Application.Notifications.Contracts;
using Avans_DevOps.AvansDevOps.Application.Notifications.Handlers;
using Avans_DevOps.AvansDevOps.Application.Notifications.Observers;
using Avans_DevOps.AvansDevOps.Application.Notifications.Publishers;
using Avans_DevOps.AvansDevOps.Application.Pipeline;
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
            var pipelineEmailOnlyContext = new NotificationContext(EmailStrategy);
            var publisher = new NotificationPublisher();
            var backlogHandler = new BacklogItemNotificationHandler(publisher);
            var sprintHandler = new SprintNotificationHandler(publisher);
            var discussionHandler = new DiscussionNotificationHandler(publisher);
            var pipelineReleaseHandler = new PipelineReleaseNotificationHandler(publisher);

            publisher.Subscribe(new BacklogItemNotificationObserver(context));
            publisher.Subscribe(new SprintNotificationObserver(context));
            publisher.Subscribe(new DiscussionNotificationObserver(context));
            publisher.Subscribe(new PipelineReleaseNotificationObserver(pipelineEmailOnlyContext));

            var userRepository = new FakeUserRepository();
            var sprintRepository = new FakeSprintRepository();
            var backlogRepository = new FakeBacklogItemRepository(sprintRepository);
            var discussionRepository = new FakeDiscussionRepository();
            var pipelineFactory = new PipelineFactory();
            var pipelineService = new PipelineService(sprintRepository, pipelineFactory, pipelineReleaseHandler);

            var sprintService = new SprintService(sprintRepository, userRepository, sprintHandler, pipelineService);
            var backlogItemService = new BacklogItemService(backlogRepository, sprintRepository, backlogHandler, userRepository);
            var discussionService = new DiscussionService(discussionRepository, userRepository, discussionHandler);

            var users = userRepository.GetAll();
            var productOwner = users[0];
            var tester = users[1];
            var scrumMaster = users[2];

            Guid CreateReleaseSprint(string sprintName)
            {
                var sprint = sprintService.Create(new Sprint(
                    Guid.NewGuid(),
                    sprintName,
                    DateOnly.FromDateTime(DateTime.UtcNow),
                    DateOnly.FromDateTime(DateTime.UtcNow.AddDays(14)),
                    SprintGoalType.Release));

                sprintService.AddMemberToSprint(sprint.Id, productOwner.Id, SprintRole.ProductOwner);
                sprintService.AddMemberToSprint(sprint.Id, tester.Id, SprintRole.Tester);
                sprintService.AddMemberToSprint(sprint.Id, scrumMaster.Id, SprintRole.ScrumMaster);

                return sprint.Id;
            }

            void PrepareFinishedReleaseSprint(Guid sprintId, string pipelineName)
            {
                sprintService.AssignDeploymentPipeline(sprintId, pipelineName);
                sprintService.Start(sprintId);

                var sprint = sprintService.GetById(sprintId);
                if (sprint != null)
                {
                    sprint.Finish();
                    sprintRepository.Update(sprintId, sprint);
                }
            }

            Console.WriteLine("[Demo] Backlog notifications");
            var backlogSprintId = CreateReleaseSprint("Backlog Notification Sprint");
            var backlogItemId = backlogItemService.Create(new BacklogItem(Guid.NewGuid(), "BacklogItem 1", "Demo item", 3));
            sprintService.AddBacklogItem(backlogSprintId, backlogItemId);
            backlogItemService.AssignDeveloper(backlogItemId, productOwner.Id);
            backlogItemService.MarkReadyForTesting(backlogItemId);
            backlogItemService.StartTesting(backlogItemId);
            backlogItemService.MarkTested(backlogItemId);
            backlogItemService.ReturnToReadyForTesting(backlogItemId);
            backlogItemService.StartTesting(backlogItemId);
            backlogItemService.ReturnToTodo(backlogItemId);

            Console.WriteLine("[Demo] Discussion notifications");
            var discussion = new DiscussionThread(Guid.NewGuid(), backlogSprintId, "Sprint retrospective");
            var discussionId = discussionService.Create(discussion);
            discussionService.Reply(discussionId, new DiscussionPost(Guid.NewGuid(), scrumMaster, "Mee eens", DateTime.UtcNow));

            Console.WriteLine("[Demo] Sprint finished notification");
            sprintService.FinishSprint(backlogSprintId);

            Console.WriteLine("[Demo] Pipeline success notification");
            var successSprintId = CreateReleaseSprint("Pipeline Success Sprint");
            PrepareFinishedReleaseSprint(successSprintId, "Pipeline Success Flow");
            sprintService.BeginRelease(successSprintId);
            sprintService.ExecuteReleasePipeline(successSprintId);

            Console.WriteLine("[Demo] Pipeline failure notification");
            var failureSprintId = CreateReleaseSprint("Pipeline Failure Sprint");
            PrepareFinishedReleaseSprint(failureSprintId, "Pipeline Failure Flow");
            sprintService.BeginRelease(failureSprintId);
            sprintService.ReleaseFailed(failureSprintId);

            Console.WriteLine("[Demo] Pipeline cancelled notification");
            var cancelledSprintId = CreateReleaseSprint("Pipeline Cancelled Sprint");
            PrepareFinishedReleaseSprint(cancelledSprintId, "Pipeline Cancelled Flow");
            sprintService.CancelRelease(cancelledSprintId);

            Console.WriteLine("[Demo] Notification demo completed");
        }
    }
}