using Avans_DevOps.AvansDevOps.Domain.Entities;

namespace Avans_DevOps.AvansDevOps.Application.Repositories
{
    internal interface IDiscussionRepository
    {
        List<(Guid Id, DiscussionThread Thread)> GetAll();
        DiscussionThread? GetById(Guid id);
        Guid Create(DiscussionThread thread);
        bool Update(Guid id, DiscussionThread thread);
        bool Delete(Guid id);
    }
}
