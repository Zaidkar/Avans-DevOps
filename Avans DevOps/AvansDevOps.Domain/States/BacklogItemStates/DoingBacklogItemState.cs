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

        public override void RemoveFromSprint(BacklogItem backlogItem)
        {
            backlogItem.RemoveFromSprintInternal();
        }

        public override void UnassignDeveloper(BacklogItem backlogItem)
        {
            backlogItem.UnassignDeveloperInternal();
        }

        public override void MarkReadyForTesting(BacklogItem backlogItem)
        {
            backlogItem.SetReadyForTestingState();
        }
    }
}