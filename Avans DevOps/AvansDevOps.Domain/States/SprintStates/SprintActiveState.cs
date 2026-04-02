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
            sprint.AssignPipelineInternal(pipeline);
        }

        public override void UploadReviewSummary(Sprint sprint, string documentPath)
        {
            sprint.SetReviewSummaryDocument(documentPath);
        }

        public override void FinishTimeBox(Sprint sprint)
        {
            sprint.SetFinishedState();
        }
    }
}
