using Avans_DevOps.AvansDevOps.Domain.Entities;
using System;
using Avans_DevOps.AvansDevOps.Application.Notifications.Simple;

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
            backlogItem.SendNotification(NotificationEventNames.ReadyForTesting, new NotificationEventData
            {
                Subject = "Backlog item ready for testing",
                Body = $"Backlog item {backlogItem.Title} is ready for testing"
            });
        }

        public override void ReturnToTodo(BacklogItem backlogItem)
        {
            backlogItem.SetTodoState();
            backlogItem.UnassignDeveloperInternal();
        }
    }
}