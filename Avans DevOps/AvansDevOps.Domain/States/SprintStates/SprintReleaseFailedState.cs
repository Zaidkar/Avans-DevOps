using Avans_DevOps.AvansDevOps.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Avans_DevOps.AvansDevOps.Application.Notifications.Simple;

namespace Avans_DevOps.AvansDevOps.Domain.States.SprintStates
{
    public class SprintReleaseFailedState : SprintState
    {
        public override string Name => "ReleaseFailed";

        public override void RetryRelease(Sprint sprint)
        {
            sprint.SetReleasingState();
        }

        public override void CancelRelease(Sprint sprint)
        {
            sprint.SetReleaseCancelledState();
            sprint.SendNotification(NotificationEventNames.ReleaseCancelled, new NotificationEventData 
            {
                SprintId = sprint.Id,
                Subject = "Pipeline release cancelled",
                Body = $"Release of sprint {sprint.Name} has been cancelled."
                    });
        }
    }
}
