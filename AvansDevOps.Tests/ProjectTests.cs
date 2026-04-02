using System;
using System.Linq;
using Avans_DevOps.AvansDevOps.Domain.Entities;
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

        private static BacklogItem CreateBacklogItem(string title)
        {
            return new BacklogItem(Guid.NewGuid(), title, "Description", 3);
        }

        [Fact]
        public void TC_01_FR_01_CreateProject_WithValidData_CreatesProject()
        {
            var owner = CreateUser("Product Owner");

            var project = new Project(Guid.NewGuid(), "Avans DevOps", "Demo project", owner);

            Assert.Equal("Avans DevOps", project.Name);
            Assert.Equal("Demo project", project.Description);
            Assert.Same(owner, project.ProductOwner);
        }

        [Fact]
        public void TC_01_FR_01_CreateProject_WithoutName_ThrowsArgumentException()
        {
            var owner = CreateUser("Product Owner");

            Assert.Throws<ArgumentException>(() =>
                new Project(Guid.NewGuid(), "", "Demo project", owner));
        }

        [Fact]
        public void TC_01_FR_01_CreateProject_WithoutProductOwner_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() =>
                new Project(Guid.NewGuid(), "Avans DevOps", "Demo project", null!));
        }

        [Fact]
        public void TC_02_FR_02_ProductBacklog_AddRemoveReorder_WorksCorrectly()
        {
            var project = new Project(
                Guid.NewGuid(),
                "Avans DevOps",
                "Demo project",
                CreateUser("Product Owner"));

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