using Avans_DevOps.AvansDevOps.Domain.Enum;
using Avans_DevOps.AvansDevOps.Domain.Interfaces;
using System;
using System.Collections.Generic;

namespace Avans_DevOps.AvansDevOps.Domain.Entities
{
    public class Activity : IBacklogWorkItemComponent
    {
        public Guid Id { get; }
        public string Title { get; private set; }
        public string Description { get; private set; }
        public ActivityStatus Status { get; private set; }
        public User? AssignedDeveloper { get; private set; }

        public IReadOnlyCollection<IBacklogWorkItemComponent> Children => Array.Empty<IBacklogWorkItemComponent>();

        public Activity(Guid id, string title, string description)
        {
            if (id == Guid.Empty)
                throw new ArgumentException("Activity id cannot be empty.", nameof(id));

            if (string.IsNullOrWhiteSpace(title))
                throw new ArgumentException("Title cannot be empty.", nameof(title));

            Id = id;
            Title = title;
            Description = description ?? string.Empty;
            Status = ActivityStatus.ToDo;
        }

        public void ChangeTitle(string title)
        {
            if (string.IsNullOrWhiteSpace(title))
                throw new ArgumentException("Title cannot be empty.", nameof(title));

            Title = title;
        }

        public void ChangeDescription(string description)
        {
            Description = description ?? string.Empty;
        }

        public void AssignDeveloper(User developer)
        {
            AssignedDeveloper = developer ?? throw new ArgumentNullException(nameof(developer));
        }

        public void UnassignDeveloper()
        {
            AssignedDeveloper = null;
        }

        public void StartWork()
        {
            if (Status != ActivityStatus.ToDo)
                throw new InvalidOperationException("Only a todo activity can be started.");

            Status = ActivityStatus.Doing;
        }

        public void MarkDone()
        {
            if (Status != ActivityStatus.Doing)
                throw new InvalidOperationException("Only an activity in doing can be marked as done.");

            Status = ActivityStatus.Done;
        }

        public bool IsDone()
        {
            return Status == ActivityStatus.Done;
        }

        public void Accept(IBacklogWorkItemVisitor visitor)
        {
            visitor.VisitActivity(this);
        }
    }
}
