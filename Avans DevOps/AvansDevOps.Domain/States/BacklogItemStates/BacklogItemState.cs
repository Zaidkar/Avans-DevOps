using Avans_DevOps.AvansDevOps.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Avans_DevOps.AvansDevOps.Domain.States.BacklogItemStates
{
    public abstract class BacklogItemState
    //abstracte klasse zodat niet in iedere state klasse code hoeft over of het niet mag of wel.
    {
        public abstract string Name { get; }

        protected InvalidOperationException InvalidAction(string actionName)
        {
            return new InvalidOperationException(
                $"Action '{actionName}' is not allowed in state '{Name}'.");
        }

        public virtual void ChangeTitle(BacklogItem backlogItem, string title)
        {
            throw InvalidAction(nameof(ChangeTitle));
        }

        public virtual void ChangeDescription(BacklogItem backlogItem, string description)
        {
            throw InvalidAction(nameof(ChangeDescription));
        }

        public virtual void ChangeStoryPoints(BacklogItem backlogItem, int storyPoints)
        {
            throw InvalidAction(nameof(ChangeStoryPoints));
        }

        public virtual void AssignDeveloper(BacklogItem backlogItem, User developer)
        {
            throw InvalidAction(nameof(AssignDeveloper));
        }

        public virtual void UnassignDeveloper(BacklogItem backlogItem)
        {
            throw InvalidAction(nameof(UnassignDeveloper));
        }

        public virtual void AddActivity(BacklogItem backlogItem, Activity activity)
        {
            throw InvalidAction(nameof(AddActivity));
        }

        public virtual void RemoveActivity(BacklogItem backlogItem, Guid activityId)
        {
            throw InvalidAction(nameof(RemoveActivity));
        }

        public virtual void AssignToSprint(BacklogItem backlogItem, Guid sprintId)
        {
            throw InvalidAction(nameof(AssignToSprint));
        }

        public virtual void RemoveFromSprint(BacklogItem backlogItem)
        {
            throw InvalidAction(nameof(RemoveFromSprint));
        }

        public virtual void StartWork(BacklogItem backlogItem)
        {
            throw InvalidAction(nameof(StartWork));
        }

        public virtual void MarkReadyForTesting(BacklogItem backlogItem)
        {
            throw InvalidAction(nameof(MarkReadyForTesting));
        }

        public virtual void StartTesting(BacklogItem backlogItem)
        {
            throw InvalidAction(nameof(StartTesting));
        }

        public virtual void MarkTested(BacklogItem backlogItem)
        {
            throw InvalidAction(nameof(MarkTested));
        }

        public virtual void ApproveDone(BacklogItem backlogItem)
        {
            throw InvalidAction(nameof(ApproveDone));
        }

        public virtual void ReturnToTodo(BacklogItem backlogItem)
        {
            throw InvalidAction(nameof(ReturnToTodo));
        }

        public virtual void ReturnToReadyForTesting(BacklogItem backlogItem)
        {
            throw InvalidAction(nameof(ReturnToReadyForTesting));
        }
    }
}
