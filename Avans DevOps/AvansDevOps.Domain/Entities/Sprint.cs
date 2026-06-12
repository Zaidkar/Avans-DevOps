using Avans_DevOps.AvansDevOps.Domain.Entities.Pipeline;
using Avans_DevOps.AvansDevOps.Domain.Enum;
using Avans_DevOps.AvansDevOps.Domain.States.SprintStates;
using Avans_DevOps.AvansDevOps.Application.Notifications.Simple;

namespace Avans_DevOps.AvansDevOps.Domain.Entities
{
    public class Sprint
    {
        private readonly IEventManager _eventManager;

        private readonly List<SprintMember> _members = [];
        private readonly List<BacklogItem> _backlogItems = [];
        private readonly List<ScmReference> _scmReferences = [];
        private SprintState _state;
        

        public IReadOnlyCollection<ScmReference> ScmReferences => _scmReferences.AsReadOnly();

        public Guid Id { get; }
        public string Name { get; private set; }
        public DateOnly StartDate { get; private set; }
        public DateOnly EndDate { get; private set; }
        public SprintGoalType SprintGoalType { get; }
        public PipelineDefinition? Pipeline { get; private set; }
        public string? ReviewSummaryDocumentPath { get; private set; }

        public IReadOnlyCollection<SprintMember> Members => _members.AsReadOnly();
        public IReadOnlyCollection<BacklogItem> BacklogItems => _backlogItems.AsReadOnly();
        public string CurrentState => _state.Name;

        public Sprint(
            Guid id,
            string name,
            DateOnly startDate,
            DateOnly endDate,
            SprintGoalType goalType,
            IEventManager eventManager)
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
            _eventManager = eventManager;
            _state = new SprintCreatedState();
        }

        public void Rename(string name) => _state.Rename(this, name);
        public void ChangePlanning(DateOnly startDate, DateOnly endDate) => _state.ChangePlanning(this, startDate, endDate);
        public void AddMember(User user, SprintRole sprintRole) => _state.AddMember(this, user, sprintRole);
        public void RemoveMember(Guid userId) => _state.RemoveMember(this, userId);
        public void AddBacklogItem(BacklogItem backlogItem) => _state.AddBacklogItem(this, backlogItem);
        public void RemoveBacklogItem(BacklogItem backlogItem) => _state.RemoveBacklogItem(this, backlogItem);
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

        public void AddScmReference(ScmReference scmReference)
        {
            if (scmReference is null)
                throw new ArgumentNullException(nameof(scmReference));

            if (_scmReferences.Any(x => x.Id == scmReference.Id))
                throw new InvalidOperationException("This SCM reference is already linked to the sprint.");

            _scmReferences.Add(scmReference);
        }
        internal bool SendNotification(String eventType, NotificationEventData data)
        {
            if (string.IsNullOrWhiteSpace(eventType)) throw new ArgumentException("Event Type is required.",nameof(eventType));
            _eventManager.Notify(eventType, data);
            return true;
        }

        public bool ExecuteReleasePipeline()
        {
          
            var executionResult = Pipeline?.Execute();
            // return executionResult.Succeeded;
            if (executionResult is null)
            {
                throw new InvalidOperationException("Execution result is null.");
            }
            return executionResult.Succeeded
                ? SendNotification(NotificationEventNames.ReleaseSuccess, new NotificationEventData
                {
                    SprintId = Id,
                    Subject = "Pipeline activities successful",
                    Body = $"All pipeline activities for sprint {Name} were executed successfully."
                })
                : SendNotification(NotificationEventNames.ReleaseFailure, new NotificationEventData
                {
                    SprintId = Id,
                    Subject = "Pipeline activity failed",
                    Body = $"A pipeline activity failed during the release of sprint {Name}."
                });
        }
        public void RemoveScmReference(Guid scmReferenceId)
        {
            var scmReference = _scmReferences.SingleOrDefault(x => x.Id == scmReferenceId)
                ?? throw new InvalidOperationException("SCM reference not found on this sprint.");

            _scmReferences.Remove(scmReference);
        }
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

        internal void AddMemberInternal(User user, SprintRole sprintRole)
        {
            var member = new SprintMember(user, sprintRole);
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

        internal void AddBacklogItemInternal(BacklogItem backlogItem)
        {
            
            _backlogItems.Add(backlogItem);
        }

        internal void RemoveBacklogItemInternal(BacklogItem backlogItem)
        {
            _backlogItems.Remove(backlogItem);
        }

        internal void SetReviewSummaryDocument(string documentPath)
        {
            if (string.IsNullOrWhiteSpace(documentPath))
                throw new ArgumentException("Document path cannot be empty.", nameof(documentPath));

            ReviewSummaryDocumentPath = documentPath;
        }


        internal bool IsReleaseSprint() => SprintGoalType == SprintGoalType.Release;
        internal bool IsReviewSprint() => SprintGoalType ==SprintGoalType.Review;
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
