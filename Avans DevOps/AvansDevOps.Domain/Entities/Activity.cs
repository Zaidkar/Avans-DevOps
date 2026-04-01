using Avans_DevOps.AvansDevOps.Domain.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Avans_DevOps.AvansDevOps.Domain.Entities
{
    public class Activity
    {
        public Guid Id { get; }
        public string Title { get; private set; }
        public string Description { get; private set; }
        public ActivityStatus Status { get; private set; }
        public User? AssignedDeveloper { get; private set; }

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
    }

}
