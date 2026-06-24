using System;
using System.Linq;
using Avans_DevOps.AvansDevOps.Application.Notifications.Simple;
using Avans_DevOps.AvansDevOps.Domain.Entities;
using Avans_DevOps.AvansDevOps.Domain.Entities.Pipeline;
using Avans_DevOps.AvansDevOps.Domain.Enum;
using Newtonsoft.Json.Linq;
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
                eventManager
            );
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
                eventManager
            );
        }

        private static ScmReference CreateScmReference(Guid? id = null)
        {
            return new ScmReference(
                id ?? Guid.NewGuid(),
                ScmReferenceType.Commit,
                "abc123",
                "Test commit",
                "GitHub"
            );
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
                Email = $"{name.Replace(" ", "").ToLowerInvariant()}@avans.dev",
            };
        }

        private static BacklogItem CreateBacklogItem()
        {
            var eventManager = new EventManager();
            return new BacklogItem(Guid.NewGuid(), "Backlog item", "Description", 5, eventManager);
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
            Assert.Throws<InvalidOperationException>(
                () => sprint.AddBacklogItem(CreateBacklogItem())
            );
        }

        [Fact]
        public void TC_07_FR_08_AddSecondScrumMaster_IsRejected()
        {
            var sprint = CreateReleaseSprint();

            sprint.AddMember(CreateUser("Scrum Master 1"), SprintRole.ScrumMaster);

            Assert.Throws<InvalidOperationException>(
                () => sprint.AddMember(CreateUser("Scrum Master 2"), SprintRole.ScrumMaster)
            );
        }

        [Fact]
        public void TC_07_FR_08_AddDuplicateMember_IsRejected()
        {
            var sprint = CreateReleaseSprint();
            var userId = Guid.NewGuid();

            sprint.AddMember(
                new User
                {
                    Id = userId,
                    Name = "Dev",
                    Email = "dev@avans.dev",
                },
                SprintRole.Developer
            );

            Assert.Throws<InvalidOperationException>(
                () =>
                    sprint.AddMember(
                        new User
                        {
                            Id = userId,
                            Name = "Dev Duplicate",
                            Email = "dev2@avans.dev",
                        },
                        SprintRole.Tester
                    )
            );
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

        [Fact]
        public void TC_11_FR_06_SprintConstructor_InvalidData_IsRejected()
        {
            var eventManager = new EventManager();

            Assert.Throws<ArgumentException>(
                () =>
                    new Sprint(
                        Guid.Empty,
                        "Sprint",
                        new DateOnly(2026, 3, 1),
                        new DateOnly(2026, 3, 14),
                        SprintGoalType.Release,
                        eventManager
                    )
            );

            Assert.Throws<ArgumentException>(
                () =>
                    new Sprint(
                        Guid.NewGuid(),
                        "",
                        new DateOnly(2026, 3, 1),
                        new DateOnly(2026, 3, 14),
                        SprintGoalType.Release,
                        eventManager
                    )
            );

            Assert.Throws<ArgumentException>(
                () =>
                    new Sprint(
                        Guid.NewGuid(),
                        "Invalid Sprint",
                        new DateOnly(2026, 3, 14),
                        new DateOnly(2026, 3, 1),
                        SprintGoalType.Release,
                        eventManager
                    )
            );
        }

        [Fact]
        public void TC_12_FR_07_CreatedSprint_CanRemoveBacklogItem()
        {
            var sprint = CreateReleaseSprint();
            var backlogItem = CreateBacklogItem();

            sprint.AddBacklogItem(backlogItem);
            sprint.RemoveBacklogItem(backlogItem);

            Assert.DoesNotContain(backlogItem, sprint.BacklogItems);
        }

        [Fact]
        public void TC_13_FR_08_CreatedSprint_CanRemoveMember()
        {
            var sprint = CreateReleaseSprint();
            var user = CreateUser("Developer One");

            sprint.AddMember(user, SprintRole.Developer);
            sprint.RemoveMember(user.Id);

            Assert.Empty(sprint.Members);
        }

        [Fact]
        public void TC_14_FR_08_RemoveMember_InvalidUser_IsRejected()
        {
            var sprint = CreateReleaseSprint();

            Assert.Throws<ArgumentException>(() => sprint.RemoveMember(Guid.Empty));

            Assert.Throws<InvalidOperationException>(() => sprint.RemoveMember(Guid.NewGuid()));
        }

        [Fact]
        public void TC_15_FR_11_ReviewSprint_WithReviewSummary_CanBeClosed()
        {
            var sprint = CreateReviewSprint();

            sprint.Start();
            sprint.UploadReviewSummary("review-summary.pdf");
            sprint.Finish();
            sprint.CloseReview();

            Assert.Equal("review-summary.pdf", sprint.ReviewSummaryDocumentPath);
        }

        [Fact]
        public void TC_16_FR_11_ReviewSprint_EmptyReviewSummary_IsRejected()
        {
            var sprint = CreateReviewSprint();

            sprint.Start();

            Assert.Throws<ArgumentException>(() => sprint.UploadReviewSummary(""));
        }

        [Fact]
        public void TC_17_FR_12_ReleaseSprint_WithPipeline_CanReleaseSuccessfully()
        {
            var sprint = CreateReleaseSprint();

            sprint.AssignPipeline(CreatePipeline());
            sprint.Start();
            sprint.Finish();
            sprint.BeginRelease();

            var result = sprint.ExecuteReleasePipeline();

            sprint.ReleaseSucceeded();

            Assert.True(result);
        }

        [Fact]
        public void TC_18_FR_12_ReleaseSprint_WithoutPipelineExecutionResult_IsRejected()
        {
            var sprint = CreateReleaseSprint();

            Assert.Throws<InvalidOperationException>(() => sprint.ExecuteReleasePipeline());
        }

        [Fact]
        public void TC_19_FR_12_ReleaseSprint_FailedRelease_CanBeRetried()
        {
            var sprint = CreateReleaseSprint();

            sprint.AssignPipeline(CreatePipeline());
            sprint.Start();
            sprint.Finish();
            sprint.BeginRelease();

            sprint.ReleaseFailed();
            sprint.RetryRelease();

            Assert.Equal("Releasing", sprint.CurrentState);
        }

        [Fact]
        public void TC_20_FR_12_ReleaseSprint_FailedRelease_CanBeCancelled()
        {
            var sprint = CreateReleaseSprint();

            sprint.AssignPipeline(CreatePipeline());
            sprint.Start();
            sprint.Finish();
            sprint.BeginRelease();

            sprint.ReleaseFailed();
            sprint.CancelRelease();

            Assert.Equal("ReleaseCancelled", sprint.CurrentState);
        }

        [Fact]
        public void TC_21_FR_11_AssignPipeline_NullPipeline_IsRejected()
        {
            var sprint = CreateReleaseSprint();

            Assert.Throws<ArgumentNullException>(() => sprint.AssignPipeline(null!));
        }

        [Fact]
        public void TC_22_FR_18_CreatedSprint_CanAddAndRemoveScmReference()
        {
            var sprint = CreateReleaseSprint();
            var scmReference = CreateScmReference();

            sprint.AddScmReference(scmReference);
            sprint.RemoveScmReference(scmReference.Id);

            Assert.Empty(sprint.ScmReferences);
        }

        [Fact]
        public void TC_23_FR_18_DuplicateScmReference_IsRejected()
        {
            var sprint = CreateReleaseSprint();
            var id = Guid.NewGuid();

            sprint.AddScmReference(CreateScmReference(id));

            Assert.Throws<InvalidOperationException>(
                () => sprint.AddScmReference(CreateScmReference(id))
            );
        }

        [Fact]
        public void TC_24_FR_18_RemoveUnknownScmReference_IsRejected()
        {
            var sprint = CreateReleaseSprint();

            Assert.Throws<InvalidOperationException>(
                () => sprint.RemoveScmReference(Guid.NewGuid())
            );
        }

        [Fact]
        public void TC_25_FR_18_AddNullScmReference_IsRejected()
        {
            var sprint = CreateReleaseSprint();

            Assert.Throws<ArgumentNullException>(() => sprint.AddScmReference(null!));
        }
    }
}
