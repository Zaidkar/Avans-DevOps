using Avans_DevOps.AvansDevOps.Domain.Entities;

namespace Avans_DevOps.AvansDevOps.Application.Services
{
    public interface IDiscussionService
    {
        List<(Guid Id, DiscussionThread Thread)> GetAll();
        DiscussionThread? GetById(Guid id);
        Guid Create(DiscussionThread thread);
        bool Reply(Guid discussionId, DiscussionPost post);
        bool Update(Guid id, DiscussionThread thread);
        bool Delete(Guid id);
    }
}
