using Avans_DevOps.AvansDevOps.Domain.Entities;
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

        public override void AssignPipeline(Sprint sprint, Guid pipelineId)
        {
            sprint.SetPipeline(pipelineId);
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
