using Avans_DevOps.AvansDevOps.Application.Notifications.Handlers;
using Avans_DevOps.AvansDevOps.Application.Repositories;
using Avans_DevOps.AvansDevOps.Domain.Entities;

namespace Avans_DevOps.AvansDevOps.Application.Services
{
    internal class DiscussionService(IDiscussionRepository discussionRepository, IUserRepository userRepository, IDiscussionNotificationHandler discussionNotificationHandler)
    {
        private readonly IDiscussionRepository _discussionRepository = discussionRepository;
        private readonly IUserRepository _userRepository = userRepository;
        private readonly IDiscussionNotificationHandler _discussionNotificationHandler = discussionNotificationHandler;

        public List<(Guid Id, DiscussionThread Thread)> GetAll()
        {
            return _discussionRepository.GetAll();
        }

        public DiscussionThread? GetById(Guid id)
        {
            return _discussionRepository.GetById(id);
        }

        public Guid Create(DiscussionThread thread)
        {
            var id = _discussionRepository.Create(thread);
            _discussionNotificationHandler.NotifyDiscussionCreated(thread.Subject, _userRepository.GetAll());
            return id;
        }

        public bool Reply(Guid discussionId, DiscussionPost post)
        {
            var thread = _discussionRepository.GetById(discussionId);
            if (thread == null)
            {
                return false;
            }

            thread.AddPost(post);
            _discussionRepository.Update(discussionId, thread);

            _discussionNotificationHandler.NotifyDiscussionReply(thread.Subject, _userRepository.GetAll());
            return true;
        }

        public bool Update(Guid id, DiscussionThread thread)
        {
            return _discussionRepository.Update(id, thread);
        }

        public bool Delete(Guid id)
        {
            return _discussionRepository.Delete(id);
        }
    }
}
