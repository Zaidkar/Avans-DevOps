using System;
using Avans_DevOps.AvansDevOps.Domain.Entities;
using Xunit;

namespace AvansDevOps.Tests
{
    public class ProjectTests
    {
        private static Project CreateProject()
        {
            return new Project(
                Guid.NewGuid(),
                "Avans DevOps",
                "Demo project",
                CreateUser("Product Owner"));
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

        private static BacklogItem CreateBacklogItem()
        {
            return new BacklogItem(
                Guid.NewGuid(),
                "Backlog item",
                "Description",
                5);
        }

        private static Sprint CreateSprint()
        {
            return new Sprint(
                Guid.NewGuid(),
                "Sprint 1",
                new DateOnly(2026, 1, 1),
                new DateOnly(2026, 1, 14),
                Avans_DevOps.AvansDevOps.Domain.Enum.SprintGoalType.Review);
        }

        private static BacklogItem CreateDoneBacklogItem()
        {
            var backlogItem = CreateBacklogItem();
            var developer = CreateUser("Developer");

            backlogItem.AssignDeveloper(developer);

            var activity = new Activity(Guid.NewGuid(), "Activity 1", "Description");
            backlogItem.AddActivity(activity);
            activity.StartWork();
            activity.MarkDone();

            backlogItem.MarkReadyForTesting();
            backlogItem.StartTesting();
            backlogItem.MarkTested();
            backlogItem.ApproveDone();

            return backlogItem;
        }
        [Fact]
        public void FR1_1_CreateProject_WithValidData_CreatesProject()
        {
            // Arrange
            var productOwner = CreateUser("Product Owner");

            // Act
            var project = new Project(
                Guid.NewGuid(),
                "Avans DevOps",
                "Demo project",
                productOwner);

            // Assert
            Assert.Equal("Avans DevOps", project.Name);
            Assert.Equal("Demo project", project.Description);
            Assert.Same(productOwner, project.ProductOwner);
        }

        [Fact]
        public void FR1_1_RenameProject_WithValidName_UpdatesName()
        {
            // Arrange
            var project = CreateProject();

            // Act
            project.Rename("New project name");

            // Assert
            Assert.Equal("New project name", project.Name);
        }

        [Fact]
        public void FR1_1_ChangeDescription_WithNull_SetsEmptyDescription()
        {
            // Arrange
            var project = CreateProject();

            // Act
            project.ChangeDescription(null);

            // Assert
            Assert.Equal(string.Empty, project.Description);
        }

        [Fact]
        public void FR1_2_ProjectHasProductBacklog_WhenBacklogItemAdded_ItemIsPresent()
        {
            // Arrange
            var project = CreateProject();
            var backlogItem = CreateBacklogItem();

            // Act
            project.AddBacklogItem(backlogItem);

            // Assert
            Assert.Single(project.ProductBacklog);
            Assert.Contains(backlogItem, project.ProductBacklog);
        }

        [Fact]
        public void FR1_2_ProjectHasProductBacklog_WhenBacklogItemRemoved_ItemIsRemoved()
        {
            // Arrange
            var project = CreateProject();
            var backlogItem = CreateBacklogItem();

            project.AddBacklogItem(backlogItem);

            // Act
            project.RemoveBacklogItem(backlogItem.Id);

            // Assert
            Assert.Empty(project.ProductBacklog);
        }

        [Fact]
        public void FR1_2_ProjectHasSprints_WhenSprintAdded_SprintIsPresent()
        {
            // Arrange
            var project = CreateProject();
            var sprint = CreateSprint();

            // Act
            project.AddSprint(sprint);

            // Assert
            Assert.Single(project.Sprints);
            Assert.Contains(sprint, project.Sprints);
        }

        [Fact]
        public void FR1_2_ProjectHasSprints_WhenSprintRemoved_SprintIsRemoved()
        {
            // Arrange
            var project = CreateProject();
            var sprint = CreateSprint();

            project.AddSprint(sprint);

            // Act
            project.RemoveSprint(sprint.Id);

            // Assert
            Assert.Empty(project.Sprints);
        }

        [Fact]
        public void FR1_3_ProjectHasExactlyOneProductOwner_WhenProductOwnerIsNull_ThrowsArgumentNullException()
        {
            // Arrange
            var id = Guid.NewGuid();

            // Act & Assert
            var exception = Assert.Throws<ArgumentNullException>(() =>
                new Project(id, "Avans DevOps", "Demo project", null!));

            Assert.Equal("productOwner", exception.ParamName);
        }

        [Fact]
        public void FR1_3_ProjectHasExactlyOneProductOwner_WhenChanged_ReplacesOwner()
        {
            // Arrange
            var project = CreateProject();
            var newProductOwner = CreateUser("New Product Owner");

            // Act
            project.ChangeProductOwner(newProductOwner);

            // Assert
            Assert.Same(newProductOwner, project.ProductOwner);
        }

        [Fact]
        public void FR2_1_RemoveBacklogItem_WhenDone_ThrowsInvalidOperationException()
        {
            // Arrange
            var project = CreateProject();
            var backlogItem = CreateDoneBacklogItem();

            project.AddBacklogItem(backlogItem);

            // Act & Assert
            var exception = Assert.Throws<InvalidOperationException>(() => project.RemoveBacklogItem(backlogItem.Id));
            Assert.Equal("Backlog item is done and cannot be removed.", exception.Message);
        }

    }
}