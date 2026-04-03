using Avans_DevOps.AvansDevOps.Domain.Entities;
using Avans_DevOps.AvansDevOps.Domain.Enum;
using Avans_DevOps.AvansDevOps.Domain.Interfaces;

namespace Avans_DevOps.AvansDevOps.Application.Scm
{
    public class GitScmAdapter : IScmAdapter
    {
        public string ProviderName => "Git";

        public ScmReference CreateCommitReference(string commitHash, string? description = null)
        {
            if (string.IsNullOrWhiteSpace(commitHash))
                throw new ArgumentException("Commit hash cannot be empty.", nameof(commitHash));

            return new ScmReference(
                Guid.NewGuid(),
                ScmReferenceType.Commit,
                commitHash.Trim(),
                description,
                ProviderName);
        }

        public ScmReference CreateBranchReference(string branchName, string? description = null)
        {
            if (string.IsNullOrWhiteSpace(branchName))
                throw new ArgumentException("Branch name cannot be empty.", nameof(branchName));

            return new ScmReference(
                Guid.NewGuid(),
                ScmReferenceType.Branch,
                branchName.Trim(),
                description,
                ProviderName);
        }

        public string BuildCommitCommand(string message)
        {
            if (string.IsNullOrWhiteSpace(message))
                throw new ArgumentException("Commit message cannot be empty.", nameof(message));

            return $"git commit -m \"{message}\"";
        }

        public string BuildCreateBranchCommand(string branchName)
        {
            if (string.IsNullOrWhiteSpace(branchName))
                throw new ArgumentException("Branch name cannot be empty.", nameof(branchName));

            return $"git branch {branchName}";
        }

        public string BuildCheckoutBranchCommand(string branchName)
        {
            if (string.IsNullOrWhiteSpace(branchName))
                throw new ArgumentException("Branch name cannot be empty.", nameof(branchName));

            return $"git checkout {branchName}";
        }
    }
}