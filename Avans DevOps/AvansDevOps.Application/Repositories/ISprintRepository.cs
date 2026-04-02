using Avans_DevOps.AvansDevOps.Domain.Entities;

namespace Avans_DevOps.AvansDevOps.Application.Repositories
{
    public interface ISprintRepository
    {
        List<Sprint> GetAll();
        Sprint? GetById(int id);
        Sprint Create(Sprint sprint);
        bool Update(int id, Sprint sprint);
        bool Delete(int id);
    }
}
