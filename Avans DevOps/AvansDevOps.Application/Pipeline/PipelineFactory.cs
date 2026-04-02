using Avans_DevOps.AvansDevOps.Domain.Entities.Pipeline;

namespace Avans_DevOps.AvansDevOps.Application.Pipeline
{
    public class PipelineFactory : IPipelineFactory
    {
        public PipelineDefinition CreateBuildValidationPipeline(string pipelineName)
        {
            var pipeline = CreatePipelineWithCommonStages(pipelineName);
            return pipeline;
        }

        public PipelineDefinition CreateDeploymentPipeline(string pipelineName)
        {
            var pipeline = CreatePipelineWithCommonStages(pipelineName);
            pipeline.Add(CreateDeployStage());
            return pipeline;
        }

        private static PipelineDefinition CreatePipelineWithCommonStages(string pipelineName)
        {
            if (string.IsNullOrWhiteSpace(pipelineName))
                throw new ArgumentException("Pipeline name cannot be empty.", nameof(pipelineName));

            var pipeline = new PipelineDefinition(Guid.NewGuid(), pipelineName);

            pipeline.Add(CreateSourceStage());
            pipeline.Add(CreateBuildStage());
            pipeline.Add(CreateQualityStage());

            return pipeline;
        }

        private static PipelineStage CreateSourceStage()
        {
            var stage = new PipelineStage(Guid.NewGuid(), "Source");
            stage.Add(new SourceAction(
                Guid.NewGuid(),
                "Get Sources",
                new Dictionary<string, string>
                {
                    { "Repository", "AvansDevOpsRepo" },
                    { "Branch", "main" }
                }));

            return stage;
        }

        private static PipelineStage CreateBuildStage()
        {
            var stage = new PipelineStage(Guid.NewGuid(), "Build");
            stage.Add(new PackageAction(
                Guid.NewGuid(),
                "Install Packages",
                new Dictionary<string, string>
                {
                    { "PackageManager", "NuGet" }
                }));
            stage.Add(new BuildAction(
                Guid.NewGuid(),
                "Build Solution",
                new Dictionary<string, string>
                {
                    { "SolutionPath", "AvansDevOps.sln" },
                    { "Configuration", "Release" }
                }));

            return stage;
        }

        private static PipelineStage CreateQualityStage()
        {
            var stage = new PipelineStage(Guid.NewGuid(), "Quality");
            stage.Add(new TestAction(
                Guid.NewGuid(),
                "Run Unit Tests",
                new Dictionary<string, string>
                {
                    { "TestProject", "AvansDevOps.Tests" },
                    { "TestTool", "NUnit" }
                }));
            stage.Add(new AnalyseAction(
                Guid.NewGuid(),
                "Run Code Analysis",
                new Dictionary<string, string>
                {
                    { "AnalysisTool", "SonarQube" }
                }));

            return stage;
        }

        private static PipelineStage CreateDeployStage()
        {
            var stage = new PipelineStage(Guid.NewGuid(), "Deploy");
            stage.Add(new DeployAction(
                Guid.NewGuid(),
                "Deploy To Environment",
                new Dictionary<string, string>
                {
                    { "TargetEnvironment", "Test" }
                }));

            return stage;
        }
    }
}
