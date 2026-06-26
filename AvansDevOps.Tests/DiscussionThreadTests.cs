using System;
using System.Collections.Generic;
using Avans_DevOps.AvansDevOps.Application.Notifications.Simple;
using Avans_DevOps.AvansDevOps.Domain.Entities;
using Xunit;

namespace AvansDevOps.Tests
{
    public class DiscussionThreadTests
    {
        private sealed class RecordingEventListener : IEventListener
        {
            public List<NotificationEventData> ReceivedEvents { get; } = new();

            public void Update(NotificationEventData data)
            {
                ReceivedEvents.Add(data);
            }
        }

        private static User CreateUser(string name)
        {
            return new User
            {
                Id = Guid.NewGuid(),
                Name = name,
                Email = $"{name.Replace(" ", "").ToLowerInvariant()}@avans.dev"
            };
        }

        private static DiscussionPost CreatePost(string authorName, string message)
        {
            return new DiscussionPost(Guid.NewGuid(), CreateUser(authorName), message, DateTime.UtcNow);
        }

        [Fact]
        public void TC_42_FR_16_CreateDiscussionThread_WithValidData_SendsCreatedNotification()
        {
            var eventManager = new EventManager();
            var listener = new RecordingEventListener();
            eventManager.Subscribe(NotificationEventNames.DiscussionCreated, listener);
            var threadId = Guid.NewGuid();
            var backlogItemId = Guid.NewGuid();

            var thread = new DiscussionThread(threadId, backlogItemId, "Sprint refinement", eventManager);

            Assert.Equal(threadId, thread.Id);
            Assert.Equal(backlogItemId, thread.BacklogItemId);
            Assert.Equal("Sprint refinement", thread.Subject);
            Assert.False(thread.IsLocked);
            Assert.Empty(thread.Posts);
            var createdEvent = Assert.Single(listener.ReceivedEvents);
            Assert.Equal(NotificationEventNames.DiscussionCreated, createdEvent.EventType);
            Assert.Equal("Discussion created", createdEvent.Subject);
            Assert.Equal("Er is een nieuwe discussie gestart over Sprint refinement", createdEvent.Body);
        }

        [Fact]
        public void TC_43_FR_16_CreateDiscussionThread_WithInvalidData_IsRejected()
        {
            var eventManager = new EventManager();

            Assert.Throws<ArgumentException>(() => new DiscussionThread(Guid.Empty, Guid.NewGuid(), "Topic", eventManager));
            Assert.Throws<ArgumentException>(() => new DiscussionThread(Guid.NewGuid(), Guid.Empty, "Topic", eventManager));
            Assert.Throws<ArgumentException>(() => new DiscussionThread(Guid.NewGuid(), Guid.NewGuid(), " ", eventManager));
        }

        [Fact]
        public void TC_44_FR_16_DiscussionThread_ChangeSubject_UpdatesSubject()
        {
            var thread = new DiscussionThread(Guid.NewGuid(), Guid.NewGuid(), "Old subject", new EventManager());

            thread.ChangeSubject("New subject");

            Assert.Equal("New subject", thread.Subject);
        }

        [Fact]
        public void TC_45_FR_16_DiscussionThread_EmptySubjectChange_IsRejected()
        {
            var thread = new DiscussionThread(Guid.NewGuid(), Guid.NewGuid(), "Old subject", new EventManager());

            Assert.Throws<ArgumentException>(() => thread.ChangeSubject(string.Empty));
        }

        [Fact]
        public void TC_46_FR_17_DiscussionThread_AddPost_AddsPostAndSendsReplyNotification()
        {
            var eventManager = new EventManager();
            var listener = new RecordingEventListener();
            eventManager.Subscribe(NotificationEventNames.DiscussionReply, listener);
            var thread = new DiscussionThread(Guid.NewGuid(), Guid.NewGuid(), "Sprint planning", eventManager);
            var post = CreatePost("Developer One", "I have a question about this item.");

            thread.AddPost(post);

            var addedPost = Assert.Single(thread.Posts);
            Assert.Same(post, addedPost);
            var replyEvent = Assert.Single(listener.ReceivedEvents);
            Assert.Equal(NotificationEventNames.DiscussionReply, replyEvent.EventType);
            Assert.Equal("Discussion reply", replyEvent.Subject);
            Assert.Equal($"Er is een nieuwe reactie geplaatst over Sprint planning:{post.Author} zei: {post.Message}", replyEvent.Body);
        }

        [Fact]
        public void TC_47_FR_17_DiscussionThread_DuplicateOrNullPost_IsRejected()
        {
            var thread = new DiscussionThread(Guid.NewGuid(), Guid.NewGuid(), "Sprint planning", new EventManager());
            var post = CreatePost("Developer One", "Looks good.");
            thread.AddPost(post);

            Assert.Throws<ArgumentNullException>(() => thread.AddPost(null!));
            Assert.Throws<InvalidOperationException>(() => thread.AddPost(post));
        }

        [Fact]
        public void TC_48_FR_17_DiscussionThread_RemovePost_RemovesExistingPost()
        {
            var thread = new DiscussionThread(Guid.NewGuid(), Guid.NewGuid(), "Sprint planning", new EventManager());
            var post = CreatePost("Developer One", "Looks good.");
            thread.AddPost(post);

            thread.RemovePost(post.Id);

            Assert.Empty(thread.Posts);
        }

        [Fact]
        public void TC_49_FR_17_DiscussionThread_RemoveMissingPost_IsRejected()
        {
            var thread = new DiscussionThread(Guid.NewGuid(), Guid.NewGuid(), "Sprint planning", new EventManager());

            Assert.Throws<InvalidOperationException>(() => thread.RemovePost(Guid.NewGuid()));
        }

        [Fact]
        public void TC_50_FR_16_FR_17_DiscussionThread_Lock_PreventsFurtherChanges()
        {
            var thread = new DiscussionThread(Guid.NewGuid(), Guid.NewGuid(), "Sprint planning", new EventManager());
            var post = CreatePost("Developer One", "Blocked by dependency.");
            thread.Lock();

            Assert.True(thread.IsLocked);
            Assert.Throws<InvalidOperationException>(() => thread.ChangeSubject("Updated subject"));
            Assert.Throws<InvalidOperationException>(() => thread.AddPost(post));
            Assert.Throws<InvalidOperationException>(() => thread.RemovePost(Guid.NewGuid()));
        }

        [Fact]
        public void TC_51_FR_17_DiscussionThread_SendNotification_WithoutEventType_IsRejected()
        {
            var thread = new DiscussionThread(Guid.NewGuid(), Guid.NewGuid(), "Sprint planning", new EventManager());

            Assert.Throws<ArgumentException>(() => thread.SendNotification(string.Empty, new NotificationEventData
            {
                Subject = "Reply",
                Body = "Message"
            }));
        }
    }
}
