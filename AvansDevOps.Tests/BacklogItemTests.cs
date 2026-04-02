using System;
using Avans_DevOps.AvansDevOps.Domain.Entities;
using Xunit;

namespace AvansDevOps.Tests
{
    public class BacklogItemTests
    {
        private static User CreateUser(string name)
        {
            return new User
            {
                Id = Guid.NewGuid(),
                Name = name,
                Email = $"{name.Replace(" ", "").ToLowerInvariant()}@avans.dev"
            };
        }

        private static BacklogItem CreateBacklogItem()
        {
            return new BacklogItem(Guid.NewGuid(), "Backlog item", "Description", 5);
        }

        private static Activity CreateActivity(Guid id, string title)
        {
            return new Activity(id, title, "Activity description");
        }

        [Fact]
        public void TC_03_FR_03_FR_04_AddActivityAndAssignDeveloper_DuplicateActivityIdIsRejected()
        {
            var backlogItem = CreateBacklogItem();
            var activityId = Guid.NewGuid();

            var activity = CreateActivity(activityId, "Activity 1");
            activity.AssignDeveloper(CreateUser("Dev Activity"));

            backlogItem.AddActivity(activity);

            Assert.Throws<InvalidOperationException>(() =>
                backlogItem.AddActivity(CreateActivity(activityId, "Activity 1 duplicate")));
        }

        [Fact]
        public void TC_03_FR_04_AssignSecondDeveloperWithoutActivities_IsRejected()
        {
            var backlogItem = CreateBacklogItem();
            var dev1 = CreateUser("Dev One");
            var dev2 = CreateUser("Dev Two");

            backlogItem.AssignDeveloper(dev1);

            Assert.Throws<InvalidOperationException>(() => backlogItem.AssignDeveloper(dev2));
        }

        [Fact]
        public void TC_04_FR_05_ApproveDone_FailsWhenNotAllActivitiesDone()
        {
            var backlogItem = CreateBacklogItem();
            var dev = CreateUser("Dev One");
            var activity = CreateActivity(Guid.NewGuid(), "Activity 1");

            backlogItem.AddActivity(activity);
            backlogItem.AssignDeveloper(dev);
            backlogItem.MarkReadyForTesting();
            backlogItem.StartTesting();
            backlogItem.MarkTested();

            Assert.Throws<InvalidOperationException>(() => backlogItem.ApproveDone());
        }

        [Fact]
        public void TC_08_FR_09_FR_10_StartWork_WithoutDeveloper_IsRejected()
        {
            var backlogItem = CreateBacklogItem();

            Assert.Throws<InvalidOperationException>(() => backlogItem.StartWork());
        }

        [Fact]
        public void TC_09_FR_09_HappyFlow_TodoToDone_Succeeds()
        {
            var backlogItem = CreateBacklogItem();
            var dev = CreateUser("Dev One");
            var activity = CreateActivity(Guid.NewGuid(), "Activity 1");

            backlogItem.AddActivity(activity);
            activity.StartWork();
            activity.MarkDone();

            backlogItem.AssignDeveloper(dev);
            backlogItem.MarkReadyForTesting();
            backlogItem.StartTesting();
            backlogItem.MarkTested();
            backlogItem.ApproveDone();

            Assert.Equal("Done", backlogItem.CurrentState);
        }

        [Fact]
        public void TC_11_FR_09_ReturnToReadyForTesting_OnlyFromTested()
        {
            var backlogItem = CreateBacklogItem();
            var dev = CreateUser("Dev One");

            backlogItem.AssignDeveloper(dev);
            backlogItem.MarkReadyForTesting();
            backlogItem.StartTesting();

            Assert.Throws<InvalidOperationException>(() => backlogItem.ReturnToReadyForTesting());

            backlogItem.MarkTested();
            backlogItem.ReturnToReadyForTesting();

            Assert.Equal("ReadyForTesting", backlogItem.CurrentState);
        }
    }
}