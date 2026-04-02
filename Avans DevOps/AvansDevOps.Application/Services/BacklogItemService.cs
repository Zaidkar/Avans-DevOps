using Avans_DevOps.AvansDevOps.Application.Notifications.Handlers;
using Avans_DevOps.AvansDevOps.Application.Repositories;
using Avans_DevOps.AvansDevOps.Domain.Entities;
using Avans_DevOps.AvansDevOps.Domain.Enum;

namespace Avans_DevOps.AvansDevOps.Application.Services
{
    public class BacklogItemService(IBacklogItemRepository backlogItemRepository, ISprintRepository sprintRepository, IBacklogItemNotificationHandler backlogNotificationHandler, IUserRepository userRepository) : IBacklogItemService
    {
        private readonly IBacklogItemRepository _backlogItemRepository = backlogItemRepository;
        private readonly ISprintRepository _sprintRepository = sprintRepository;
        private readonly IBacklogItemNotificationHandler _backlogNotificationHandler = backlogNotificationHandler;
        private readonly IUserRepository _userRepository = userRepository;

        public List<(Guid Id, BacklogItem Item)> GetAll()
        {
            return _backlogItemRepository.GetAll();
        }

        public Sprint? GetSprintForBacklogItem(Guid backlogItemId)
        {
            return _backlogItemRepository.GetSprintForBacklogItem(backlogItemId);
        }

        public BacklogItem? GetById(Guid id)
        {
            return _backlogItemRepository.GetById(id);
        }

        public Guid Create(BacklogItem item)
        {
            return _backlogItemRepository.Create(item);
        }

        public bool Update(Guid id, BacklogItem item)
        {
            return _backlogItemRepository.Update(id, item);
        }

        public bool Delete(Guid id)
        {
            return _backlogItemRepository.Delete(id);
        }

        public bool StartWork(Guid id)
        {
            var backlogItem = _backlogItemRepository.GetById(id);
            if (backlogItem == null)
            {
                return false;
            }

            backlogItem.StartWork();
            return _backlogItemRepository.Update(id, backlogItem);
        }

        public bool MarkReadyForTesting(Guid id)
        {
            Console.WriteLine($"[BacklogItemService] Marking backlog item {id} as ready for testing");
            var backlogItem = _backlogItemRepository.GetById(id);
            if (backlogItem == null)
            {
                return false;
            }

            var sprint = _backlogItemRepository.GetSprintForBacklogItem(id);
            if (sprint == null)
            {
                 Console.WriteLine($"[BacklogItemService] No sprint found for backlog item {id}");
                return false;
               
            }

            backlogItem.MarkReadyForTesting();
            _backlogItemRepository.Update(id, backlogItem);

            var recipients = _sprintRepository.GetMembersByRole(sprint.Id, SprintRole.Tester);
            _backlogNotificationHandler.NotifyReadyForTesting(backlogItem.Title, recipients);
            return true;
        }

        public bool StartTesting(Guid id)
        {
            var backlogItem = _backlogItemRepository.GetById(id);
            if (backlogItem == null)
            {
                return false;
            }

            backlogItem.StartTesting();
            return _backlogItemRepository.Update(id, backlogItem);
        }

        public bool MarkTested(Guid id)
        {
            var backlogItem = _backlogItemRepository.GetById(id);
            if (backlogItem == null)
            {
                return false;
            }

            backlogItem.MarkTested();
            return _backlogItemRepository.Update(id, backlogItem);
        }

        public bool ApproveDone(Guid id)
        {
            var backlogItem = _backlogItemRepository.GetById(id);
            if (backlogItem == null)
            {
                return false;
            }

            backlogItem.ApproveDone();
            return _backlogItemRepository.Update(id, backlogItem);
        }

        public bool ReturnToReadyForTesting(Guid id)
        {
            var backlogItem = _backlogItemRepository.GetById(id);
            if (backlogItem == null)
            {
                return false;
            }
            var sprint = _backlogItemRepository.GetSprintForBacklogItem(id);
            if (sprint == null)            {
                return false;
            }
            backlogItem.ReturnToReadyForTesting();
            var recipients = _sprintRepository.GetMembersByRole(sprint.Id, SprintRole.Tester);
            _backlogNotificationHandler.NotifyReadyForTesting(backlogItem.Title, recipients);
            return _backlogItemRepository.Update(id, backlogItem);

        }

        public bool ReturnToTodo(Guid id)
        {
            var backlogItem = _backlogItemRepository.GetById(id);
            if (backlogItem == null)
            {
                return false;
            }

            var assignedDeveloper = backlogItem.AssignedDeveloper;
            if (assignedDeveloper == null)
            {
                    Console.WriteLine($"[BacklogItemService] No assigned developer for backlog item {id}");
                return false;
            }

            var sprint = _backlogItemRepository.GetSprintForBacklogItem(id);
            if (sprint == null)
            {
                return false;
            }

            backlogItem.ReturnToTodo();
            _backlogItemRepository.Update(id, backlogItem);

            var recipients = _sprintRepository.GetMembersByRole(sprint.Id, SprintRole.ScrumMaster);
            _backlogNotificationHandler.NotifyBackToTodo(backlogItem.Title, assignedDeveloper, recipients);
            return true;
        }

        public bool AssignDeveloper(Guid backlogItemId, Guid userId)
        {
            var backlogItem = _backlogItemRepository.GetById(backlogItemId);
            if (backlogItem == null)            {
                return false;
            }
            var developer = _userRepository.GetById(userId);
            if (developer == null)
            {
                return false;
            }
            backlogItem.AssignDeveloper(developer);
             return _backlogItemRepository.Update(backlogItemId, backlogItem);
        }
    }
}
