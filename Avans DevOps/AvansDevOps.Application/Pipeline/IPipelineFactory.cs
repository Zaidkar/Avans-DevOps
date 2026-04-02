using Avans_DevOps.AvansDevOps.Domain.Entities.Pipeline;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Avans_DevOps.AvansDevOps.Application.Pipeline
{
    public interface IPipelineFactory
    {
        PipelineDefinition CreateBuildValidationPipeline(string pipelineName);
        PipelineDefinition CreateDeploymentPipeline(string pipelineName);
    }
}
