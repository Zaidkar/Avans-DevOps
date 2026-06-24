using Avans_DevOps.AvansDevOps.Application.Notifications.Simple;
using Avans_DevOps.AvansDevOps.Domain.Entities;

namespace Avans_DevOps.AvansDevOps.Domain.States.BacklogItemStates
{
    public class DoingBacklogItemState : BacklogItemState
    {
        public override string Name => "Doing";

      
        public override void AddActivity(BacklogItem backlogItem, Activity activity)
        {
            backlogItem.AddActivityInternal(activity);
        }

        public override void RemoveActivity(BacklogItem backlogItem, Guid activityId)
        {
            backlogItem.RemoveActivityInternal(activityId);
        }

        public override void AssignDeveloper(BacklogItem backlogItem, User developer)
        {
            backlogItem.AssignDeveloperInternal(developer);
        }

        public override void UnassignDeveloper(BacklogItem backlogItem)
        {
            backlogItem.UnassignDeveloperInternal();
            backlogItem.SetTodoState();
        }

        public override void MarkReadyForTesting(BacklogItem backlogItem)
        {
            backlogItem.SetReadyForTestingState();
            backlogItem.SendNotification(NotificationEventNames.ReadyForTesting, new NotificationEventData
            {
                // SprintId = backlogItem.SprintId ?? throw new InvalidOperationException("Backlog item must be in a sprint"),
                Subject = "Backlog item ready for testing",
                Body = $"Backlog item {backlogItem.Title} is ready for testing"
            });
            
        }
    }
}