using Avans_DevOps.AvansDevOps.Domain.Entities;
using System;

namespace Avans_DevOps.AvansDevOps.Domain.States.BacklogItemStates
{
    public class TestedBacklogItemState : BacklogItemState
    {
        public override string Name => "Tested";

        public override void ApproveDone(BacklogItem backlogItem)
        {
            if (!backlogItem.AllActivitiesDone())
                throw new InvalidOperationException("A backlog item cannot be marked as done until all underlying activities are done.");
            backlogItem.UnassignDeveloperInternal();
            backlogItem.SetDoneState();
        }

        public override void ReturnToReadyForTesting(BacklogItem backlogItem)
        {
            backlogItem.SetReadyForTestingState();
        }

        public override void ReturnToTodo(BacklogItem backlogItem)
        {
            backlogItem.SetTodoState();
            backlogItem.UnassignDeveloperInternal();
        }
    }
}



    