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

        public List<(int Id, DiscussionThread Thread)> GetAll()
        {
            return _discussionRepository.GetAll();
        }

        public DiscussionThread? GetById(int id)
        {
            return _discussionRepository.GetById(id);
        }

        public int Create(DiscussionThread thread, string discussionTitle)
        {
            var id = _discussionRepository.Create(thread);
            _discussionNotificationHandler.NotifyDiscussionCreated(discussionTitle, _userRepository.GetAll());
            return id;
        }

        public bool Reply(int discussionId, string discussionTitle)
        {
            var thread = _discussionRepository.GetById(discussionId);
            if (thread == null)
            {
                return false;
            }

            _discussionNotificationHandler.NotifyDiscussionReply(discussionTitle, _userRepository.GetAll());
            return true;
        }

        public bool Update(int id, DiscussionThread thread)
        {
            return _discussionRepository.Update(id, thread);
        }

        public bool Delete(int id)
        {
            return _discussionRepository.Delete(id);
        }
    }
}
