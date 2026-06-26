using System;
using Avans_DevOps.AvansDevOps.Application.Notifications.Simple;
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
            var eventManager = new EventManager();
            return new BacklogItem(Guid.NewGuid(), "Backlog item", "Beschrijving", 5, eventManager);
        }

        private static Activity CreateActivity(string title)
        {
            return new Activity(Guid.NewGuid(), title, "Activity beschrijving");
        }

        [Fact]
        public void TC_22_FR_09_FR_14_BacklogItem_TestingReturnToTodo_UnassignsDeveloper()
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
        public void TC_23_FR_09_FR_15_BacklogItem_ReturnToReadyForTesting_FromTested()
        {
            var backlogItem = CreateBacklogItem();
            var developer = CreateUser("Dev One");

            backlogItem.AssignDeveloper(developer);
            backlogItem.MarkReadyForTesting();
            backlogItem.StartTesting();

            

            backlogItem.MarkTested();
            backlogItem.ReturnToReadyForTesting();

            Assert.Equal("ReadyForTesting", backlogItem.CurrentState);
        }

        [Fact]
        public void TC_24_FR_09_BacklogItem_ReturnToReadyForTesting_FromOtherShouldFail()
        {
            var backlogItem = CreateBacklogItem();
            var developer = CreateUser("Dev One");
            backlogItem.AssignDeveloper(developer);
            backlogItem.MarkReadyForTesting();
            
            Assert.Throws<InvalidOperationException>(() => backlogItem.ReturnToReadyForTesting());
        }

        [Fact]
        public void TC_25_NFR_04_BacklogItem_ChangingInformation_FromToDo()
        {
            var backlogItem = CreateBacklogItem();
            backlogItem.ChangeDescription("Nieuwe beschrijving");
            backlogItem.ChangeStoryPoints(3);
            backlogItem.ChangeTitle("Nieuwe Titel");
            Assert.Equal("Nieuwe beschrijving", backlogItem.Description);
            Assert.Equal("Nieuwe Titel", backlogItem.Title);
            Assert.Equal(3, backlogItem.StoryPoints);
        }

        [Fact]
        public void TC_26_NFR_04_BacklogItem_ChangingInformation_FromDoingFails()
        {
            var backlogItem = CreateBacklogItem();
            var developer = CreateUser("Dev One");
            backlogItem.AssignDeveloper(developer);
            Assert.Throws<InvalidOperationException>(() => backlogItem.ChangeStoryPoints(3));
            Assert.Throws<InvalidOperationException>(() => backlogItem.ChangeTitle("Nieuwe Titel"));
            Assert.Throws<InvalidOperationException>(() => backlogItem.ChangeDescription("Nieuwe Titel"));
        }

        [Fact]
        public void TC_27_FR_04_AssignAndUnassignDeveloper_FromReadyForTesting_ShouldFail()
        {
            var backlogItem = CreateBacklogItem();
            var developer = CreateUser("Dev One");
            backlogItem.AssignDeveloper(developer);
            backlogItem.MarkReadyForTesting();
            Assert.Throws<InvalidOperationException>(() => backlogItem.AssignDeveloper(developer)); 
            Assert.Throws<InvalidOperationException>(() => backlogItem.UnassignDeveloper());
        }

        [Fact]
        public void TC_28_FR_03_AddingAndRemovingActivity_FromReadyForTesting_ShouldFail()
        {
            var backlogItem = CreateBacklogItem();
            var developer = CreateUser("Dev One");
            var activity = CreateActivity("Activity");
            backlogItem.AssignDeveloper(developer);
            backlogItem.MarkReadyForTesting();
            Assert.Throws<InvalidOperationException>(() => backlogItem.AddActivity(activity));
            Assert.Throws<InvalidOperationException>(() => backlogItem.RemoveActivity(activity.Id));
        }
        [Theory]
        [InlineData("Tested",          nameof(BacklogItem.StartWork))]
        [InlineData("ReadyForTesting", nameof(BacklogItem.MarkReadyForTesting))]
        [InlineData("Todo",            nameof(BacklogItem.StartTesting))]
        [InlineData("Todo",            nameof(BacklogItem.MarkTested))]
        [InlineData("Todo",            nameof(BacklogItem.ApproveDone))]
        [InlineData("Todo",            nameof(BacklogItem.ReturnToTodo))]
        public void TC_29_NFR_04_StateBaseFallbackExceptions_ShouldThrowInvalidOperationException(
            string stateSetup, 
            string actionToTrigger)
        {
            // Arrange - Safely advance state using standard domain transitions
            var backlogItem = CreateBacklogItem();
            var developer = CreateUser("Dev One");

            if (stateSetup != "Todo")
            {
                backlogItem.AssignDeveloper(developer); // Moves to Doing
            }
            if (stateSetup == "ReadyForTesting" || stateSetup == "Tested")
            {
                backlogItem.MarkReadyForTesting();
            }
            if (stateSetup == "Tested")
            {
                backlogItem.StartTesting();
                backlogItem.MarkTested();
            }

            // Act & Assert
            Assert.Throws<InvalidOperationException>(() =>
            {
                switch (actionToTrigger)
                {
                    case nameof(backlogItem.StartWork): 
                        backlogItem.StartWork(); 
                        break;
                    case nameof(backlogItem.MarkReadyForTesting): 
                        backlogItem.MarkReadyForTesting(); 
                        break;
                    case nameof(backlogItem.StartTesting): 
                        backlogItem.StartTesting(); 
                        break;
                    case nameof(backlogItem.MarkTested): 
                        backlogItem.MarkTested(); 
                        break;
                    case nameof(backlogItem.ApproveDone): 
                        backlogItem.ApproveDone(); 
                        break;
                    case nameof(backlogItem.ReturnToTodo): 
                        backlogItem.ReturnToTodo(); 
                        break;
                    default:
                        throw new ArgumentException($"Action {actionToTrigger} missing mapping.");
                }
            });
            
        }
        [Fact]
        public void TC_30_NFR_04_BacklogItem_ChangeTitle_WithEmptyTitle_ShouldThrowArgumentException()
        {
            // Arrange
            var backlogItem = CreateBacklogItem(); // Starts in Todo state by default

            // Act & Assert
            var exception = Assert.Throws<ArgumentException>(() => backlogItem.ChangeTitle(""));
            Assert.Contains("Title cannot be empty.", exception.Message);
        }

        [Fact]
        public void TC_31_NFR_04_BacklogItem_ChangeStoryPoints_WithNegativePoints_ShouldThrowArgumentException()
        {
            // Arrange
            var backlogItem = CreateBacklogItem(); // Starts in Todo state by default

            // Act & Assert
            var exception = Assert.Throws<ArgumentException>(() => backlogItem.ChangeStoryPoints(-5));
            Assert.Contains("Story points cannot be negative.", exception.Message);
        }

        [Fact]
        public void TC_32_NFR_04_BacklogItem_ChangeDescription_WithNull_ShouldFallbackToEmptyString()
        {
            // Arrange
            var backlogItem = CreateBacklogItem(); // Starts in Todo state by default

            // Act
            backlogItem.ChangeDescription(null);

            // Assert
            Assert.Equal(string.Empty, backlogItem.Description);
        }
        [Fact]
        public void TC_33_FR_04_BacklogItem_AssigningSecondDeveloper_WithoutActivities_ShouldThrowInvalidOperationException()
        {
            var backlogItem = CreateBacklogItem();
            var developer = CreateUser("Dev One");
            var developer2 = CreateUser("Dev Two");
            
            backlogItem.AssignDeveloper(developer);
            var exception = Assert.Throws<InvalidOperationException>(() => backlogItem.AssignDeveloper(developer2));
            Assert.Contains("Assigning multiple developers requires activities.", exception.Message);
        }

        [Fact]
        public void TC_34_FR_03_BacklogItem_RemovingActivity_WithoutActivities_ShouldThrowInvalidOperationException()
        {
            var backlogItem = CreateBacklogItem();
            var exception = Assert.Throws<InvalidOperationException>(() => backlogItem.RemoveActivity(Guid.NewGuid()));
            Assert.Contains("Activity not found on this backlog item.", exception.Message);
        }
    }
}