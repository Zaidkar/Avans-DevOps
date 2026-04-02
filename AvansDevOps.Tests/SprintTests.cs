using System;
using Avans_DevOps.AvansDevOps.Domain.Entities;
using Avans_DevOps.AvansDevOps.Domain.Entities.Pipeline;
using Avans_DevOps.AvansDevOps.Domain.Enum;
using Xunit;

namespace AvansDevOps.Tests
{
    public class SprintTests
    {
        private static Sprint CreateReleaseSprint()
        {
            return new Sprint(
                Guid.NewGuid(),
                "Release Sprint",
                new DateOnly(2026, 1, 1),
                new DateOnly(2026, 1, 14),
                SprintGoalType.Release);
        }

        private static Sprint CreateReviewSprint()
        {
            return new Sprint(
                Guid.NewGuid(),
                "Review Sprint",
                new DateOnly(2026, 2, 1),
                new DateOnly(2026, 2, 14),
                SprintGoalType.Review);
        }

        private static PipelineDefinition CreatePipeline()
        {
            return new PipelineDefinition(Guid.NewGuid(), "Release Pipeline");
        }

        private static SprintMember CreateMember(Guid userId, SprintRole role)
        {
            var user = new User
            {
                Id = userId,
                Name = "User",
                Email = "user@avans.dev"
            };

            return new SprintMember(Guid.NewGuid(), user, role);
        }

        [Fact]
        public void Start_WhenInCreated_MovesToActive()
        {
            // Arrange
            var sprint = CreateReleaseSprint();

            // Act
            sprint.Start();

            // Assert
            Assert.Equal("Active", sprint.CurrentState);
        }

        [Fact]
        public void AddBacklogItem_WhenActive_ThrowsInvalidOperationException()
        {
            // Arrange
            var sprint = CreateReleaseSprint();
            sprint.Start();

            var backlogItemId = Guid.NewGuid();

            // Act & Assert
            var exception = Assert.Throws<InvalidOperationException>(() => sprint.AddBacklogItem(backlogItemId));
            Assert.Equal("Action 'AddBacklogItem' is not allowed in state 'Active'.", exception.Message);
        }

        [Fact]
        public void Finish_WhenActive_MovesToFinished()
        {
            // Arrange
            var sprint = CreateReleaseSprint();
            sprint.Start();

            // Act
            sprint.Finish();

            // Assert
            Assert.Equal("Finished", sprint.CurrentState);
        }

        [Fact]
        public void BeginRelease_WhenReleaseSprintWithoutPipeline_ThrowsInvalidOperationException()
        {
            // Arrange
            var sprint = CreateReleaseSprint();
            sprint.Start();
            sprint.Finish();

            // Act & Assert
            var exception = Assert.Throws<InvalidOperationException>(() => sprint.BeginRelease());
            Assert.Equal("A release sprint must have a pipeline assigned.", exception.Message);
        }

        [Fact]
        public void BeginRelease_WhenReleaseSprintWithPipeline_MovesToReleasing()
        {
            // Arrange
            var sprint = CreateReleaseSprint();
            sprint.AssignPipeline(CreatePipeline());
            sprint.Start();
            sprint.Finish();

            // Act
            sprint.BeginRelease();

            // Assert
            Assert.Equal("Releasing", sprint.CurrentState);
        }

        [Fact]
        public void CloseReview_WhenReviewSprintWithSummary_MovesToClosed()
        {
            // Arrange
            var sprint = CreateReviewSprint();
            sprint.Start();
            sprint.UploadReviewSummary("review-summary.pdf");
            sprint.Finish();

            // Act
            sprint.CloseReview();

            // Assert
            Assert.Equal("Closed", sprint.CurrentState);
        }

        [Fact]
        public void Constructor_WhenEndDateBeforeStartDate_ThrowsArgumentException()
        {
            // Arrange
            var startDate = new DateOnly(2026, 1, 10);
            var endDate = new DateOnly(2026, 1, 9);

            // Act & Assert
            var exception = Assert.Throws<ArgumentException>(() =>
                new Sprint(Guid.NewGuid(), "Sprint", startDate, endDate, SprintGoalType.Release));

            Assert.Equal("End date cannot be before start date.", exception.Message);
        }

        [Fact]
        public void ChangePlanning_WhenEndDateBeforeStartDate_ThrowsArgumentException()
        {
            // Arrange
            var sprint = CreateReleaseSprint();
            var startDate = new DateOnly(2026, 1, 10);
            var endDate = new DateOnly(2026, 1, 9);

            // Act & Assert
            var exception = Assert.Throws<ArgumentException>(() => sprint.ChangePlanning(startDate, endDate));
            Assert.Equal("End date cannot be before start date.", exception.Message);
        }

        [Fact]
        public void AddMember_WhenDuplicateUser_ThrowsInvalidOperationException()
        {
            // Arrange
            var sprint = CreateReleaseSprint();
            var userId = Guid.NewGuid();

            sprint.AddMember(CreateMember(userId, SprintRole.Developer));

            // Act & Assert
            var exception = Assert.Throws<InvalidOperationException>(() =>
                sprint.AddMember(CreateMember(userId, SprintRole.ScrumMaster)));

            Assert.Equal("User is already a member of the sprint.", exception.Message);
        }

        [Fact]
        public void RemoveMember_WhenUserNotMember_ThrowsInvalidOperationException()
        {
            // Arrange
            var sprint = CreateReleaseSprint();
            var userId = Guid.NewGuid();

            // Act & Assert
            var exception = Assert.Throws<InvalidOperationException>(() => sprint.RemoveMember(userId));
            Assert.Equal("User is not a member of the sprint.", exception.Message);
        }
    }
}