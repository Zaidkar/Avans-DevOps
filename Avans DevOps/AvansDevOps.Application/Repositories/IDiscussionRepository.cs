using Avans_DevOps.AvansDevOps.Domain.Entities;

namespace Avans_DevOps.AvansDevOps.Application.Repositories
{
    internal interface IDiscussionRepository
    {
        List<(int Id, DiscussionThread Thread)> GetAll();
        DiscussionThread? GetById(int id);
        int Create(DiscussionThread thread);
        bool Update(int id, DiscussionThread thread);
        bool Delete(int id);
    }
}
