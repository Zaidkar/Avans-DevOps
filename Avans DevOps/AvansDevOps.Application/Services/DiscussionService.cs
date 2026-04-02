using Avans_DevOps.AvansDevOps.Application.Notifications.Handlers;
using Avans_DevOps.AvansDevOps.Application.Repositories;
using Avans_DevOps.AvansDevOps.Domain.Entities;

namespace Avans_DevOps.AvansDevOps.Application.Services
{
    public class DiscussionService(
        IDiscussionRepository discussionRepository,
        IBacklogItemRepository backlogItemRepository,
        IUserRepository userRepository,
        IDiscussionNotificationHandler discussionNotificationHandler) : IDiscussionService
    {
        private readonly IDiscussionRepository _discussionRepository = discussionRepository;
        private readonly IBacklogItemRepository _backlogItemRepository = backlogItemRepository;
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
            EnsureBacklogItemIsOpen(thread.BacklogItemId);
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

            if (IsBacklogItemDone(thread.BacklogItemId))
            {
                thread.Lock();
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

        private void EnsureBacklogItemIsOpen(Guid backlogItemId)
        {
            if (IsBacklogItemDone(backlogItemId))
            {
                throw new InvalidOperationException("Discussion threads cannot be created for a backlog item that is already done.");
            }
        }

        private bool IsBacklogItemDone(Guid backlogItemId)
        {
            var backlogItem = _backlogItemRepository.GetById(backlogItemId);
            return backlogItem is not null && backlogItem.IsDone();
        }
    }
}
