using Avans_DevOps.AvansDevOps.Domain.Entities;
using Avans_DevOps.AvansDevOps.Domain.Entities.Pipeline;
using Avans_DevOps.AvansDevOps.Domain.Enum;

namespace Avans_DevOps.AvansDevOps.Application.Services
{
    public interface ISprintService
    {
        List<Sprint> GetAll();
        Sprint? GetById(Guid id);
        Sprint Create(Sprint sprint);
        bool Update(Guid id, Sprint sprint);
        bool Rename(Guid sprintId, string name);
        bool ChangePlanning(Guid sprintId, DateOnly startDate, DateOnly endDate);
        bool AddBacklogItem(Guid sprintId, Guid backlogItemId);
        bool RemoveBacklogItem(Guid sprintId, Guid backlogItemId);
        bool AddMemberToSprint(Guid sprintId, Guid userId, SprintRole role);
        bool RemoveMemberFromSprint(Guid sprintId, Guid userId);
        bool AssignPipeline(Guid sprintId, PipelineDefinition pipeline);
        bool AssignBuildValidationPipeline(Guid sprintId, string pipelineName);
        bool AssignDeploymentPipeline(Guid sprintId, string pipelineName);
        bool UploadReviewSummary(Guid sprintId, string documentPath);
        bool Start(Guid sprintId);
        bool Delete(Guid id);
        bool FinishSprint(Guid id);
        bool BeginRelease(Guid sprintId);
        bool ExecuteReleasePipeline(Guid sprintId);
        bool ReleaseSucceeded(Guid sprintId);
        bool ReleaseFailed(Guid sprintId);
        bool RetryRelease(Guid sprintId);
        bool CancelRelease(Guid sprintId);
        bool CloseReview(Guid sprintId);
        bool NotifyReleaseResult(Guid id, bool releaseSucceeded);
    }
}
