using Avans_DevOps.AvansDevOps.Domain.Entities;
using Avans_DevOps.AvansDevOps.Domain.Entities.Pipeline;
using Avans_DevOps.AvansDevOps.Domain.Enum;
using Xunit;

namespace AvansDevOps.Tests
{
    public class SprintStateTests
    {
        private static Sprint CreateReleaseSprint()
        {
            return new Sprint(
                Guid.NewGuid(),
                "Release Sprint",
                new DateOnly(2026, 3, 1),
                new DateOnly(2026, 3, 14),
                SprintGoalType.Release);
        }

        private static Sprint CreateReviewSprint()
        {
            return new Sprint(
                Guid.NewGuid(),
                "Review Sprint",
                new DateOnly(2026, 4, 1),
                new DateOnly(2026, 4, 14),
                SprintGoalType.Review);
        }

        private static PipelineDefinition CreatePipeline()
        {
            return new PipelineDefinition(Guid.NewGuid(), "Release Pipeline");
        }

        [Fact]
        public void TC_21_NFR_04_NFR_05_ReleaseSprint_HappyPath_CreatedToReleased()
        {
            var sprint = CreateReleaseSprint();

            Assert.Equal("Created", sprint.CurrentState);

            sprint.AssignPipeline(CreatePipeline());
            sprint.Start();
            Assert.Equal("Active", sprint.CurrentState);

            sprint.Finish();
            Assert.Equal("Finished", sprint.CurrentState);

            sprint.BeginRelease();
            Assert.Equal("Releasing", sprint.CurrentState);

            sprint.ReleaseSucceeded();
            Assert.Equal("Released", sprint.CurrentState);
        }

        [Fact]
        public void TC_21_NFR_04_NFR_05_ReleaseSprint_AlternativePath_ReleaseFailedRetryThenCancel()
        {
            var sprint = CreateReleaseSprint();

            sprint.AssignPipeline(CreatePipeline());
            sprint.Start();
            sprint.Finish();
            sprint.BeginRelease();

            sprint.ReleaseFailed();
            Assert.Equal("ReleaseFailed", sprint.CurrentState);

            sprint.RetryRelease();
            Assert.Equal("Releasing", sprint.CurrentState);

            sprint.ReleaseFailed();
            Assert.Equal("ReleaseFailed", sprint.CurrentState);

            sprint.CancelRelease();
            Assert.Equal("ReleaseCancelled", sprint.CurrentState);
        }

        [Fact]
        public void TC_21_NFR_04_NFR_05_ReviewSprint_HappyPath_CreatedToClosed()
        {
            var sprint = CreateReviewSprint();

            Assert.Equal("Created", sprint.CurrentState);

            sprint.Start();
            Assert.Equal("Active", sprint.CurrentState);

            sprint.UploadReviewSummary("review-summary.pdf");
            sprint.Finish();
            Assert.Equal("Finished", sprint.CurrentState);

            sprint.CloseReview();
            Assert.Equal("Closed", sprint.CurrentState);
        }

        [Fact]
        public void TC_21_NFR_04_NFR_05_ReviewSprint_UnhappyPaths_AreRejected()
        {
            var sprint = CreateReviewSprint();

            sprint.Start();
            sprint.Finish();

            Assert.Throws<InvalidOperationException>(() => sprint.BeginRelease());
            Assert.Throws<InvalidOperationException>(() => sprint.CloseReview());
        }
    }
}