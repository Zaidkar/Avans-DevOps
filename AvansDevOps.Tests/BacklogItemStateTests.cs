using Avans_DevOps.AvansDevOps.Domain.Entities;
using Xunit;

namespace AvansDevOps.Tests
{
    public class BacklogItemStateTests
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
            return new BacklogItem(Guid.NewGuid(), "Backlog item", "Beschrijving", 5);
        }

        private static Activity CreateActivity(string title)
        {
            return new Activity(Guid.NewGuid(), title, "Activity beschrijving");
        }

        [Fact]
        public void TC_09_BacklogItem_HappyFlow_TodoToDone()
        {
            var backlogItem = CreateBacklogItem();
            var developer = CreateUser("Dev One");
            var activity = CreateActivity("Implement API");

            backlogItem.AddActivity(activity);
            activity.StartWork();
            activity.MarkDone();

            Assert.Equal("Todo", backlogItem.CurrentState);

            backlogItem.AssignDeveloper(developer);
            Assert.Equal("Doing", backlogItem.CurrentState);

            backlogItem.MarkReadyForTesting();
            Assert.Equal("ReadyForTesting", backlogItem.CurrentState);

            backlogItem.StartTesting();
            Assert.Equal("Testing", backlogItem.CurrentState);

            backlogItem.MarkTested();
            Assert.Equal("Tested", backlogItem.CurrentState);

            backlogItem.ApproveDone();
            Assert.Equal("Done", backlogItem.CurrentState);
            Assert.Null(backlogItem.AssignedDeveloper);
        }

        [Fact]
        public void TC_08_BacklogItem_StartWorkWithoutDeveloper_IsRejected()
        {
            var backlogItem = CreateBacklogItem();

            Assert.Throws<InvalidOperationException>(() => backlogItem.StartWork());
            Assert.Equal("Todo", backlogItem.CurrentState);
        }

        [Fact]
        public void TC_04_BacklogItem_ApproveDone_WhenActivitiesNotDone_IsRejected()
        {
            var backlogItem = CreateBacklogItem();
            var developer = CreateUser("Dev One");
            var activity = CreateActivity("Implement API");

            backlogItem.AddActivity(activity);
            backlogItem.AssignDeveloper(developer);
            backlogItem.MarkReadyForTesting();
            backlogItem.StartTesting();
            backlogItem.MarkTested();

            Assert.Throws<InvalidOperationException>(() => backlogItem.ApproveDone());
            Assert.Equal("Tested", backlogItem.CurrentState);
        }

        [Fact]
        public void TC_10_BacklogItem_TestingReturnToTodo_UnassignsDeveloper()
        {
            var backlogItem = CreateBacklogItem();
            var developer = CreateUser("Dev One");

            backlogItem.AssignDeveloper(developer);
            backlogItem.MarkReadyForTesting();
            backlogItem.StartTesting();

            backlogItem.ReturnToTodo();

            Assert.Equal("Todo", backlogItem.CurrentState);
            Assert.Null(backlogItem.AssignedDeveloper);
            Assert.Equal(developer.Id, backlogItem.LastDeveloper?.Id);
        }

        [Fact]
        public void TC_11_BacklogItem_ReturnToReadyForTesting_OnlyFromTested()
        {
            var backlogItem = CreateBacklogItem();
            var developer = CreateUser("Dev One");

            backlogItem.AssignDeveloper(developer);
            backlogItem.MarkReadyForTesting();
            backlogItem.StartTesting();

            Assert.Throws<InvalidOperationException>(() => backlogItem.ReturnToReadyForTesting());

            backlogItem.MarkTested();
            backlogItem.ReturnToReadyForTesting();

            Assert.Equal("ReadyForTesting", backlogItem.CurrentState);
        }
    }
}