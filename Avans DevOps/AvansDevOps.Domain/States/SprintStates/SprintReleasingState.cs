using Avans_DevOps.AvansDevOps.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Avans_DevOps.AvansDevOps.Domain.States.SprintStates
{
    public class SprintReleasingState : SprintState
    {
        public override string Name => "Releasing";

        public override void ReleaseSucceeded(Sprint sprint)
        {
            sprint.SetReleasedState();
        }

        public override void ReleaseFailed(Sprint sprint)
        {
            sprint.SetReleaseFailedState();
        }
    }
}
