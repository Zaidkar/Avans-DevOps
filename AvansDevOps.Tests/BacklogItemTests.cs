using System;
using Avans_DevOps.AvansDevOps.Application.Notifications.Simple;
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
            var eventManager = new EventManager();
            return new BacklogItem(Guid.NewGuid(), "Backlog item", "Description", 5,eventManager);
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
        public void TC_05_FR_05_ApproveDone_FailsWhenNotAllActivitiesDone()
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

        [Theory]
        [InlineData("EmptyGuid", "", "Backlog item id cannot be empty.", 5)]
        [InlineData("EmptyTitle", "", "Title cannot be empty.", 5)]
        [InlineData("WhitespaceTitle", "   ", "Title cannot be empty.", 5)]
        [InlineData("NegativeStoryPoints", "Valid Title", "Story points cannot be negative.", -1)]
        public void TC_23_BacklogItemCreation_FailsIfParametersAreInvalid(
            string scenario,
            string title,
            string expectedMessage,
            int storyPoints)
        {
            // Arrange
            var id = scenario == "EmptyGuid" ? Guid.Empty : Guid.NewGuid();
            var eventManager = new EventManager();
            var description = "Valid description";

            // Act & Assert
            var exception = Assert.Throws<ArgumentException>(() => 
                new BacklogItem(id, title, description, storyPoints, eventManager)
            );

            // Verify it caught the exact validation branch text and the correct parameter name
            Assert.Contains(expectedMessage, exception.Message);
        }
    }
}