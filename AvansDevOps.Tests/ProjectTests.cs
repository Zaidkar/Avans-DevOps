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
        public void TC_01_FR_01_CreateProject_WithoutName_ThrowsArgumentException()
        {
            var user = CreateUser("Product Owner");
            var owner = CreateMember(user, SprintRole.ProductOwner);
            

            Assert.Throws<ArgumentException>(() =>
                new Project( "", "Demo project", owner));
        }

        [Fact]
        public void TC_01_FR_01_CreateProject_WithoutProductOwner_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() =>
                new Project("Avans DevOps", "Demo project", null!));
        }

        [Fact]
        public void TC_02_FR_02_ProductBacklog_AddRemoveReorder_WorksCorrectly()
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
    }
}