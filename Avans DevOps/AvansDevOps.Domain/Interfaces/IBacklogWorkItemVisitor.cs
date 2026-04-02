using Avans_DevOps.AvansDevOps.Domain.Entities;

namespace Avans_DevOps.AvansDevOps.Domain.Interfaces
{
    public interface IBacklogWorkItemVisitor
    {
        void VisitBacklogItem(BacklogItem backlogItem);
        void VisitActivity(Activity activity);
    }
}