namespace Avans_DevOps.AvansDevOps.Application.Notifications.Simple;

public static class NotificationEventNames
{
    public const string SprintFinished = nameof(SprintFinished);
    public const string ReadyForTesting = nameof(ReadyForTesting);
    public const string TestFailure = nameof(TestFailure);
    public const string ReleaseSuccess = nameof(ReleaseSuccess);
    public const string ReleaseFailure = nameof(ReleaseFailure);
    public const string ReleaseCancelled = nameof(ReleaseCancelled);
    public const string DiscussionCreated = nameof(DiscussionCreated);
    public const string DiscussionReply = nameof(DiscussionReply);
}
