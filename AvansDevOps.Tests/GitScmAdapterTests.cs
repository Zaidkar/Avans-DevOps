using System;
using Avans_DevOps.AvansDevOps.Application.Scm;
using Avans_DevOps.AvansDevOps.Domain.Enum;
using Xunit;

namespace AvansDevOps.Tests
{
    public class GitScmAdapterTests
    {
        [Fact]
        public void TC_31_FR_19_GitScmAdapter_CreateCommitReference_Succeeds()
        {
            var adapter = new GitScmAdapter();

            var reference = adapter.CreateCommitReference("  abc123  ", "Initial commit");

            Assert.Equal("Git", adapter.ProviderName);
            Assert.Equal(ScmReferenceType.Commit, reference.Type);
            Assert.Equal("abc123", reference.Value);
            Assert.Equal("Initial commit", reference.Description);
            Assert.Equal("Git", reference.Provider);
            Assert.NotEqual(Guid.Empty, reference.Id);
        }

        [Fact]
        public void TC_32_FR_19_GitScmAdapter_CreateBranchReference_Succeeds()
        {
            var adapter = new GitScmAdapter();

            var reference = adapter.CreateBranchReference("  feature/login  ", "Login branch");

            Assert.Equal(ScmReferenceType.Branch, reference.Type);
            Assert.Equal("feature/login", reference.Value);
            Assert.Equal("Login branch", reference.Description);
            Assert.Equal("Git", reference.Provider);
            Assert.NotEqual(Guid.Empty, reference.Id);
        }

        [Fact]
        public void TC_33_FR_19_GitScmAdapter_BuildGitCommands_Succeeds()
        {
            var adapter = new GitScmAdapter();

            var commitCommand = adapter.BuildCommitCommand("Add login feature");
            var createBranchCommand = adapter.BuildCreateBranchCommand("feature/login");
            var checkoutCommand = adapter.BuildCheckoutBranchCommand("feature/login");

            Assert.Equal("git commit -m \"Add login feature\"", commitCommand);
            Assert.Equal("git branch feature/login", createBranchCommand);
            Assert.Equal("git checkout feature/login", checkoutCommand);
        }

        [Fact]
        public void TC_34_FR_19_GitScmAdapter_InvalidInput_IsRejected()
        {
            var adapter = new GitScmAdapter();

            Assert.Throws<ArgumentException>(() => adapter.CreateCommitReference(""));

            Assert.Throws<ArgumentException>(() => adapter.CreateBranchReference(""));

            Assert.Throws<ArgumentException>(() => adapter.BuildCommitCommand(""));

            Assert.Throws<ArgumentException>(() => adapter.BuildCreateBranchCommand(""));

            Assert.Throws<ArgumentException>(() => adapter.BuildCheckoutBranchCommand(""));
        }
    }
}
