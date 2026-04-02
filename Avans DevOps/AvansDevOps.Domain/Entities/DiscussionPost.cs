using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Avans_DevOps.AvansDevOps.Domain.Entities
{
    public class DiscussionPost
    {
        public Guid Id { get; }
        public User Author { get; }
        public string Message { get; private set; }
        public DateTime CreatedAt { get; }

        public DiscussionPost(Guid id, User author, string message, DateTime createdAt)
        {
            if (id == Guid.Empty)
                throw new ArgumentException("Post id cannot be empty.", nameof(id));

            Author = author ?? throw new ArgumentNullException(nameof(author));

            if (string.IsNullOrWhiteSpace(message))
                throw new ArgumentException("Message cannot be empty.", nameof(message));

            Id = id;
            Message = message;
            CreatedAt = createdAt;
        }

        public void EditMessage(string message)
        {
            if (string.IsNullOrWhiteSpace(message))
                throw new ArgumentException("Message cannot be empty.", nameof(message));

            Message = message;
        }
    }
}
