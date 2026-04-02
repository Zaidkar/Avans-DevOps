using System;
using System.Linq;
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

        private static SprintMember CreateMember(string name, SprintRole role)
        {
            return new SprintMember(
                Guid.NewGuid(),
                new User
                {
                    Id = Guid.NewGuid(),
                    Name = name,
                    Email = $"{name.Replace(" ", "").ToLowerInvariant()}@avans.dev"
                },
                role);
        }

        [Fact]
        public void TC_05_FR_06_FR_07_CreatedSprint_IsMutable()
        {
            var sprint = CreateReleaseSprint();
            var backlogItemId = Guid.NewGuid();

            sprint.Rename("Sprint 1");
            sprint.ChangePlanning(new DateOnly(2026, 1, 2), new DateOnly(2026, 1, 15));
            sprint.AddMember(CreateMember("Dev One", SprintRole.Developer));
            sprint.AddBacklogItem(backlogItemId);
            sprint.AssignPipeline(CreatePipeline());

            Assert.Equal("Sprint 1", sprint.Name);
            Assert.Contains(backlogItemId, sprint.BacklogItemIds);
            Assert.Single(sprint.Members);
            Assert.NotNull(sprint.Pipeline);
        }

        [Fact]
        public void TC_06_FR_06_FR_07_StartedSprint_IsNotMutable()
        {
            var sprint = CreateReleaseSprint();
            sprint.Start();

            Assert.Throws<InvalidOperationException>(() => sprint.Rename("New name"));
            Assert.Throws<InvalidOperationException>(() => sprint.AddBacklogItem(Guid.NewGuid()));
        }

        [Fact]
        public void TC_07_FR_08_AddSecondScrumMaster_IsRejected()
        {
            var sprint = CreateReleaseSprint();

            sprint.AddMember(CreateMember("Scrum Master 1", SprintRole.ScrumMaster));

            Assert.Throws<InvalidOperationException>(() =>
                sprint.AddMember(CreateMember("Scrum Master 2", SprintRole.ScrumMaster)));
        }

        [Fact]
        public void TC_07_FR_08_AddDuplicateMember_IsRejected()
        {
            var sprint = CreateReleaseSprint();
            var userId = Guid.NewGuid();

            sprint.AddMember(new SprintMember(
                Guid.NewGuid(),
                new User { Id = userId, Name = "Dev", Email = "dev@avans.dev" },
                SprintRole.Developer));

            Assert.Throws<InvalidOperationException>(() =>
                sprint.AddMember(new SprintMember(
                    Guid.NewGuid(),
                    new User { Id = userId, Name = "Dev Duplicate", Email = "dev2@avans.dev" },
                    SprintRole.Tester)));
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