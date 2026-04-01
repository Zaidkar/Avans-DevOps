using Avans_DevOps.AvansDevOps.Domain.States.BacklogItemStates;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Avans_DevOps.AvansDevOps.Domain.Entities
{
    public class BacklogItem
    {
        private readonly List<Activity> _activities = new();
        private BacklogItemState _state;

        public Guid Id { get; }
        public string Title { get; private set; }
        public string Description { get; private set; }
        public int StoryPoints { get; private set; }
        public User? AssignedDeveloper { get; private set; }

        public IReadOnlyCollection<Activity> Activities => _activities.AsReadOnly();
        public string CurrentState => _state.Name;

        public BacklogItem(Guid id, string title, string description, int storyPoints)
        {
            if (id == Guid.Empty)
                throw new ArgumentException("Backlog item id cannot be empty.", nameof(id));

            if (string.IsNullOrWhiteSpace(title))
                throw new ArgumentException("Title cannot be empty.", nameof(title));

            if (storyPoints < 0)
                throw new ArgumentException("Story points cannot be negative.", nameof(storyPoints));

            Id = id;
            Title = title;
            Description = description ?? string.Empty;
            StoryPoints = storyPoints;

            _state = new TodoBacklogItemState();
        }

        public void ChangeTitle(string title) => _state.ChangeTitle(this, title);
        public void ChangeDescription(string description) => _state.ChangeDescription(this, description);
        public void ChangeStoryPoints(int storyPoints) => _state.ChangeStoryPoints(this, storyPoints);

        public void AssignDeveloper(User developer) => _state.AssignDeveloper(this, developer);
        public void UnassignDeveloper() => _state.UnassignDeveloper(this);

        public void AddActivity(Activity activity) => _state.AddActivity(this, activity);
        public void RemoveActivity(Guid activityId) => _state.RemoveActivity(this, activityId);

        public void StartWork() => _state.StartWork(this);
        public void MarkReadyForTesting() => _state.MarkReadyForTesting(this);
        public void StartTesting() => _state.StartTesting(this);
        public void MarkTested() => _state.MarkTested(this);
        public void ApproveDone() => _state.ApproveDone(this);
        public void ReturnToTodo() => _state.ReturnToTodo(this);
        public void ReturnToReadyForTesting() => _state.ReturnToReadyForTesting(this);

        internal void ChangeTitleInternal(string title)
        {
            if (string.IsNullOrWhiteSpace(title))
                throw new ArgumentException("Title cannot be empty.", nameof(title));

            Title = title;
        }

        internal void ChangeDescriptionInternal(string description)
        {
            Description = description ?? string.Empty;
        }

        internal void ChangeStoryPointsInternal(int storyPoints)
        {
            if (storyPoints < 0)
                throw new ArgumentException("Story points cannot be negative.", nameof(storyPoints));

            StoryPoints = storyPoints;
        }

        internal void AssignDeveloperInternal(User developer)
        {
            AssignedDeveloper = developer ?? throw new ArgumentNullException(nameof(developer));
        }

        internal void UnassignDeveloperInternal()
        {
            AssignedDeveloper = null;
        }

        internal void AddActivityInternal(Activity activity)
        {
            if (activity is null)
                throw new ArgumentNullException(nameof(activity));

            if (_activities.Any(x => x.Id == activity.Id))
                throw new InvalidOperationException("This activity is already linked to the backlog item.");

            _activities.Add(activity);
        }

        internal void RemoveActivityInternal(Guid activityId)
        {
            var activity = _activities.SingleOrDefault(x => x.Id == activityId) ?? throw new InvalidOperationException("Activity not found on this backlog item.");
            _activities.Remove(activity);
        }

        internal bool HasAssignedDeveloper() => AssignedDeveloper is not null;

        internal bool AllActivitiesDone()
        {
            return _activities.All(activity => activity.IsDone());
        }

        internal void SetTodoState()
        {
            _state = new TodoBacklogItemState();
        }

        internal void SetDoingState()
        {
            _state = new DoingBacklogItemState();
        }

        internal void SetReadyForTestingState()
        {
            _state = new ReadyForTestingBacklogItemState();
        }

        internal void SetTestingState()
        {
            _state = new TestingBacklogItemState();
        }

        internal void SetTestedState()
        {
            _state = new TestedBacklogItemState();
        }

        internal void SetDoneState()
        {
            _state = new DoneBacklogItemState();
        }
    }
}
