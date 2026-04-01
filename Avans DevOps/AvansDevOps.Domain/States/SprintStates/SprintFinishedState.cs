using Avans_DevOps.AvansDevOps.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Avans_DevOps.AvansDevOps.Domain.States.SprintStates
{
    public class SprintFinishedState : SprintState
    {
        public override string Name => "Finished";

        public override void StartRelease(Sprint sprint)
        {
            if (!sprint.IsReleaseSprint())
                throw new InvalidOperationException("Only release sprints can start a release.");

            if (!sprint.HasPipeline())
                throw new InvalidOperationException("A release sprint must have a pipeline assigned.");

            sprint.SetReleasingState();
        }

        public override void CancelRelease(Sprint sprint)
        {
            if (!sprint.IsReleaseSprint())
                throw new InvalidOperationException("Only release sprints can cancel a release.");

            sprint.SetReleaseCancelledState();
        }

        public override void CloseReview(Sprint sprint)
        {
            if (!sprint.IsReviewSprint())
                throw new InvalidOperationException("Only review sprints can be closed through a review.");

            if (!sprint.HasReviewSummary())
                throw new InvalidOperationException("A review summary document must be uploaded first.");

            sprint.SetClosedState();
        }
    }
}
