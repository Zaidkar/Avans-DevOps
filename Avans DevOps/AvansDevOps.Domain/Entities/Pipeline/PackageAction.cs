using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Avans_DevOps.AvansDevOps.Domain.Entities.Pipeline
{
    public class PackageAction : PipelineAction
    {
        public PackageAction(Guid id, string name, Dictionary<string, string>? settings = null)
            : base(id, name, settings)
        {
        }

        protected override PipelineExecutionResult Validate()
        {
            if (!HasSetting("PackageManager"))
                return PipelineExecutionResult.Failure("Package action requires a PackageManager setting.");

            return PipelineExecutionResult.Success();
        }

        protected override PipelineExecutionResult RunCore()
        {
            Console.WriteLine($"Running packaging for action: {Name}");
            return PipelineExecutionResult.Success();
        }
    }
}
