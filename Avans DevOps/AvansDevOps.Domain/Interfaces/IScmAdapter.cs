using Avans_DevOps.AvansDevOps.Domain.Entities;

namespace Avans_DevOps.AvansDevOps.Domain.Interfaces
{
    public interface IScmAdapter
    {
        string ProviderName { get; }

        ScmReference CreateCommitReference(string commitHash, string? description = null);
        ScmReference CreateBranchReference(string branchName, string? description = null);

        string BuildCommitCommand(string message);
        string BuildCreateBranchCommand(string branchName);
        string BuildCheckoutBranchCommand(string branchName);
    }
}