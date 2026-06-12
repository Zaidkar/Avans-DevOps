using Avans_DevOps.AvansDevOps.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Avans_DevOps.AvansDevOps.Application.Notifications.Simple;

namespace Avans_DevOps.AvansDevOps.Domain.States.SprintStates
{
    public class SprintReleasingState : SprintState
    {
        public override string Name => "Releasing";

        public override void ReleaseSucceeded(Sprint sprint)
        {
            sprint.SetReleasedState();
            sprint.SendNotification(NotificationEventNames.ReleaseSuccess, new NotificationEventData{
                SprintId = sprint.Id,
                Subject = "Pipeline activities successful",
                Body = $"All pipeline activities for sprint {sprint.Name} were executed successfully."
            });
        }

        public override void ReleaseFailed(Sprint sprint)
        {
            sprint.SetReleaseFailedState();
            sprint.SendNotification(NotificationEventNames.ReleaseFailure, new NotificationEventData
            {
                    SprintId = sprint.Id,
                    Subject = "Pipeline activity failed",
                    Body = $"A pipeline activity failed during the release of sprint {sprint.Name}."
            });
        }
    }
}
