using System;
using Avans_DevOps.AvansDevOps.Domain.Entities;
using Xunit;

namespace AvansDevOps.Tests
{
    public class BacklogItemTests
    {
        private readonly User _developer = new()
        {
            Id = Guid.NewGuid(),
            Name = "Dev One",
            Email = "devone@avans.dev"
        };

        private readonly User _otherDeveloper = new()
        {
            Id = Guid.NewGuid(),
            Name = "Dev Two",
            Email = "devtwo@avans.dev"
        };

        private static BacklogItem CreateBacklogItem()
        {
            return new BacklogItem(
                Guid.NewGuid(),
                "Backlog item",
                "Description",
                5);
        }

        private static Activity CreateActivity(string title)
        {
            return new Activity(Guid.NewGuid(), title, "Activity description");
        }

        private static void MoveToTested(BacklogItem backlogItem, User developer)
        {
            backlogItem.AssignDeveloper(developer);
            backlogItem.MarkReadyForTesting();
            backlogItem.StartTesting();
            backlogItem.MarkTested();
        }

        private static void MoveToDone(BacklogItem backlogItem, User developer)
        {
            var activity = CreateActivity("Activity 1");
            backlogItem.AddActivity(activity);
            activity.StartWork();
            activity.MarkDone();

            MoveToTested(backlogItem, developer);
            backlogItem.ApproveDone();
        }

        [Fact]
        public void FR2_1_CreateBacklogItem_WithValidData_CreatesItem()
        {
            // Arrange
            var id = Guid.NewGuid();

            // Act
            var backlogItem = new BacklogItem(id, "Title", "Description", 3);

            // Assert
            Assert.Equal(id, backlogItem.Id);
            Assert.Equal("Title", backlogItem.Title);
            Assert.Equal("Description", backlogItem.Description);
            Assert.Equal(3, backlogItem.StoryPoints);
            Assert.Equal("Todo", backlogItem.CurrentState);
        }

        [Fact]
        public void FR2_1_ChangeTitle_WhenDoing_UpdatesTitle()
        {
            // Arrange
            var backlogItem = CreateBacklogItem();
            backlogItem.AssignDeveloper(_developer);

            // Act
            backlogItem.ChangeTitle("Updated title");

            // Assert
            Assert.Equal("Updated title", backlogItem.Title);
        }

        [Fact]
        public void FR2_1_ChangeDescription_WhenTested_UpdatesDescription()
        {
            // Arrange
            var backlogItem = CreateBacklogItem();
            MoveToTested(backlogItem, _developer);

            // Act
            backlogItem.ChangeDescription("Updated description");

            // Assert
            Assert.Equal("Updated description", backlogItem.Description);
        }

        [Fact]
        public void FR2_1_ChangeStoryPoints_WhenDone_ThrowsInvalidOperationException()
        {
            // Arrange
            var backlogItem = CreateBacklogItem();
            MoveToDone(backlogItem, _developer);

            // Act & Assert
            var exception = Assert.Throws<InvalidOperationException>(() => backlogItem.ChangeStoryPoints(8));
            Assert.Equal("Action 'ChangeStoryPoints' is not allowed in state 'Done'.", exception.Message);
        }

        [Fact]
        public void FR2_2_BacklogItemContainsRequiredFields_OnCreation()
        {
            // Arrange
            var backlogItem = CreateBacklogItem();

            // Assert
            Assert.False(string.IsNullOrWhiteSpace(backlogItem.Title));
            Assert.NotNull(backlogItem.Description);
            Assert.True(backlogItem.StoryPoints >= 0);
            Assert.Equal("Todo", backlogItem.CurrentState);
        }

        [Fact]
        public void FR2_2_AssignedDeveloperOptional_DefaultsToNull()
        {
            // Arrange
            var backlogItem = CreateBacklogItem();

            // Assert
            Assert.Null(backlogItem.AssignedDeveloper);
        }

        [Fact]
        public void FR2_2_SprintOptional_DefaultsToNull()
        {
            // Arrange
            var backlogItem = CreateBacklogItem();

            // Assert
            Assert.Null(backlogItem.SprintId);
        }

        [Fact]
        public void FR2_2_AssignAndRemoveSprint_SetsSprintId()
        {
            // Arrange
            var backlogItem = CreateBacklogItem();
            var sprintId = Guid.NewGuid();

            // Act
            backlogItem.AssignToSprint(sprintId);
            backlogItem.RemoveFromSprint();

            // Assert
            Assert.Null(backlogItem.SprintId);
        }

        [Fact]
        public void FR2_3_AddMultipleActivities_AddsAllActivities()
        {
            // Arrange
            var backlogItem = CreateBacklogItem();
            var activity1 = CreateActivity("Activity 1");
            var activity2 = CreateActivity("Activity 2");

            // Act
            backlogItem.AddActivity(activity1);
            backlogItem.AddActivity(activity2);

            // Assert
            Assert.Equal(2, backlogItem.Activities.Count);
            Assert.Contains(activity1, backlogItem.Activities);
            Assert.Contains(activity2, backlogItem.Activities);
        }

        [Fact]
        public void FR2_3_RemoveActivity_RemovesActivity()
        {
            // Arrange
            var backlogItem = CreateBacklogItem();
            var activity = CreateActivity("Activity 1");

            backlogItem.AddActivity(activity);

            // Act
            backlogItem.RemoveActivity(activity.Id);

            // Assert
            Assert.Empty(backlogItem.Activities);
        }

        [Fact]
        public void FR2_4_ApproveDone_WhenActivitiesNotDone_ThrowsInvalidOperationException()
        {
            // Arrange
            var backlogItem = CreateBacklogItem();
            backlogItem.AssignDeveloper(_developer);

            var activity = CreateActivity("Activity 1");
            backlogItem.AddActivity(activity);

            backlogItem.MarkReadyForTesting();
            backlogItem.StartTesting();
            backlogItem.MarkTested();

            // Act & Assert
            var exception = Assert.Throws<InvalidOperationException>(() => backlogItem.ApproveDone());
            Assert.Equal("A backlog item cannot be marked as done until all underlying activities are done.", exception.Message);
        }

        [Fact]
        public void FR2_4_ApproveDone_WhenAllActivitiesDone_SetsDone()
        {
            // Arrange
            var backlogItem = CreateBacklogItem();
            MoveToDone(backlogItem, _developer);

            // Assert
            Assert.Equal("Done", backlogItem.CurrentState);
        }

        [Fact]
        public void FR2_5_AssignDeveloper_SetsSingleDeveloper()
        {
            // Arrange
            var backlogItem = CreateBacklogItem();

            // Act
            backlogItem.AssignDeveloper(_developer);

            // Assert
            Assert.Same(_developer, backlogItem.AssignedDeveloper);
        }

        [Fact]
        public void FR2_6_AssignSecondDeveloperWithoutActivities_ThrowsInvalidOperationException()
        {
            // Arrange
            var backlogItem = CreateBacklogItem();
            backlogItem.AssignDeveloper(_developer);

            // Act & Assert
            var exception = Assert.Throws<InvalidOperationException>(() => backlogItem.AssignDeveloper(_otherDeveloper));
            Assert.Equal("Assigning multiple developers requires activities.", exception.Message);
        }

        [Fact]
        public void FR2_6_AssignSecondDeveloperWithActivities_AllowsReassignment()
        {
            // Arrange
            var backlogItem = CreateBacklogItem();
            backlogItem.AssignDeveloper(_developer);

            backlogItem.AddActivity(CreateActivity("Activity 1"));

            // Act
            backlogItem.AssignDeveloper(_otherDeveloper);

            // Assert
            Assert.Same(_otherDeveloper, backlogItem.AssignedDeveloper);
        }

        [Fact]
        public void FR3_1_StatusTransitions_FollowRequiredOrder()
        {
            // Arrange
            var backlogItem = CreateBacklogItem();
            backlogItem.AssignDeveloper(_developer);

            // Act
            backlogItem.MarkReadyForTesting();
            backlogItem.StartTesting();
            backlogItem.MarkTested();

            // Assert
            Assert.Equal("Tested", backlogItem.CurrentState);
        }

        [Fact]
        public void FR3_1_StartTesting_FromTodo_ThrowsInvalidOperationException()
        {
            // Arrange
            var backlogItem = CreateBacklogItem();

            // Act & Assert
            var exception = Assert.Throws<InvalidOperationException>(() => backlogItem.StartTesting());
            Assert.Equal("Action 'StartTesting' is not allowed in state 'Todo'.", exception.Message);
        }

        [Fact]
        public void FR3_3_TestFailure_ReturnToTodo_ResetsToTodoAndUnassignsDeveloper()
        {
            // Arrange
            var backlogItem = CreateBacklogItem();
            backlogItem.AssignDeveloper(_developer);
            backlogItem.MarkReadyForTesting();
            backlogItem.StartTesting();

            // Act
            backlogItem.ReturnToTodo();

            // Assert
            Assert.Equal("Todo", backlogItem.CurrentState);
            Assert.Null(backlogItem.AssignedDeveloper);
        }

        [Fact]
        public void FR3_4_RejectDefinitionOfDone_ReturnToReadyForTesting()
        {
            // Arrange
            var backlogItem = CreateBacklogItem();
            MoveToTested(backlogItem, _developer);

            // Act
            backlogItem.ReturnToReadyForTesting();

            // Assert
            Assert.Equal("ReadyForTesting", backlogItem.CurrentState);
        }

        [Fact]
        public void FR3_5_ReturnToTodo_FromDone_Allowed()
        {
            // Arrange
            var backlogItem = CreateBacklogItem();
            MoveToDone(backlogItem, _developer);

            // Act
            backlogItem.ReturnToTodo();

            // Assert
            Assert.Equal("Todo", backlogItem.CurrentState);
        }

        [Fact]
        public void FR3_6_ReadyForTesting_CannotGoBackToDoing()
        {
            // Arrange
            var backlogItem = CreateBacklogItem();
            backlogItem.AssignDeveloper(_developer);
            backlogItem.MarkReadyForTesting();

            // Act & Assert
            var exception = Assert.Throws<InvalidOperationException>(() => backlogItem.StartWork());
            Assert.Equal("Action 'StartWork' is not allowed in state 'ReadyForTesting'.", exception.Message);
        }
    }
}