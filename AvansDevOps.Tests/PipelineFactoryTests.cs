using Avans_DevOps.AvansDevOps.Application.Pipeline;
using Avans_DevOps.AvansDevOps.Domain.Entities.Pipeline;
using Xunit;

namespace AvansDevOps.Tests
{
    public class PipelineFactoryTests
    {
        [Fact]
        public void TC_19_PipelineFactory_CreateBuildValidationPipeline_BuildsExpectedStages()
        {
            var factory = new PipelineFactory();

            var pipeline = factory.CreateBuildValidationPipeline("Build Validation");

            Assert.Equal("Build Validation", pipeline.Name);
            Assert.Equal(3, pipeline.Components.Count);

            var sourceStage = Assert.IsType<PipelineStage>(pipeline.Components.ElementAt(0));
            var buildStage = Assert.IsType<PipelineStage>(pipeline.Components.ElementAt(1));
            var qualityStage = Assert.IsType<PipelineStage>(pipeline.Components.ElementAt(2));

            Assert.Equal("Source", sourceStage.Name);
            Assert.Equal("Build", buildStage.Name);
            Assert.Equal("Quality", qualityStage.Name);

            Assert.Single(sourceStage.Components);
            Assert.Equal(2, buildStage.Components.Count);
            Assert.Equal(2, qualityStage.Components.Count);

            Assert.IsType<SourceAction>(sourceStage.Components.ElementAt(0));
            Assert.IsType<PackageAction>(buildStage.Components.ElementAt(0));
            Assert.IsType<BuildAction>(buildStage.Components.ElementAt(1));
            Assert.IsType<TestAction>(qualityStage.Components.ElementAt(0));
            Assert.IsType<AnalyseAction>(qualityStage.Components.ElementAt(1));
        }

        [Fact]
        public void TC_19_PipelineFactory_CreateDeploymentPipeline_AddsDeployStage()
        {
            var factory = new PipelineFactory();

            var pipeline = factory.CreateDeploymentPipeline("Deployment");

            Assert.Equal("Deployment", pipeline.Name);
            Assert.Equal(4, pipeline.Components.Count);

            var deployStage = Assert.IsType<PipelineStage>(pipeline.Components.ElementAt(3));
            Assert.Equal("Deploy", deployStage.Name);
            Assert.Single(deployStage.Components);
            Assert.IsType<DeployAction>(deployStage.Components.ElementAt(0));
        }

        [Fact]
        public void TC_19_PipelineFactory_CreateBuildValidationPipeline_WithEmptyName_Throws()
        {
            var factory = new PipelineFactory();

            Assert.Throws<ArgumentException>(() => factory.CreateBuildValidationPipeline(""));
            Assert.Throws<ArgumentException>(() => factory.CreateBuildValidationPipeline("   "));
        }

        [Fact]
        public void TC_19_PipelineFactory_CreateDeploymentPipeline_ProducesExecutablePipeline()
        {
            var factory = new PipelineFactory();

            var pipeline = factory.CreateDeploymentPipeline("Release Pipeline");
            var result = pipeline.Execute();

            Assert.True(result.Succeeded);
            Assert.Null(result.ErrorMessage);
            Assert.True(pipeline.HasExecuted);
        }
    }
}