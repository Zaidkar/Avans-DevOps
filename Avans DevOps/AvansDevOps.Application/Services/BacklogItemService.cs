using Avans_DevOps.AvansDevOps.Application.Notifications.Simple;
using Avans_DevOps.AvansDevOps.Application.Repositories;
using Avans_DevOps.AvansDevOps.Domain.Entities;

namespace Avans_DevOps.AvansDevOps.Application.Services
{
    public class BacklogItemService(IBacklogItemRepository backlogItemRepository, ISprintRepository sprintRepository, IEventManager eventManager, IUserRepository userRepository) : IBacklogItemService
    {
        private readonly IBacklogItemRepository _backlogItemRepository = backlogItemRepository;
        private readonly ISprintRepository _sprintRepository = sprintRepository;
        private readonly IEventManager _eventManager = eventManager;
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

            _eventManager.Notify(NotificationEventNames.ReadyForTesting, new NotificationEventData
            {
                EventType = NotificationEventNames.ReadyForTesting,
                SprintId = sprint.Id,
                Subject = "Backlog item ready for testing",
                Body = $"Backlogitem {backlogItem.Title} is ready for testing"
            });
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
            _eventManager.Notify(NotificationEventNames.ReadyForTesting, new NotificationEventData
            {
                EventType = NotificationEventNames.ReadyForTesting,
                SprintId = sprint.Id,
                Subject = "Backlog item ready for testing",
                Body = $"Backlogitem {backlogItem.Title} is ready for testing"
            });
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

            _eventManager.Notify(NotificationEventNames.TestFailure, new NotificationEventData
            {
                EventType = NotificationEventNames.TestFailure,
                SprintId = sprint.Id,
                Subject = "Backlog item rejected after testing",
                Body = $"Backlogitem {backlogItem.Title} developed door {assignedDeveloper.Name} is teruggezet naar todo"
            });
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
