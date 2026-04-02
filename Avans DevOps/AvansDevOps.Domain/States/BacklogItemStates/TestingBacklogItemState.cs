using Avans_DevOps.AvansDevOps.Domain.Entities;

namespace Avans_DevOps.AvansDevOps.Domain.States.BacklogItemStates
{
    public class TestingBacklogItemState : BacklogItemState
    {
        public override string Name => "Testing";

        public override void MarkTested(BacklogItem backlogItem)
        {
            backlogItem.SetTestedState();
        }

        public override void ReturnToTodo(BacklogItem backlogItem)
        {
            backlogItem.SetTodoState();
        }

    }
}