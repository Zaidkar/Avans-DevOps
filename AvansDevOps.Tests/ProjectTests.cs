using System;
using Avans_DevOps.AvansDevOps.Domain.Entities;
using Xunit;

namespace AvansDevOps.Tests
{
    public class ProjectTests
    {
        [Fact]
        public void AddBacklogItem_WhenItemIsValid_AddsItemToProductBacklog()
        {
            // Arrange
            var productOwner = new User
            {
                Id = Guid.NewGuid(),
                Name = "Product Owner",
                Email = "po@avans.dev"
            };

            var project = new Project(
                Guid.NewGuid(),
                "Avans DevOps",
                "Demo project",
                productOwner);

            var backlogItem = new BacklogItem(
                Guid.NewGuid(),
                "Create login page",
                "As a user I want to log in",
                3);

            // Act
            project.AddBacklogItem(backlogItem);

            // Assert
            Assert.Single(project.ProductBacklog);
            Assert.Contains(backlogItem, project.ProductBacklog);
        }

        [Fact]
        public void Rename_WhenNameIsValid_UpdatesProjectName()
        {
            // Arrange
            var productOwner = new User
            {
                Id = Guid.NewGuid(),
                Name = "Product Owner",
                Email = "po@avans.dev"
            };

            var project = new Project(
                Guid.NewGuid(),
                "Old project name",
                "Demo project",
                productOwner);

            // Act
            project.Rename("New project name");

            // Assert
            Assert.Equal("New project name", project.Name);
        }

        [Fact]
        public void AddBacklogItem_WhenItemAlreadyExists_ThrowsInvalidOperationException()
        {
            // Arrange
            var productOwner = new User
            {
                Id = Guid.NewGuid(),
                Name = "Product Owner",
                Email = "po@avans.dev"
            };

            var project = new Project(
                Guid.NewGuid(),
                "Avans DevOps",
                "Demo project",
                productOwner);

            var backlogItem = new BacklogItem(
                Guid.NewGuid(),
                "Create login page",
                "As a user I want to log in",
                3);

            project.AddBacklogItem(backlogItem);

            // Act & Assert
            var exception = Assert.Throws<InvalidOperationException>(() => project.AddBacklogItem(backlogItem));
            Assert.Equal("This backlog item is already part of the product backlog.", exception.Message);
        }
    }
}