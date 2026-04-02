using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Avans_DevOps.AvansDevOps.Domain.Entities
{
    public class DiscussionThread
    {
        private readonly List<DiscussionPost> _posts = new();

        public Guid Id { get; }
        public Guid BacklogItemId { get; }
        public string Subject { get; private set; }
        public bool IsLocked { get; private set; }

        public IReadOnlyCollection<DiscussionPost> Posts => _posts.AsReadOnly();

        public DiscussionThread(Guid id, Guid backlogItemId, string subject)
        {
            if (id == Guid.Empty)
                throw new ArgumentException("Thread id cannot be empty.", nameof(id));

            if (backlogItemId == Guid.Empty)
                throw new ArgumentException("Backlog item id cannot be empty.", nameof(backlogItemId));

            if (string.IsNullOrWhiteSpace(subject))
                throw new ArgumentException("Subject cannot be empty.", nameof(subject));

            Id = id;
            BacklogItemId = backlogItemId;
            Subject = subject;
        }

        public void ChangeSubject(string subject)
        {
            EnsureUnlocked();

            if (string.IsNullOrWhiteSpace(subject))
                throw new ArgumentException("Subject cannot be empty.", nameof(subject));

            Subject = subject;
        }

        public void AddPost(DiscussionPost post)
        {
            EnsureUnlocked();

            if (post is null)
                throw new ArgumentNullException(nameof(post));

            if (_posts.Any(x => x.Id == post.Id))
                throw new InvalidOperationException("This post is already part of the thread.");

            _posts.Add(post);
        }

        public void RemovePost(Guid postId)
        {
            EnsureUnlocked();

            var post = _posts.SingleOrDefault(x => x.Id == postId);

            if (post is null)
                throw new InvalidOperationException("Post not found in this thread.");

            _posts.Remove(post);
        }

        public void Lock()
        {
            IsLocked = true;
        }

        private void EnsureUnlocked()
        {
            if (IsLocked)
                throw new InvalidOperationException("This discussion thread is locked.");
        }
    }
}
