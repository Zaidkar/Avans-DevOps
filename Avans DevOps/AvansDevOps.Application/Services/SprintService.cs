using Avans_DevOps.AvansDevOps.Application.Notifications.Simple;
using Avans_DevOps.AvansDevOps.Application.Repositories;
using Avans_DevOps.AvansDevOps.Domain.Entities;
using Avans_DevOps.AvansDevOps.Domain.Entities.Pipeline;
using Avans_DevOps.AvansDevOps.Domain.Enum;

namespace Avans_DevOps.AvansDevOps.Application.Services
{
    public class SprintService(
        ISprintRepository sprintRepository,
        IUserRepository userRepository,
        IEventManager eventManager,
        IPipelineService pipelineService) : ISprintService
    {
        private readonly ISprintRepository _sprintRepository = sprintRepository;
        private readonly IUserRepository _userRepository = userRepository;
        private readonly IEventManager _eventManager = eventManager;
        private readonly IPipelineService _pipelineService = pipelineService;

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

        public bool AssignPipeline(Guid sprintId, PipelineDefinition pipeline)
        {
            return _pipelineService.AssignPipeline(sprintId, pipeline);
        }

        public bool AssignBuildValidationPipeline(Guid sprintId, string pipelineName)
        {
            return _pipelineService.AssignBuildValidationPipeline(sprintId, pipelineName);
        }

        public bool AssignDeploymentPipeline(Guid sprintId, string pipelineName)
        {
            return _pipelineService.AssignDeploymentPipeline(sprintId, pipelineName);
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

            sprint.Finish();
            var updated = _sprintRepository.Update(id, sprint);
            if (!updated)
            {
                return false;
            }

            _eventManager.Notify(NotificationEventNames.SprintFinished, new NotificationEventData
            {
                EventType = NotificationEventNames.SprintFinished,
                SprintId = sprint.Id,
                Subject = "Sprint finished",
                Body = $"Sprint {sprint.Name} has reached the finished state."
            });

            return true;
        }

        public bool BeginRelease(Guid sprintId)
        {
            return _pipelineService.BeginRelease(sprintId);
        }

        public bool ExecuteReleasePipeline(Guid sprintId)
        {
            return _pipelineService.ExecuteReleasePipeline(sprintId);
        }

        public bool ReleaseSucceeded(Guid sprintId)
        {
            return _pipelineService.ReleaseSucceeded(sprintId);
        }

        public bool ReleaseFailed(Guid sprintId)
        {
            return _pipelineService.ReleaseFailed(sprintId);
        }

        public bool RetryRelease(Guid sprintId)
        {
            return _pipelineService.RetryRelease(sprintId);
        }

        public bool CancelRelease(Guid sprintId)
        {
            return _pipelineService.CancelRelease(sprintId);
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
            return _pipelineService.NotifyReleaseResult(id, releaseSucceeded);
        }

        
    }
}
