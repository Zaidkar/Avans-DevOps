using System;
using System.Linq;
using Avans_DevOps.AvansDevOps.Application.Notifications.Simple;
using Avans_DevOps.AvansDevOps.Domain.Entities;
using Avans_DevOps.AvansDevOps.Domain.Enum;
using Xunit;

namespace AvansDevOps.Tests
{
    public class ProjectTests
    {
        private static User CreateUser(string name)
        {
            return new User
            {
                Id = Guid.NewGuid(),
                Name = name,
                Email = $"{name.Replace(" ", "").ToLowerInvariant()}@avans.dev"
            };
        }

        private static SprintMember CreateMember(User user, SprintRole role) =>
            new(user, role);

        private static BacklogItem CreateBacklogItem(string title)
        {
            var eventManager = new EventManager();
            return new BacklogItem(Guid.NewGuid(), title, "Description", 3, eventManager);
        }

        private static Sprint CreateSprint(string name)
        {
            var eventManager = new EventManager();
            return new Sprint(
                Guid.NewGuid(),
                name,
                DateOnly.FromDateTime(DateTime.UtcNow),
                DateOnly.FromDateTime(DateTime.UtcNow.AddDays(14)),
                SprintGoalType.Release,
                eventManager);
        }

        [Fact]
        public void TC_01_FR_01_CreateProject_WithValidData_CreatesProject()
        {
            var owner = CreateUser("Product Owner");
            var ownerMember = CreateMember(owner, SprintRole.ProductOwner);

            var project = new Project("Avans DevOps", "Demo project", ownerMember);

            Assert.Equal("Avans DevOps", project.Name);
            Assert.Equal("Demo project", project.Description);
            Assert.Same(ownerMember, project.ProductOwner);
        }

        [Fact]
        public void TC_02_FR_01_CreateProject_WithoutName_ThrowsArgumentException()
        {
            var user = CreateUser("Product Owner");
            var owner = CreateMember(user, SprintRole.ProductOwner);

            Assert.Throws<ArgumentException>(() =>
                new Project("", "Demo project", owner));
        }

        [Fact]
        public void TC_03_FR_01_CreateProject_WithoutProductOwner_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() =>
                new Project("Avans DevOps", "Demo project", null!));
        }

        [Fact]
        public void TC_04_FR_02_ProductBacklog_AddRemoveReorder_WorksCorrectly()
        {
            var project = new Project(
                "Avans DevOps",
                "Demo project",
                CreateMember(CreateUser("Product Owner"), SprintRole.ProductOwner));

            var itemA = CreateBacklogItem("A");
            var itemB = CreateBacklogItem("B");

            project.AddBacklogItem(itemA);
            project.AddBacklogItem(itemB);
            project.MoveBacklogItem(itemB.Id, 0);
            project.RemoveBacklogItem(itemA.Id);

            Assert.Single(project.ProductBacklog);
            Assert.Equal(itemB.Id, project.ProductBacklog.First().Id);
        }

        [Fact]
        public void TC_05_FR_01_ProjectManagement_RenameDescriptionAndProductOwner_UpdateProject()
        {
            var originalOwner = CreateMember(CreateUser("Original Owner"), SprintRole.ProductOwner);
            var newOwner = CreateMember(CreateUser("New Owner"), SprintRole.ProductOwner);
            var project = new Project("Avans DevOps", "Demo project", originalOwner);

            project.Rename("Avans DevOps 2");
            project.ChangeDescription(null!);
            project.ChangeProductOwner(newOwner);

            Assert.Equal("Avans DevOps 2", project.Name);
            Assert.Equal(string.Empty, project.Description);
            Assert.Same(newOwner, project.ProductOwner);
        }

        [Fact]
        public void TC_06_FR_01_ProjectManagement_InvalidRenameAndOwnerChange_AreRejected()
        {
            var project = new Project(
                "Avans DevOps",
                "Demo project",
                CreateMember(CreateUser("Product Owner"), SprintRole.ProductOwner));

            Assert.Throws<ArgumentException>(() => project.Rename(" "));
            Assert.Throws<ArgumentNullException>(() => project.ChangeProductOwner(null!));
        }

        [Fact]
        public void TC_07_FR_02_ProductBacklog_DuplicateAndNullItems_AreRejected()
        {
            var project = new Project(
                "Avans DevOps",
                "Demo project",
                CreateMember(CreateUser("Product Owner"), SprintRole.ProductOwner));
            var item = CreateBacklogItem("Duplicate backlog item");

            project.AddBacklogItem(item);

            Assert.Throws<ArgumentNullException>(() => project.AddBacklogItem(null!));
            Assert.Throws<InvalidOperationException>(() => project.AddBacklogItem(item));
        }

        [Fact]
        public void TC_08_FR_02_ProductBacklog_RemoveMissingOrDoneItem_IsRejected()
        {
            var project = new Project(
                "Avans DevOps",
                "Demo project",
                CreateMember(CreateUser("Product Owner"), SprintRole.ProductOwner));
            var eventManager = new EventManager();
            var developer = CreateUser("Developer One");
            var doneItem = new BacklogItem(Guid.NewGuid(), "Done item", "Description", 3, eventManager);

            doneItem.AssignDeveloper(developer);
            doneItem.MarkReadyForTesting();
            doneItem.StartTesting();
            doneItem.MarkTested();
            doneItem.ApproveDone();
            project.AddBacklogItem(doneItem);

            Assert.Throws<ArgumentNullException>(() => project.RemoveBacklogItem(Guid.NewGuid()));
            Assert.Throws<InvalidOperationException>(() => project.RemoveBacklogItem(doneItem.Id));
        }

        [Fact]
        public void TC_09_FR_02_ProductBacklog_MoveInvalidItemOrIndex_IsRejected()
        {
            var project = new Project(
                "Avans DevOps",
                "Demo project",
                CreateMember(CreateUser("Product Owner"), SprintRole.ProductOwner));
            var item = CreateBacklogItem("Backlog item");

            project.AddBacklogItem(item);

            Assert.Throws<ArgumentOutOfRangeException>(() => project.MoveBacklogItem(item.Id, -1));
            Assert.Throws<ArgumentOutOfRangeException>(() => project.MoveBacklogItem(item.Id, 1));
            Assert.Throws<InvalidOperationException>(() => project.MoveBacklogItem(Guid.NewGuid(), 0));
        }

        [Fact]
        public void TC_10_FR_06_ProjectSprintManagement_AddAndRemoveSprint_WorksCorrectly()
        {
            var project = new Project(
                "Avans DevOps",
                "Demo project",
                CreateMember(CreateUser("Product Owner"), SprintRole.ProductOwner));
            var sprint = CreateSprint("Sprint 1");

            project.AddSprint(sprint);
            project.RemoveSprint(sprint.Id);

            Assert.Throws<InvalidOperationException>(() => project.RemoveSprint(sprint.Id));
        }

        [Fact]
        public void TC_11_FR_06_ProjectSprintManagement_DuplicateMissingOrNullSprint_IsRejected()
        {
            var project = new Project(
                "Avans DevOps",
                "Demo project",
                CreateMember(CreateUser("Product Owner"), SprintRole.ProductOwner));
            var sprint = CreateSprint("Sprint 1");

            project.AddSprint(sprint);

            Assert.Throws<ArgumentNullException>(() => project.AddSprint(null!));
            Assert.Throws<InvalidOperationException>(() => project.AddSprint(sprint));
            Assert.Throws<InvalidOperationException>(() => project.RemoveSprint(Guid.NewGuid()));
        }
    }
}
