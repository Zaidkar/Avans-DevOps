using Avans_DevOps.AvansDevOps.Domain.Entities;

namespace Avans_DevOps.AvansDevOps.Application.Services
{
    public interface IBacklogItemService
    {
        List<(Guid Id, BacklogItem Item)> GetAll();
        Sprint? GetSprintForBacklogItem(Guid backlogItemId);
        BacklogItem? GetById(Guid id);
        Guid Create(BacklogItem item);
        bool Update(Guid id, BacklogItem item);
        bool Delete(Guid id);
        bool StartWork(Guid id);
        bool MarkReadyForTesting(Guid id);
        bool StartTesting(Guid id);
        bool MarkTested(Guid id);
        bool ApproveDone(Guid id);
        bool ReturnToReadyForTesting(Guid id);
        bool ReturnToTodo(Guid id);
        bool AssignDeveloper(Guid backlogItemId, Guid userId);
    }
}
