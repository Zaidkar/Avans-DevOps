using Avans_DevOps.AvansDevOps.Domain.Entities;

namespace Avans_DevOps.AvansDevOps.Domain.States.BacklogItemStates
{
    public class ReadyForTestingBacklogItemState : BacklogItemState
    {
        public override string Name => "ReadyForTesting";


        public override void StartTesting(BacklogItem backlogItem)
        {
            backlogItem.SetTestingState();
        }
    }
}