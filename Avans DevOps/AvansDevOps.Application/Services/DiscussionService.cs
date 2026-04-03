using Avans_DevOps.AvansDevOps.Application.Notifications.Simple;
using Avans_DevOps.AvansDevOps.Application.Repositories;
using Avans_DevOps.AvansDevOps.Domain.Entities;

namespace Avans_DevOps.AvansDevOps.Application.Services
{
    public class DiscussionService(
        IDiscussionRepository discussionRepository,
        IBacklogItemRepository backlogItemRepository,
        IEventManager eventManager) : IDiscussionService
    {
        private readonly IDiscussionRepository _discussionRepository = discussionRepository;
        private readonly IBacklogItemRepository _backlogItemRepository = backlogItemRepository;
        private readonly IEventManager _eventManager = eventManager;

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
            var sprint = _backlogItemRepository.GetSprintForBacklogItem(thread.BacklogItemId);
            if (sprint != null)
            {
                _eventManager.Notify(NotificationEventNames.DiscussionCreated, new NotificationEventData
                {
                    EventType = NotificationEventNames.DiscussionCreated,
                    SprintId = sprint.Id,
                    Subject = "Discussion created",
                    Body = $"Er is een nieuwe discussie gestart over {thread.Subject}"
                });
            }
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

            var sprint = _backlogItemRepository.GetSprintForBacklogItem(thread.BacklogItemId);
            if (sprint != null)
            {
                _eventManager.Notify(NotificationEventNames.DiscussionReply, new NotificationEventData
                {
                    EventType = NotificationEventNames.DiscussionReply,
                    SprintId = sprint.Id,
                    Subject = "Discussion reply",
                    Body = $"Er is een nieuwe reactie geplaatst over {thread.Subject}:{post.Author} zei: {post.Message}"
                });
            }
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
