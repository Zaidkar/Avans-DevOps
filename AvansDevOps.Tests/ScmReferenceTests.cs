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
                "GitHub");
        }

        private static ScmReference CreateBranchReference(string value)
        {
            return new ScmReference(
                Guid.NewGuid(),
                ScmReferenceType.Branch,
                value,
                "Branch reference",
                "GitHub");
        }

        [Fact]
        public void TC_18_ScmReference_BacklogItem_AddCommitAndBranch_Succeeds()
        {
            var backlogItem = new BacklogItem(Guid.NewGuid(), "BI-1", "Desc", 3);
            var commit = CreateCommitReference("a1b2c3d");
            var branch = CreateBranchReference("feature/login");

            backlogItem.AddScmReference(commit);
            backlogItem.AddScmReference(branch);

            Assert.Equal(2, backlogItem.ScmReferences.Count);
            Assert.Contains(backlogItem.ScmReferences, x => x.Id == commit.Id && x.Type == ScmReferenceType.Commit);
            Assert.Contains(backlogItem.ScmReferences, x => x.Id == branch.Id && x.Type == ScmReferenceType.Branch);
        }

        [Fact]
        public void TC_18_ScmReference_BacklogItem_AddDuplicateReference_IsRejected()
        {
            var backlogItem = new BacklogItem(Guid.NewGuid(), "BI-1", "Desc", 3);
            var commit = CreateCommitReference("a1b2c3d");

            backlogItem.AddScmReference(commit);

            Assert.Throws<InvalidOperationException>(() => backlogItem.AddScmReference(commit));
        }

        [Fact]
        public void TC_18_ScmReference_Activity_AddAndRemove_Succeeds()
        {
            var activity = new Activity(Guid.NewGuid(), "ACT-1", "Desc");
            var commit = CreateCommitReference("z9y8x7w");

            activity.AddScmReference(commit);
            Assert.Single(activity.ScmReferences);

            activity.RemoveScmReference(commit.Id);
            Assert.Empty(activity.ScmReferences);
        }
    }
}