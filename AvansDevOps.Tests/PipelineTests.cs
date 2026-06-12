using Avans_DevOps.AvansDevOps.Domain.Entities.Pipeline;
using Xunit;

namespace AvansDevOps.Tests
{
    public class PipelineTests
    {
        [Fact]
        public void TC_19_PipelineAction_SourceAction_MissingRepository_ReturnsFailure()
        {
            var action = new SourceAction(
                Guid.NewGuid(),
                "Get Sources",
                new Dictionary<string, string>
                {
                    { "Branch", "main" }
                });

            var result = action.Execute();

            Assert.False(result.Succeeded);
            Assert.Equal("Source action requires a Repository setting.", result.ErrorMessage);
            Assert.True(action.HasExecuted);
        }

        [Fact]
        public void TC_19_PipelineAction_BuildAction_MissingConfiguration_ReturnsFailure()
        {
            var action = new BuildAction(
                Guid.NewGuid(),
                "Build Solution",
                new Dictionary<string, string>
                {
                    { "SolutionPath", "AvansDevOps.sln" }
                });

            var result = action.Execute();

            Assert.False(result.Succeeded);
            Assert.Equal("Build action requires a Configuration setting.", result.ErrorMessage);
            Assert.True(action.HasExecuted);
        }

        [Fact]
        public void TC_19_PipelineAction_DeployAction_WithRequiredSettings_ReturnsSuccess()
        {
            var action = new DeployAction(
                Guid.NewGuid(),
                "Deploy To Test",
                new Dictionary<string, string>
                {
                    { "TargetEnvironment", "Test" }
                });

            var result = action.Execute();

            Assert.True(result.Succeeded);
            Assert.Null(result.ErrorMessage);
            Assert.True(action.HasExecuted);
        }

        [Fact]
        public void TC_19_PipelineComponent_LeafAddRemove_AreRejected()
        {
            var leaf = new BuildAction(
                Guid.NewGuid(),
                "Build",
                new Dictionary<string, string>
                {
                    { "SolutionPath", "AvansDevOps.sln" },
                    { "Configuration", "Release" }
                });

            var child = new PipelineStage(Guid.NewGuid(), "Child Stage");

            Assert.Throws<InvalidOperationException>(() => leaf.Add(child));
            Assert.Throws<InvalidOperationException>(() => leaf.Remove(child));
        }

        [Fact]
        public void TC_19_PipelineComponent_Stage_StopsOnFirstFailure()
        {
            var stage = new PipelineStage(Guid.NewGuid(), "Build Stage");

            var failingAction = new BuildAction(
                Guid.NewGuid(),
                "Build",
                new Dictionary<string, string>
                {
                    { "SolutionPath", "AvansDevOps.sln" }
                });

            var nextAction = new PackageAction(
                Guid.NewGuid(),
                "Install Packages",
                new Dictionary<string, string>
                {
                    { "PackageManager", "NuGet" }
                });

            stage.Add(failingAction);
            stage.Add(nextAction);

            var result = stage.Execute();

            Assert.False(result.Succeeded);
            Assert.Equal("Build action requires a Configuration setting.", result.ErrorMessage);
            Assert.True(failingAction.HasExecuted);
            Assert.False(nextAction.HasExecuted);
            Assert.True(stage.HasExecuted);
        }

        [Fact]
        public void TC_19_PipelineComponent_Definition_WithNestedStages_ExecutesSuccessfully()
        {
            var pipeline = new PipelineDefinition(Guid.NewGuid(), "CI Pipeline");
            var sourceStage = new PipelineStage(Guid.NewGuid(), "Source");
            var buildStage = new PipelineStage(Guid.NewGuid(), "Build");

            sourceStage.Add(new SourceAction(
                Guid.NewGuid(),
                "Get Sources",
                new Dictionary<string, string>
                {
                    { "Repository", "AvansDevOpsRepo" },
                    { "Branch", "main" }
                }));

            buildStage.Add(new PackageAction(
                Guid.NewGuid(),
                "Install Packages",
                new Dictionary<string, string>
                {
                    { "PackageManager", "NuGet" }
                }));

            buildStage.Add(new BuildAction(
                Guid.NewGuid(),
                "Build Solution",
                new Dictionary<string, string>
                {
                    { "SolutionPath", "AvansDevOps.sln" },
                    { "Configuration", "Release" }
                }));

            pipeline.Add(sourceStage);
            pipeline.Add(buildStage);

            var result = pipeline.Execute();

            Assert.True(result.Succeeded);
            Assert.Null(result.ErrorMessage);
            Assert.True(sourceStage.HasExecuted);
            Assert.True(buildStage.HasExecuted);
            Assert.True(pipeline.HasExecuted);
        }
    }
}