using Avans_DevOps.AvansDevOps.Application.Notifications.Handlers;
using Avans_DevOps.AvansDevOps.Application.Repositories;
using Avans_DevOps.AvansDevOps.Domain.Entities;

namespace Avans_DevOps.AvansDevOps.Application.Services
{
    public class SprintService(ISprintRepository sprintRepository, IUserRepository userRepository, ISprintNotificationHandler sprintNotificationHandler)
    {
        private readonly ISprintRepository _sprintRepository = sprintRepository;
        private readonly IUserRepository _userRepository = userRepository;
        private readonly ISprintNotificationHandler _sprintNotificationHandler = sprintNotificationHandler;

        public List<Sprint> GetAll()
        {
            return _sprintRepository.GetAll();
        }

        public Sprint? GetById(int id)
        {
            return _sprintRepository.GetById(id);
        }

        public Sprint Create(Sprint sprint)
        {
            return _sprintRepository.Create(sprint);
        }

        public bool Update(int id, Sprint sprint)
        {
            return _sprintRepository.Update(id, sprint);
        }

        public bool Delete(int id)
        {
            return _sprintRepository.Delete(id);
        }

        public bool FinishSprint(int id)
        {
            var sprint = _sprintRepository.GetById(id);
            if (sprint == null)
            {
                return false;
            }

            var recipients = _userRepository.GetAll();
            _sprintNotificationHandler.NotifySprintFinished(sprint.Name, recipients);
            return true;
        }

        public bool NotifyReleaseResult(int id, bool releaseSucceeded)
        {
            var sprint = _sprintRepository.GetById(id);
            if (sprint == null)
            {
                return false;
            }

            var recipients = _userRepository.GetAll();
            if (releaseSucceeded)
            {
                _sprintNotificationHandler.NotifyReleaseSuccess(sprint.Name, recipients);
            }
            else
            {
                _sprintNotificationHandler.NotifyReleaseFailure(sprint.Name, recipients);
            }

            return true;
        }
    }
}
