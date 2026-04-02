using Avans_DevOps.AvansDevOps.Application.Repositories;
using Avans_DevOps.AvansDevOps.Domain.Entities;

namespace Avans_DevOps.AvansDevOps.Application.Services
{
    public class ActivityService(IBacklogItemRepository backlogItemRepository, IUserRepository userRepository) : IActivityService
    {
        private readonly IBacklogItemRepository _backlogItemRepository = backlogItemRepository;
        private readonly IUserRepository _userRepository = userRepository;

        public List<Activity> GetAll(Guid backlogItemId)
        {
            var backlogItem = GetBacklogItemOrThrow(backlogItemId);
            return backlogItem.Activities.ToList();
        }

        public Activity? GetById(Guid backlogItemId, Guid activityId)
        {
            var backlogItem = _backlogItemRepository.GetById(backlogItemId);
            return backlogItem?.Activities.FirstOrDefault(activity => activity.Id == activityId);
        }

        public Guid AddActivity(Guid backlogItemId, Activity activity)
        {
            var backlogItem = GetBacklogItemOrThrow(backlogItemId);
            backlogItem.AddActivity(activity);
            _backlogItemRepository.Update(backlogItemId, backlogItem);
            return activity.Id;
        }

        public bool RemoveActivity(Guid backlogItemId, Guid activityId)
        {
            var backlogItem = GetBacklogItemOrThrow(backlogItemId);
            backlogItem.RemoveActivity(activityId);
            return _backlogItemRepository.Update(backlogItemId, backlogItem);
        }

        public bool ChangeTitle(Guid backlogItemId, Guid activityId, string title)
        {
            var activity = GetActivityOrThrow(backlogItemId, activityId);
            activity.ChangeTitle(title);
            return PersistBacklogItem(backlogItemId);
        }

        public bool ChangeDescription(Guid backlogItemId, Guid activityId, string description)
        {
            var activity = GetActivityOrThrow(backlogItemId, activityId);
            activity.ChangeDescription(description);
            return PersistBacklogItem(backlogItemId);
        }

        public bool AssignDeveloper(Guid backlogItemId, Guid activityId, Guid userId)
        {
            var activity = GetActivityOrThrow(backlogItemId, activityId);
            var developer = _userRepository.GetById(userId) ?? throw new InvalidOperationException($"User with id {userId} not found.");

            activity.AssignDeveloper(developer);
            return PersistBacklogItem(backlogItemId);
        }

        public bool StartWork(Guid backlogItemId, Guid activityId)
        {
            var activity = GetActivityOrThrow(backlogItemId, activityId);
            activity.StartWork();
            return PersistBacklogItem(backlogItemId);
        }

        public bool MarkDone(Guid backlogItemId, Guid activityId)
        {
            var activity = GetActivityOrThrow(backlogItemId, activityId);
            activity.MarkDone();
            return PersistBacklogItem(backlogItemId);
        }

        private BacklogItem GetBacklogItemOrThrow(Guid backlogItemId)
        {
            return _backlogItemRepository.GetById(backlogItemId)
                ?? throw new InvalidOperationException($"Backlog item with id {backlogItemId} not found.");
        }

        private Activity GetActivityOrThrow(Guid backlogItemId, Guid activityId)
        {
            var backlogItem = GetBacklogItemOrThrow(backlogItemId);
            return backlogItem.Activities.FirstOrDefault(activity => activity.Id == activityId)
                ?? throw new InvalidOperationException($"Activity with id {activityId} not found on backlog item {backlogItemId}.");
        }

        private bool PersistBacklogItem(Guid backlogItemId)
        {
            var backlogItem = GetBacklogItemOrThrow(backlogItemId);
            return _backlogItemRepository.Update(backlogItemId, backlogItem);
        }
    }
}