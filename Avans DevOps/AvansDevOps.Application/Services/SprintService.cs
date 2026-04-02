using Avans_DevOps.AvansDevOps.Application.Notifications.Handlers;
using Avans_DevOps.AvansDevOps.Application.Repositories;
using Avans_DevOps.AvansDevOps.Domain.Entities;
using Avans_DevOps.AvansDevOps.Domain.Enum;

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

        public Sprint? GetById(Guid id)
        {
            return _sprintRepository.GetById(id);
        }

        public Sprint Create(Sprint sprint)
        {
            return _sprintRepository.Create(sprint);
        }

        public bool Update(Guid id, Sprint sprint)
        {
            return _sprintRepository.Update(id, sprint);
        }

        public bool Rename(Guid sprintId, string name)
        {
            var sprint = _sprintRepository.GetById(sprintId);
            if (sprint == null)
            {
                return false;
            }

            sprint.Rename(name);
            return _sprintRepository.Update(sprintId, sprint);
        }

        public bool ChangePlanning(Guid sprintId, DateOnly startDate, DateOnly endDate)
        {
            var sprint = _sprintRepository.GetById(sprintId);
            if (sprint == null)
            {
                return false;
            }

            sprint.ChangePlanning(startDate, endDate);
            return _sprintRepository.Update(sprintId, sprint);
        }
        public bool AddBacklogItem(Guid sprintId, Guid backlogItemId)
        {
            var sprint = _sprintRepository.GetById(sprintId);
            if (sprint == null)
            {
                return false;
            }

            sprint.AddBacklogItem(backlogItemId);
            return _sprintRepository.Update(sprintId, sprint);
        }

        public bool RemoveBacklogItem(Guid sprintId, Guid backlogItemId)
        {
            var sprint = _sprintRepository.GetById(sprintId);
            if (sprint == null)
            {
                return false;
            }

            sprint.RemoveBacklogItem(backlogItemId);
            return _sprintRepository.Update(sprintId, sprint);
        }

        public bool AddMemberToSprint(Guid sprintId, Guid userId, SprintRole role)
        {
            var sprint = _sprintRepository.GetById(sprintId);
            if (sprint == null)
            {
                return false;
            }

            var user = _userRepository.GetById(userId);
            if (user == null)
            {
                return false;
            }

            var member = new SprintMember(Guid.NewGuid(), user, role);
            sprint.AddMember(member);

            return _sprintRepository.Update(sprintId, sprint);
        }

        public bool RemoveMemberFromSprint(Guid sprintId, Guid userId)
        {
            var sprint = _sprintRepository.GetById(sprintId);
            if (sprint == null)
            {
                return false;
            }

            sprint.RemoveMember(userId);
            return _sprintRepository.Update(sprintId, sprint);
        }

        public bool AssignPipeline(Guid sprintId, Guid pipelineId)
        {
            var sprint = _sprintRepository.GetById(sprintId);
            if (sprint == null)
            {
                return false;
            }

            sprint.AssignPipeline(pipelineId);
            return _sprintRepository.Update(sprintId, sprint);
        }

        public bool UploadReviewSummary(Guid sprintId, string documentPath)
        {
            var sprint = _sprintRepository.GetById(sprintId);
            if (sprint == null)
            {
                return false;
            }

            sprint.UploadReviewSummary(documentPath);
            return _sprintRepository.Update(sprintId, sprint);
        }

        public bool Start(Guid sprintId)
        {
            var sprint = _sprintRepository.GetById(sprintId);
            if (sprint == null)
            {
                return false;
            }

            sprint.Start();
            return _sprintRepository.Update(sprintId, sprint);
        }

        public bool Delete(Guid id)
        {
            return _sprintRepository.Delete(id);
        }

        public bool FinishSprint(Guid id)
        {
            var sprint = _sprintRepository.GetById(id);
            if (sprint == null)
            {
                return false;
            }

            // Notify sprint members instead of all users
            var members = _sprintRepository.GetMembers(id);
            var recipients = members.Select(m => m.User).ToList();
            _sprintNotificationHandler.NotifySprintFinished(sprint.Name, recipients);
            return true;
        }

        public bool BeginRelease(Guid sprintId)
        {
            var sprint = _sprintRepository.GetById(sprintId);
            if (sprint == null)
            {
                return false;
            }

            sprint.BeginRelease();
            return _sprintRepository.Update(sprintId, sprint);
        }

        public bool ReleaseSucceeded(Guid sprintId)
        {
            var sprint = _sprintRepository.GetById(sprintId);
            if (sprint == null)
            {
                return false;
            }

            sprint.ReleaseSucceeded();
            _sprintRepository.Update(sprintId, sprint);

            var members = _sprintRepository.GetMembers(sprintId);
            var recipients = members.Select(m => m.User).ToList();
            _sprintNotificationHandler.NotifyReleaseSuccess(sprint.Name, recipients);
            return true;
        }

        public bool ReleaseFailed(Guid sprintId)
        {
            var sprint = _sprintRepository.GetById(sprintId);
            if (sprint == null)
            {
                return false;
            }

            sprint.ReleaseFailed();
            _sprintRepository.Update(sprintId, sprint);

            var members = _sprintRepository.GetMembers(sprintId);
            var recipients = members.Select(m => m.User).ToList();
            _sprintNotificationHandler.NotifyReleaseFailure(sprint.Name, recipients);
            return true;
        }

        public bool RetryRelease(Guid sprintId)
        {
            var sprint = _sprintRepository.GetById(sprintId);
            if (sprint == null)
            {
                return false;
            }

            sprint.RetryRelease();
            return _sprintRepository.Update(sprintId, sprint);
        }

        public bool CancelRelease(Guid sprintId)
        {
            var sprint = _sprintRepository.GetById(sprintId);
            if (sprint == null)
            {
                return false;
            }

            sprint.CancelRelease();
            _sprintRepository.Update(sprintId, sprint);

            var recipients = _sprintRepository.GetMembersByRole(sprintId, SprintRole.ScrumMaster);
            recipients.AddRange(_sprintRepository.GetMembersByRole(sprintId, SprintRole.ProductOwner));
            _sprintNotificationHandler.NotifyReleaseFailure(sprint.Name, recipients);
            return true;
        }

        public bool CloseReview(Guid sprintId)
        {
            var sprint = _sprintRepository.GetById(sprintId);
            if (sprint == null)
            {
                return false;
            }

            sprint.CloseReview();
            return _sprintRepository.Update(sprintId, sprint);
        }

        public bool NotifyReleaseResult(Guid id, bool releaseSucceeded)
        {
            var sprint = _sprintRepository.GetById(id);
            if (sprint == null)
            {
                return false;
            }

            // Notify sprint members instead of all users
            var members = _sprintRepository.GetMembers(id);
            var recipients = members.Select(m => m.User).ToList();
            
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
