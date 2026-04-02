using Avans_DevOps.AvansDevOps.Application.Notifications.Handlers;
using Avans_DevOps.AvansDevOps.Application.Repositories;
using Avans_DevOps.AvansDevOps.Domain.Entities;

namespace Avans_DevOps.AvansDevOps.Application.Services
{
    public class BacklogItemService(IBacklogItemRepository backlogItemRepository, IUserRepository userRepository, IBacklogItemNotificationHandler backlogNotificationHandler)
    {
        private readonly IBacklogItemRepository _backlogItemRepository = backlogItemRepository;
        private readonly IUserRepository _userRepository = userRepository;
        private readonly IBacklogItemNotificationHandler _backlogNotificationHandler = backlogNotificationHandler;

        public List<(int Id, BacklogItem Item)> GetAll()
        {
            return _backlogItemRepository.GetAll();
        }

        public BacklogItem? GetById(int id)
        {
            return _backlogItemRepository.GetById(id);
        }

        public int Create(BacklogItem item)
        {
            return _backlogItemRepository.Create(item);
        }

        public bool Update(int id, BacklogItem item)
        {
            return _backlogItemRepository.Update(id, item);
        }

        public bool Delete(int id)
        {
            return _backlogItemRepository.Delete(id);
        }

        public bool MarkReadyForTesting(int id, string backlogItemTitle)
        {
            var backlogItem = _backlogItemRepository.GetById(id);
            if (backlogItem == null)
            {
                return false;
            }

            var recipients = _userRepository.GetTesters();
            _backlogNotificationHandler.NotifyReadyForTesting(backlogItemTitle, recipients);
            return true;
        }

        public bool MoveBackToTodo(int id, string backlogItemTitle)
        {
            var backlogItem = _backlogItemRepository.GetById(id);
            if (backlogItem == null)
            {
                return false;
            }

            var recipients = _userRepository.GetAll();
            _backlogNotificationHandler.NotifyBackToTodo(backlogItemTitle, recipients);
            return true;
        }
    }
}
