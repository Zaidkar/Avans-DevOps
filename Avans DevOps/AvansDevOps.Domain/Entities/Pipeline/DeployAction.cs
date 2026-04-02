using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Avans_DevOps.AvansDevOps.Domain.Entities.Pipeline
{
    public class DeployAction : PipelineAction
    {
        public DeployAction(Guid id, string name, Dictionary<string, string>? settings = null)
            : base(id, name, settings)
        {
        }

        protected override PipelineExecutionResult Validate()
        {
            if (!HasSetting("TargetEnvironment"))
                return PipelineExecutionResult.Failure("Deploy action requires a TargetEnvironment setting.");

            return PipelineExecutionResult.Success();
        }

        protected override PipelineExecutionResult RunCore()
        {
            Console.WriteLine($"Running deployment for action: {Name}");
            return PipelineExecutionResult.Success();
        }
    }
}
