using System;
using Avans_DevOps.AvansDevOps.Application.Notifications.Simple;
using Avans_DevOps.AvansDevOps.Domain.Entities;
using Avans_DevOps.AvansDevOps.Domain.Enum;
using Xunit;

namespace AvansDevOps.Tests
{
    public class ScmReferenceTests
    {
        private static ScmReference CreateCommitReference(string value)
        {
            return new ScmReference(
                Guid.NewGuid(),
                ScmReferenceType.Commit,
                value,
                "Commit reference",
                "GitHub"
            );
        }

        private static ScmReference CreateBranchReference(string value)
        {
            return new ScmReference(
                Guid.NewGuid(),
                ScmReferenceType.Branch,
                value,
                "Branch reference",
                "GitHub"
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

        [Fact]
        public void TC_87_FR_18_ScmReference_BacklogItem_AddCommitAndBranch_Succeeds()
        {
            var eventManager = new EventManager();
            var backlogItem = new BacklogItem(Guid.NewGuid(), "BI-1", "Desc", 3, eventManager);
            var commit = CreateCommitReference("a1b2c3d");
            var branch = CreateBranchReference("feature/login");

            backlogItem.AddScmReference(commit);
            backlogItem.AddScmReference(branch);

            Assert.Equal(2, backlogItem.ScmReferences.Count);
            Assert.Contains(
                backlogItem.ScmReferences,
                x => x.Id == commit.Id && x.Type == ScmReferenceType.Commit
            );
            Assert.Contains(
                backlogItem.ScmReferences,
                x => x.Id == branch.Id && x.Type == ScmReferenceType.Branch
            );
        }

        [Fact]
        public void TC_88_FR_18_ScmReference_BacklogItem_AddDuplicateReference_IsRejected()
        {
            var eventManager = new EventManager();
            var backlogItem = new BacklogItem(Guid.NewGuid(), "BI-1", "Desc", 3, eventManager);
            var commit = CreateCommitReference("a1b2c3d");

            backlogItem.AddScmReference(commit);

            Assert.Throws<InvalidOperationException>(() => backlogItem.AddScmReference(commit));
        }

        [Fact]
        public void TC_89_FR_18_ScmReference_Activity_AddAndRemove_Succeeds()
        {
            var activity = new Activity(Guid.NewGuid(), "ACT-1", "Desc");
            var commit = CreateCommitReference("z9y8x7w");

            activity.AddScmReference(commit);
            Assert.Single(activity.ScmReferences);

            activity.RemoveScmReference(commit.Id);
            Assert.Empty(activity.ScmReferences);
        }

        [Fact]
        public void TC_90_FR_18_ScmReference_WithValidData_IsCreated()
        {
            var id = Guid.NewGuid();

            var scmReference = new ScmReference(
                id,
                ScmReferenceType.Branch,
                "main",
                "Main branch",
                "GitHub"
            );

            Assert.Equal(id, scmReference.Id);
            Assert.Equal(ScmReferenceType.Branch, scmReference.Type);
            Assert.Equal("main", scmReference.Value);
            Assert.Equal("Main branch", scmReference.Description);
            Assert.Equal("GitHub", scmReference.Provider);
        }

        [Fact]
        public void TC_91_FR_18_ScmReference_InvalidData_IsRejected()
        {
            Assert.Throws<ArgumentException>(
                () => new ScmReference(Guid.Empty, ScmReferenceType.Commit, "abc123")
            );

            Assert.Throws<ArgumentException>(
                () => new ScmReference(Guid.NewGuid(), ScmReferenceType.Commit, "")
            );

            Assert.Throws<ArgumentException>(
                () =>
                    new ScmReference(
                        Guid.NewGuid(),
                        ScmReferenceType.Commit,
                        "abc123",
                        provider: ""
                    )
            );
        }

        [Fact]
        public void TC_92_FR_18_ScmReference_DescriptionCanBeChanged()
        {
            var scmReference = CreateScmReference();

            scmReference.ChangeDescription("Updated description");

            Assert.Equal("Updated description", scmReference.Description);

            scmReference.ChangeDescription(null);

            Assert.Null(scmReference.Description);
        }
    }
}
