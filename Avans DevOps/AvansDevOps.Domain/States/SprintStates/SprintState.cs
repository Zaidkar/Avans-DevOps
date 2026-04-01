using Avans_DevOps.AvansDevOps.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Avans_DevOps.AvansDevOps.Domain.States.SprintStates
{
    public abstract class SprintState
        //abstracte klasse zodat niet in iedere state klasse code hoeft over of het niet mag of wel.
    {
        public abstract string Name { get; }

        protected InvalidOperationException InvalidAction(string actionName)
        {
            return new InvalidOperationException(
                $"Action '{actionName}' is not allowed in state '{Name}'.");
        }

        public virtual void Rename(Sprint sprint, string name)
        {
            throw InvalidAction(nameof(Rename));
        }

        public virtual void ChangePlanning(Sprint sprint, DateOnly startDate, DateOnly endDate)
        {
            throw InvalidAction(nameof(ChangePlanning));
        }

        public virtual void AddMember(Sprint sprint, SprintMember member)
        {
            throw InvalidAction(nameof(AddMember));
        }

        public virtual void RemoveMember(Sprint sprint, Guid userId)
        {
            throw InvalidAction(nameof(RemoveMember));
        }

        public virtual void AddBacklogItem(Sprint sprint, Guid backlogItemId)
        {
            throw InvalidAction(nameof(AddBacklogItem));
        }

        public virtual void RemoveBacklogItem(Sprint sprint, Guid backlogItemId)
        {
            throw InvalidAction(nameof(RemoveBacklogItem));
        }

        public virtual void AssignPipeline(Sprint sprint, Guid pipelineId)
        {
            throw InvalidAction(nameof(AssignPipeline));
        }

        public virtual void UploadReviewSummary(Sprint sprint, string documentPath)
        {
            throw InvalidAction(nameof(UploadReviewSummary));
        }

        public virtual void Start(Sprint sprint)
        {
            throw InvalidAction(nameof(Start));
        }

        public virtual void FinishTimeBox(Sprint sprint)
        {
            throw InvalidAction(nameof(FinishTimeBox));
        }

        public virtual void StartRelease(Sprint sprint)
        {
            throw InvalidAction(nameof(StartRelease));
        }

        public virtual void ReleaseSucceeded(Sprint sprint)
        {
            throw InvalidAction(nameof(ReleaseSucceeded));
        }

        public virtual void ReleaseFailed(Sprint sprint)
        {
            throw InvalidAction(nameof(ReleaseFailed));
        }

        public virtual void RetryRelease(Sprint sprint)
        {
            throw InvalidAction(nameof(RetryRelease));
        }

        public virtual void CancelRelease(Sprint sprint)
        {
            throw InvalidAction(nameof(CancelRelease));
        }

        public virtual void CloseReview(Sprint sprint)
        {
            throw InvalidAction(nameof(CloseReview));
        }
    }
}
