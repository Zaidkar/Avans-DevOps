using Avans_DevOps.AvansDevOps.Domain.Entities;

namespace Avans_DevOps.AvansDevOps.Domain.States.BacklogItemStates
{
    public class ReadyForTestingBacklogItemState : BacklogItemState
    {
        public override string Name => "ReadyForTesting";

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

        public override void AddActivity(BacklogItem backlogItem, Activity activity)
        {
            backlogItem.AddActivityInternal(activity);
        }

        public override void RemoveActivity(BacklogItem backlogItem, Guid activityId)
        {
            backlogItem.RemoveActivityInternal(activityId);
        }

        public override void AssignToSprint(BacklogItem backlogItem, Guid sprintId)
        {
            backlogItem.AssignToSprintInternal(sprintId);
        }

        public override void RemoveFromSprint(BacklogItem backlogItem)
        {
            backlogItem.RemoveFromSprintInternal();
        }

        public override void StartTesting(BacklogItem backlogItem)
        {
            backlogItem.SetTestingState();
        }

        public override void ReturnToTodo(BacklogItem backlogItem)
        {
            backlogItem.SetTodoState();
        }
    }
}