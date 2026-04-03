using Avans_DevOps.AvansDevOps.Application.Notifications.Simple;
using Avans_DevOps.AvansDevOps.Application.Pipeline;
using Avans_DevOps.AvansDevOps.Application.Repositories;
using Avans_DevOps.AvansDevOps.Domain.Entities;
using Avans_DevOps.AvansDevOps.Domain.Entities.Pipeline;

namespace Avans_DevOps.AvansDevOps.Application.Services
{
    public class PipelineService(
        ISprintRepository sprintRepository,
        IPipelineFactory pipelineFactory,
        IEventManager eventManager) : IPipelineService
    {
        private readonly ISprintRepository _sprintRepository = sprintRepository;
        private readonly IPipelineFactory _pipelineFactory = pipelineFactory;
        private readonly IEventManager _eventManager = eventManager;

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

            _eventManager.Notify(NotificationEventNames.ReleaseSuccess, new NotificationEventData
            {
                EventType = NotificationEventNames.ReleaseSuccess,
                SprintId = sprint.Id,
                Subject = "Pipeline activities successful",
                Body = $"All pipeline activities for sprint {sprint.Name} were executed successfully."
            });
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

            _eventManager.Notify(NotificationEventNames.ReleaseFailure, new NotificationEventData
            {
                EventType = NotificationEventNames.ReleaseFailure,
                SprintId = sprint.Id,
                Subject = "Pipeline activity failed",
                Body = $"A pipeline activity failed during the release of sprint {sprint.Name}."
            });
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

            _eventManager.Notify(NotificationEventNames.ReleaseCancelled, new NotificationEventData
            {
                EventType = NotificationEventNames.ReleaseCancelled,
                SprintId = sprint.Id,
                Subject = "Pipeline release cancelled",
                Body = $"Release of sprint {sprint.Name} has been cancelled."
            });
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

    }
}
