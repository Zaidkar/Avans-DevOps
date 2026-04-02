using Avans_DevOps.AvansDevOps.Domain.Entities.Pipeline;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Avans_DevOps.AvansDevOps.Application.Pipeline
{
    public class PipelineFactory : IPipelineFactory
    {
        public PipelineDefinition CreateBuildValidationPipeline(string pipelineName)
        {
            if (string.IsNullOrWhiteSpace(pipelineName))
                throw new ArgumentException("Pipeline name cannot be empty.", nameof(pipelineName));

            var pipeline = new PipelineDefinition(Guid.NewGuid(), pipelineName);

            var sourceStage = new PipelineStage(Guid.NewGuid(), "Source");
            sourceStage.Add(new SourceAction(
                Guid.NewGuid(),
                "Get Sources",
                new Dictionary<string, string>
                {
                    { "Repository", "AvansDevOpsRepo" },
                    { "Branch", "main" }
                }));

            var buildStage = new PipelineStage(Guid.NewGuid(), "Build");
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

            var qualityStage = new PipelineStage(Guid.NewGuid(), "Quality");
            qualityStage.Add(new TestAction(
                Guid.NewGuid(),
                "Run Unit Tests",
                new Dictionary<string, string>
                {
                    { "TestProject", "AvansDevOps.Tests" },
                    { "TestTool", "NUnit" }
                }));
            qualityStage.Add(new AnalyseAction(
                Guid.NewGuid(),
                "Run Code Analysis",
                new Dictionary<string, string>
                {
                    { "AnalysisTool", "SonarQube" }
                }));

            pipeline.Add(sourceStage);
            pipeline.Add(buildStage);
            pipeline.Add(qualityStage);

            return pipeline;
        }

        public PipelineDefinition CreateDeploymentPipeline(string pipelineName)
        {
            if (string.IsNullOrWhiteSpace(pipelineName))
                throw new ArgumentException("Pipeline name cannot be empty.", nameof(pipelineName));

            var pipeline = new PipelineDefinition(Guid.NewGuid(), pipelineName);

            var sourceStage = new PipelineStage(Guid.NewGuid(), "Source");
            sourceStage.Add(new SourceAction(
                Guid.NewGuid(),
                "Get Sources",
                new Dictionary<string, string>
                {
                    { "Repository", "AvansDevOpsRepo" },
                    { "Branch", "main" }
                }));

            var buildStage = new PipelineStage(Guid.NewGuid(), "Build");
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

            var qualityStage = new PipelineStage(Guid.NewGuid(), "Quality");
            qualityStage.Add(new TestAction(
                Guid.NewGuid(),
                "Run Unit Tests",
                new Dictionary<string, string>
                {
                    { "TestProject", "AvansDevOps.Tests" },
                    { "TestTool", "NUnit" }
                }));
            qualityStage.Add(new AnalyseAction(
                Guid.NewGuid(),
                "Run Code Analysis",
                new Dictionary<string, string>
                {
                    { "AnalysisTool", "SonarQube" }
                }));

            var deployStage = new PipelineStage(Guid.NewGuid(), "Deploy");
            deployStage.Add(new DeployAction(
                Guid.NewGuid(),
                "Deploy To Environment",
                new Dictionary<string, string>
                {
                    { "TargetEnvironment", "Test" }
                }));

            pipeline.Add(sourceStage);
            pipeline.Add(buildStage);
            pipeline.Add(qualityStage);
            pipeline.Add(deployStage);

            return pipeline;
        }
    }
}
