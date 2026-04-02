using Avans_DevOps.AvansDevOps.Domain.Entities.Pipeline;
using Avans_DevOps.AvansDevOps.Domain.Enum;
using Avans_DevOps.AvansDevOps.Domain.States.SprintStates;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Avans_DevOps.AvansDevOps.Domain.Entities
{
    public class Sprint
    {
        private readonly List<SprintMember> _members = [];
        private readonly List<Guid> _backlogItemIds = [];
        private SprintState _state;

        public Guid Id { get; }
        public string Name { get; private set; }
        public DateOnly StartDate { get; private set; }
        public DateOnly EndDate { get; private set; }
        public SprintGoalType SprintGoalType { get; }
        public PipelineDefinition? Pipeline { get; private set; }
        public string? ReviewSummaryDocumentPath { get; private set; }

        public IReadOnlyCollection<SprintMember> Members => _members.AsReadOnly();
        public IReadOnlyCollection<Guid> BacklogItemIds => _backlogItemIds.AsReadOnly();
        public string CurrentState => _state.Name;

        public Sprint(
            Guid id,
            string name,
            DateOnly startDate,
            DateOnly endDate,
            SprintGoalType goalType)
        {
            if (id == Guid.Empty)
                throw new ArgumentException("Sprint id cannot be empty.", nameof(id));

            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Sprint name cannot be empty.", nameof(name));

            if (endDate < startDate)
                throw new ArgumentException("End date cannot be before start date.");

            Id = id;
            Name = name;
            StartDate = startDate;
            EndDate = endDate;
            SprintGoalType = goalType;

            _state = new SprintCreatedState();
        }

        public void Rename(string name) => _state.Rename(this, name);
        public void ChangePlanning(DateOnly startDate, DateOnly endDate) => _state.ChangePlanning(this, startDate, endDate);
        public void AddMember(SprintMember member) => _state.AddMember(this, member);
        public void RemoveMember(Guid userId) => _state.RemoveMember(this, userId);
        public void AddBacklogItem(Guid backlogItemId) => _state.AddBacklogItem(this, backlogItemId);
        public void RemoveBacklogItem(Guid backlogItemId) => _state.RemoveBacklogItem(this, backlogItemId);
        public void AssignPipeline(PipelineDefinition pipeline) => _state.AssignPipeline(this, pipeline);
        public void UploadReviewSummary(string documentPath) => _state.UploadReviewSummary(this, documentPath);


        public void Start() => _state.Start(this);
        public void Finish() => _state.FinishTimeBox(this);
        public void BeginRelease() => _state.StartRelease(this);
        public void ReleaseSucceeded() => _state.ReleaseSucceeded(this);
        public void ReleaseFailed() => _state.ReleaseFailed(this);
        public void RetryRelease() => _state.RetryRelease(this);
        public void CancelRelease() => _state.CancelRelease(this);
        public void CloseReview() => _state.CloseReview(this);


        internal void SetName(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Sprint name cannot be empty.", nameof(name));

            Name = name;
        }

        internal void SetPlanning(DateOnly startDate, DateOnly endDate)
        {
            if (endDate < startDate)
                throw new ArgumentException("End date cannot be before start date.");

            StartDate = startDate;
            EndDate = endDate;
        }

        internal void AddMemberInternal(SprintMember member)
        {
            if (member is null)
                throw new ArgumentNullException(nameof(member));

            if (_members.Any(m => m.User.Id == member.User.Id))
                throw new InvalidOperationException("User is already a member of the sprint.");

            if (member.SprintRole == SprintRole.ScrumMaster &&
                _members.Any(m => m.SprintRole == SprintRole.ScrumMaster))
                throw new InvalidOperationException("A sprint can only have one scrum master.");

            _members.Add(member);
        }

        internal void RemoveMemberInternal(Guid userId)
        {
            if (userId == Guid.Empty)
                throw new ArgumentException("User id cannot be empty.", nameof(userId));

            var existing = _members.FirstOrDefault(m => m.User.Id == userId);
            if (existing is null)
                throw new InvalidOperationException("User is not a member of the sprint.");

            _members.Remove(existing);
        }

        internal void AddBacklogItemInternal(Guid backlogItemId)
        {
            if (backlogItemId == Guid.Empty)
                throw new ArgumentException("Backlog item id cannot be empty.", nameof(backlogItemId));

            if (_backlogItemIds.Contains(backlogItemId))
                throw new InvalidOperationException("Backlog item already exists in the sprint.");

            _backlogItemIds.Add(backlogItemId);
        }

        internal void RemoveBacklogItemInternal(Guid backlogItemId)
        {
            if (backlogItemId == Guid.Empty)
                throw new ArgumentException("Backlog item id cannot be empty.", nameof(backlogItemId));

            if (!_backlogItemIds.Remove(backlogItemId))
                throw new InvalidOperationException("Backlog item does not exist in the sprint.");
        }

        internal void SetReviewSummaryDocument(string documentPath)
        {
            if (string.IsNullOrWhiteSpace(documentPath))
                throw new ArgumentException("Document path cannot be empty.", nameof(documentPath));

            ReviewSummaryDocumentPath = documentPath;
        }


        internal bool IsReleaseSprint() => SprintGoalType == global::Avans_DevOps.AvansDevOps.Domain.Enum.SprintGoalType.Release;
        internal bool IsReviewSprint() => SprintGoalType == global::Avans_DevOps.AvansDevOps.Domain.Enum.SprintGoalType.Review;
        internal bool HasPipeline() => Pipeline is not null;
        internal void AssignPipelineInternal(PipelineDefinition pipeline) => Pipeline = pipeline ?? throw new ArgumentNullException(nameof(pipeline));
        internal bool HasReviewSummary() => !string.IsNullOrWhiteSpace(ReviewSummaryDocumentPath);
        internal void SetActiveState() => _state = new SprintActiveState();
        internal void SetFinishedState() => _state = new SprintFinishedState();
        internal void SetReleasingState() => _state = new SprintReleasingState();
        internal void SetReleaseFailedState() => _state = new SprintReleaseFailedState();
        internal void SetReleasedState() => _state = new SprintReleasedState();
        internal void SetReleaseCancelledState() => _state = new SprintReleaseCancelledState();
        internal void SetClosedState() => _state = new SprintClosedState();
    }
}
