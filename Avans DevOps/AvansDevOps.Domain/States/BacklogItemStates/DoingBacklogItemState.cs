using Avans_DevOps.AvansDevOps.Domain.Entities;

namespace Avans_DevOps.AvansDevOps.Domain.States.BacklogItemStates
{
    public class DoingBacklogItemState : BacklogItemState
    {
        public override string Name => "Doing";

        public override void AssignDeveloper(BacklogItem backlogItem, User developer)
        {
            backlogItem.AssignDeveloperInternal(developer);
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