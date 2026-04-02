using Avans_DevOps.AvansDevOps.Domain.Entities;
using Avans_DevOps.AvansDevOps.Domain.Entities.Pipeline;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Avans_DevOps.AvansDevOps.Domain.States.SprintStates
{
    public class SprintActiveState : SprintState
    {
        public override string Name => "Active";

        public override void AssignPipeline(Sprint sprint, PipelineDefinition pipeline)
        {
            if (!sprint.IsReleaseSprint())
                throw new InvalidOperationException("Only release sprints can have a pipeline assigned.");

            sprint.AssignPipelineInternal(pipeline);
        }

        public override void UploadReviewSummary(Sprint sprint, string documentPath)
        {
            if (!sprint.IsReviewSprint())
                throw new InvalidOperationException("Only review sprints can upload a review summary.");

            sprint.SetReviewSummaryDocument(documentPath);
        }

        public override void FinishTimeBox(Sprint sprint)
        {
            sprint.SetFinishedState();
        }
    }
}
