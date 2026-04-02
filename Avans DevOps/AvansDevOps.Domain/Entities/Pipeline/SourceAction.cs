using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Avans_DevOps.AvansDevOps.Domain.Entities.Pipeline
{
    public class SourceAction : PipelineAction
    {
        public SourceAction(Guid id, string name, Dictionary<string, string>? settings = null)
            : base(id, name, settings)
        {
        }

        protected override PipelineExecutionResult Validate()
        {
            if (!HasSetting("Repository"))
                return PipelineExecutionResult.Failure("Source action requires a Repository setting.");

            if (!HasSetting("Branch"))
                return PipelineExecutionResult.Failure("Source action requires a Branch setting.");

            return PipelineExecutionResult.Success();
        }

        protected override PipelineExecutionResult RunCore()
        {
            Console.WriteLine($"Running source retrieval for action: {Name}");
            return PipelineExecutionResult.Success();
        }
    }
}
