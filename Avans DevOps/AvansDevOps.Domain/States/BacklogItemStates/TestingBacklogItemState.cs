using Avans_DevOps.AvansDevOps.Application.Notifications.Simple;
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
            var developer = backlogItem.AssignedDeveloper ?? throw new InvalidOperationException("No assigned developer");
            backlogItem.SetTodoState();
            backlogItem.SendNotification(NotificationEventNames.TestFailure, new NotificationEventData
            {
                Subject = "Backlog item rejected after testing",
                Body = $"Backlog item {backlogItem.Title} developed door {developer.Name} is teruggezet naar todo"
            });
            backlogItem.UnassignDeveloperInternal();
        }
    }
}