using System;
using System.Linq;
using Avans_DevOps.AvansDevOps.Application.Notifications.Simple;
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
            var eventManager = new EventManager();
            return new Sprint(
                Guid.NewGuid(),
                "Release Sprint",
                new DateOnly(2026, 3, 1),
                new DateOnly(2026, 3, 14),
                SprintGoalType.Release,
                eventManager);
        }

        private static Sprint CreateReviewSprint()
        {
            var eventManager = new EventManager();
            return new Sprint(
                Guid.NewGuid(),
                "Review Sprint",
                new DateOnly(2026, 4, 1),
                new DateOnly(2026, 4, 14),
                SprintGoalType.Review,
                eventManager);
        }

        private static PipelineDefinition CreatePipeline()
        {
            return new PipelineDefinition(Guid.NewGuid(), "Release Pipeline");
        }

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
        [Fact]
        public void TC_05_FR_06_FR_07_CreatedSprint_IsMutable()
        {
            var sprint = CreateReleaseSprint();
            var backlogItem = CreateBacklogItem();

            sprint.Rename("Sprint 1");
            sprint.ChangePlanning(new DateOnly(2026, 1, 2), new DateOnly(2026, 1, 15));
            sprint.AddMember(CreateUser("Dev One"), SprintRole.Developer);
            sprint.AddBacklogItem(backlogItem);
            sprint.AssignPipeline(CreatePipeline());

            Assert.Equal("Sprint 1", sprint.Name);
            Assert.Contains(backlogItem, sprint.BacklogItems);
            Assert.Single(sprint.Members);
            Assert.NotNull(sprint.Pipeline);
        }

        [Fact]
        public void TC_06_FR_06_FR_07_StartedSprint_IsNotMutable()
        {
            var sprint = CreateReleaseSprint();
            sprint.Start();

            Assert.Throws<InvalidOperationException>(() => sprint.Rename("New name"));
            Assert.Throws<InvalidOperationException>(() => sprint.AddBacklogItem(CreateBacklogItem()));
        }

        [Fact]
        public void TC_07_FR_08_AddSecondScrumMaster_IsRejected()
        {
            var sprint = CreateReleaseSprint();

            sprint.AddMember(CreateUser("Scrum Master 1"), SprintRole.ScrumMaster);

            Assert.Throws<InvalidOperationException>(() =>
                sprint.AddMember(CreateUser("Scrum Master 2"), SprintRole.ScrumMaster));
        }

        [Fact]
        public void TC_07_FR_08_AddDuplicateMember_IsRejected()
        {
            var sprint = CreateReleaseSprint();
            var userId = Guid.NewGuid();

            sprint.AddMember(
                new User { Id = userId, Name = "Dev", Email = "dev@avans.dev" },
                SprintRole.Developer);

            Assert.Throws<InvalidOperationException>(() =>
                sprint.AddMember(
                    new User { Id = userId, Name = "Dev Duplicate", Email = "dev2@avans.dev" },
                    SprintRole.Tester));
        }

        [Fact]
        public void TC_13_FR_11_ReviewSprint_CannotCloseWithoutReviewSummary()
        {
            var sprint = CreateReviewSprint();
            sprint.Start();
            sprint.Finish();

            Assert.Throws<InvalidOperationException>(() => sprint.CloseReview());
        }

        [Fact]
        public void TC_14_FR_11_FR_12_ReleaseSprint_CannotStartReleaseWithoutPipeline()
        {
            var sprint = CreateReleaseSprint();
            sprint.Start();
            sprint.Finish();

            Assert.Throws<InvalidOperationException>(() => sprint.BeginRelease());
        }
    }
}