using Avans_DevOps.AvansDevOps.Domain.Entities;

namespace Avans_DevOps.AvansDevOps.Application.Services
{
    public interface IActivityService
    {
        List<Activity> GetAll(Guid backlogItemId);
        Activity? GetById(Guid backlogItemId, Guid activityId);
        Guid AddActivity(Guid backlogItemId, Activity activity);
        bool RemoveActivity(Guid backlogItemId, Guid activityId);
        bool ChangeTitle(Guid backlogItemId, Guid activityId, string title);
        bool ChangeDescription(Guid backlogItemId, Guid activityId, string description);
        bool AssignDeveloper(Guid backlogItemId, Guid activityId, Guid userId);
        bool StartWork(Guid backlogItemId, Guid activityId);
        bool MarkDone(Guid backlogItemId, Guid activityId);
    }
}
