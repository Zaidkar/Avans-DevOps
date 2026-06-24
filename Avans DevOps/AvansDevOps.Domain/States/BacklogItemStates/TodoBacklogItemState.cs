using Avans_DevOps.AvansDevOps.Domain.Entities;
using System;

namespace Avans_DevOps.AvansDevOps.Domain.States.BacklogItemStates
{
    public class TodoBacklogItemState : BacklogItemState
    {
        public override string Name => "Todo";

        public override void ChangeTitle(BacklogItem backlogItem, string title)
        {
            backlogItem.ChangeTitleInternal(title);
        }

        public override void ChangeDescription(BacklogItem backlogItem, string description)
        {
            backlogItem.ChangeDescriptionInternal(description);
        }

        public override void ChangeStoryPoints(BacklogItem backlogItem, int storyPoints)
        {
            backlogItem.ChangeStoryPointsInternal(storyPoints);
        }

        public override void AssignDeveloper(BacklogItem backlogItem, User developer)
        {
            backlogItem.AssignDeveloperInternal(developer);
            backlogItem.SetDoingState();
        }

        public override void UnassignDeveloper(BacklogItem backlogItem)
        {
            backlogItem.UnassignDeveloperInternal();
        }

        public override void AddActivity(BacklogItem backlogItem, Activity activity)
        {
            backlogItem.AddActivityInternal(activity);
        }

        public override void RemoveActivity(BacklogItem backlogItem, Guid activityId)
        {
            backlogItem.RemoveActivityInternal(activityId);
        }

        public override void StartWork(BacklogItem backlogItem)
        {
            if (!backlogItem.HasAssignedDeveloper())
                throw new InvalidOperationException(
                    "A backlog item must have an assigned developer before work can start.");

            backlogItem.SetDoingState();
        }
    }
}