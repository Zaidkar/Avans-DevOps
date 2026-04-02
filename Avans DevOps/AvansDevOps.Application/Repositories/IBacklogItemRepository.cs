using Avans_DevOps.AvansDevOps.Domain.Entities;

namespace Avans_DevOps.AvansDevOps.Application.Repositories
{
    public interface IBacklogItemRepository
    {
        List<(int Id, BacklogItem Item)> GetAll();
        BacklogItem? GetById(int id);
        int Create(BacklogItem item);
        bool Update(int id, BacklogItem item);
        bool Delete(int id);
    }
}
