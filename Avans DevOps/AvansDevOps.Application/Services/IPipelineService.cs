using Avans_DevOps.AvansDevOps.Domain.Entities.Pipeline;

namespace Avans_DevOps.AvansDevOps.Application.Services
{
    public interface IPipelineService
    {
        bool AssignBuildValidationPipeline(Guid sprintId, string pipelineName);
        bool AssignDeploymentPipeline(Guid sprintId, string pipelineName);
        bool BeginRelease(Guid sprintId);
        bool ExecuteReleasePipeline(Guid sprintId);
        bool ReleaseSucceeded(Guid sprintId);
        bool ReleaseFailed(Guid sprintId);
        bool CancelRelease(Guid sprintId);
        bool NotifyReleaseResult(Guid sprintId, bool releaseSucceeded);
        bool AssignPipeline(Guid sprintId, PipelineDefinition pipeline);
    }
}
