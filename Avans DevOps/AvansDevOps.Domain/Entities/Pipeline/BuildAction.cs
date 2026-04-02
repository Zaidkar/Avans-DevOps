using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Avans_DevOps.AvansDevOps.Domain.Entities.Pipeline
{
    public class BuildAction : PipelineAction
    {
        public BuildAction(Guid id, string name, Dictionary<string, string>? settings = null)
            : base(id, name, settings)
        {
        }

        protected override PipelineExecutionResult Validate()
        {
            if (!HasSetting("SolutionPath"))
                return PipelineExecutionResult.Failure("Build action requires a SolutionPath setting.");

            if (!HasSetting("Configuration"))
                return PipelineExecutionResult.Failure("Build action requires a Configuration setting.");

            return PipelineExecutionResult.Success();
        }

        protected override PipelineExecutionResult RunCore()
        {
            Console.WriteLine($"Running build for action: {Name}");
            return PipelineExecutionResult.Success();
        }
    }
}
