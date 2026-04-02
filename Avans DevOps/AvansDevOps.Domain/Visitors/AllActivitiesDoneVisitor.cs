using Avans_DevOps.AvansDevOps.Domain.Entities;
using Avans_DevOps.AvansDevOps.Domain.Interfaces;

namespace Avans_DevOps.AvansDevOps.Domain.Visitors
{
    public class AllActivitiesDoneVisitor : IBacklogWorkItemVisitor
    {
        public bool AllActivitiesDone { get; private set; } = true;

        public void VisitBacklogItem(BacklogItem backlogItem)
        {
        }

        public void VisitActivity(Activity activity)
        {
            if (!activity.IsDone())
                AllActivitiesDone = false;
        }
    }
}