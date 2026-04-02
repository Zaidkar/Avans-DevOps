using Avans_DevOps.AvansDevOps.Application.Notifications.Handlers;
using Avans_DevOps.AvansDevOps.Application.Pipeline;
using Avans_DevOps.AvansDevOps.Application.Repositories;
using Avans_DevOps.AvansDevOps.Domain.Entities;
using Avans_DevOps.AvansDevOps.Domain.Entities.Pipeline;
using Avans_DevOps.AvansDevOps.Domain.Enum;

namespace Avans_DevOps.AvansDevOps.Application.Services
{
    public class PipelineService(
        ISprintRepository sprintRepository,
        IPipelineFactory pipelineFactory,
        IPipelineReleaseNotificationHandler pipelineReleaseNotificationHandler) : IPipelineService
    {
        private readonly ISprintRepository _sprintRepository = sprintRepository;
        private readonly IPipelineFactory _pipelineFactory = pipelineFactory;
        private readonly IPipelineReleaseNotificationHandler _pipelineReleaseNotificationHandler = pipelineReleaseNotificationHandler;

        public bool AssignBuildValidationPipeline(Guid sprintId, string pipelineName)
        {
            var pipeline = _pipelineFactory.CreateBuildValidationPipeline(pipelineName);
            return AssignPipeline(sprintId, pipeline);
        }

        public bool AssignDeploymentPipeline(Guid sprintId, string pipelineName)
        {
            var pipeline = _pipelineFactory.CreateDeploymentPipeline(pipelineName);
            return AssignPipeline(sprintId, pipeline);
        }

        public bool AssignPipeline(Guid sprintId, PipelineDefinition pipeline)
        {
            var sprint = _sprintRepository.GetById(sprintId);
            if (sprint == null)
            {
                return false;
            }

            sprint.AssignPipeline(pipeline);
            return _sprintRepository.Update(sprintId, sprint);
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

        public bool ExecuteReleasePipeline(Guid sprintId)
        {
            var sprint = _sprintRepository.GetById(sprintId);
            if (sprint == null || sprint.Pipeline == null)
            {
                return false;
            }

            var executionResult = sprint.Pipeline.Execute();
            return NotifyReleaseResult(sprintId, executionResult.Succeeded);
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

            var recipients = GetRecipientsByRoles(sprintId, SprintRole.ScrumMaster, SprintRole.ProductOwner);
            _pipelineReleaseNotificationHandler.NotifyPipelineCompletedSuccessfully(sprint.Name, recipients);
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

            var recipients = GetRecipientsByRoles(sprintId, SprintRole.ScrumMaster);
            _pipelineReleaseNotificationHandler.NotifyPipelineActivityFailed(sprint.Name, recipients);
            return true;
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

            var recipients = GetRecipientsByRoles(sprintId, SprintRole.ScrumMaster, SprintRole.ProductOwner);
            _pipelineReleaseNotificationHandler.NotifyReleaseCancelled(sprint.Name, recipients);
            return true;
        }

        public bool RetryRelease(Guid sprintId)
        {
            var sprint = _sprintRepository.GetById(sprintId);
            if (sprint == null || sprint.Pipeline == null)
            {
                return false;
            }

            try
            {
                sprint.RetryRelease();
                _sprintRepository.Update(sprintId, sprint);
            }
            catch (InvalidOperationException)
            {
                return false;
            }

            var executionResult = sprint.Pipeline.Execute();
            return NotifyReleaseResult(sprintId, executionResult.Succeeded);
        }

        public bool NotifyReleaseResult(Guid sprintId, bool releaseSucceeded)
        {
            return releaseSucceeded
                ? ReleaseSucceeded(sprintId)
                : ReleaseFailed(sprintId);
        }

        private List<User> GetRecipientsByRoles(Guid sprintId, params SprintRole[] roles)
        {
            var recipientsById = new Dictionary<Guid, User>();

            foreach (var role in roles)
            {
                foreach (var user in _sprintRepository.GetMembersByRole(sprintId, role))
                {
                    recipientsById[user.Id] = user;
                }
            }

            return recipientsById.Values.ToList();
        }
    }
}
