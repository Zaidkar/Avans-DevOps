using Avans_DevOps.AvansDevOps.Domain.Entities;
using Avans_DevOps.AvansDevOps.Domain.Interfaces;

namespace Avans_DevOps.AvansDevOps.Application.Scm
{
    public class ScmLinkService
    {
        private readonly IScmAdapter _scmAdapter;

        public ScmLinkService(IScmAdapter scmAdapter)
        {
            _scmAdapter = scmAdapter ?? throw new ArgumentNullException(nameof(scmAdapter));
        }

        public void LinkCommitToBacklogItem(BacklogItem backlogItem, string commitHash, string? description = null)
        {
            if (backlogItem is null)
                throw new ArgumentNullException(nameof(backlogItem));

            backlogItem.AddScmReference(_scmAdapter.CreateCommitReference(commitHash, description));
        }

        public void LinkBranchToSprint(Sprint sprint, string branchName, string? description = null)
        {
            if (sprint is null)
                throw new ArgumentNullException(nameof(sprint));

            sprint.AddScmReference(_scmAdapter.CreateBranchReference(branchName, description));
        }

        public void LinkCommitToActivity(Activity activity, string commitHash, string? description = null)
        {
            if (activity is null)
                throw new ArgumentNullException(nameof(activity));

            activity.AddScmReference(_scmAdapter.CreateCommitReference(commitHash, description));
        }
    }
}