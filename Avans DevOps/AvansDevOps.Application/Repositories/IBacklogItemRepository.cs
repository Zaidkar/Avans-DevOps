using Avans_DevOps.AvansDevOps.Domain.Entities;

namespace Avans_DevOps.AvansDevOps.Application.Repositories
{
    public interface IBacklogItemRepository
    {
        List<(Guid Id, BacklogItem Item)> GetAll();
        BacklogItem? GetById(Guid id);
        Guid Create(BacklogItem item);
        bool Update(Guid id, BacklogItem item);
        bool Delete(Guid id);
        Sprint? GetSprintForBacklogItem(Guid backlogItemId);
    }
}
