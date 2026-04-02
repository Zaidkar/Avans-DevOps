using Avans_DevOps.AvansDevOps.Domain.Entities;

namespace Avans_DevOps.AvansDevOps.Domain.States.BacklogItemStates
{
    public class DoneBacklogItemState : BacklogItemState
    {
        public override string Name => "Done";

        public override void ReturnToTodo(BacklogItem backlogItem)
        {
            backlogItem.SetTodoState();
        }
    }
}