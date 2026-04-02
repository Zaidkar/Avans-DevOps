using System;
using Avans_DevOps.AvansDevOps.Domain.Entities;
using Avans_DevOps.AvansDevOps.Domain.Entities.Pipeline;
using Avans_DevOps.AvansDevOps.Domain.Enum;
using Xunit;

namespace AvansDevOps.Tests
{
    public class SprintReleaseTests
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

        private static PipelineDefinition CreatePipeline()
        {
            return new PipelineDefinition(Guid.NewGuid(), "Release Pipeline");
        }

        private static Sprint CreateSprintInReleasingState()
        {
            var sprint = CreateReleaseSprint();
            sprint.AssignPipeline(CreatePipeline());
            sprint.Start();
            sprint.Finish();
            sprint.BeginRelease();
            return sprint;
        }

        [Fact]
        public void TC_15_FR_12_WhenPipelineSucceeds_SprintBecomesReleased()
        {
            var sprint = CreateSprintInReleasingState();

            sprint.ReleaseSucceeded();

            Assert.Equal("Released", sprint.CurrentState);
        }

        [Fact]
        public void TC_16_FR_12_WhenPipelineFails_SprintBecomesReleaseFailed()
        {
            var sprint = CreateSprintInReleasingState();

            sprint.ReleaseFailed();

            Assert.Equal("ReleaseFailed", sprint.CurrentState);
        }

        [Fact]
        public void TC_16_FR_12_ReleaseFailed_SupportsRetryAndCancel()
        {
            var sprintForRetry = CreateSprintInReleasingState();
            sprintForRetry.ReleaseFailed();

            sprintForRetry.RetryRelease();

            Assert.Equal("Releasing", sprintForRetry.CurrentState);

            var sprintForCancel = CreateSprintInReleasingState();
            sprintForCancel.ReleaseFailed();

            sprintForCancel.CancelRelease();

            Assert.Equal("ReleaseCancelled", sprintForCancel.CurrentState);
        }

        [Fact]
        public void TC_16_FR_12_DuringReleasing_SprintCannotBeChanged()
        {
            var sprint = CreateSprintInReleasingState();

            Assert.Throws<InvalidOperationException>(() => sprint.Rename("New Name"));
            Assert.Throws<InvalidOperationException>(() => sprint.ChangePlanning(
                new DateOnly(2026, 1, 2),
                new DateOnly(2026, 1, 13)));
        }
    }
}   