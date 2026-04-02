using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Avans_DevOps.AvansDevOps.Domain.Entities.Pipeline
{
    public class TestAction : PipelineAction
    {
        public TestAction(Guid id, string name, Dictionary<string, string>? settings = null)
            : base(id, name, settings)
        {
        }

        protected override PipelineExecutionResult Validate()
        {
            if (!HasSetting("TestProject"))
                return PipelineExecutionResult.Failure("Test action requires a TestProject setting.");

            if (!HasSetting("TestTool"))
                return PipelineExecutionResult.Failure("Test action requires a TestTool setting.");

            return PipelineExecutionResult.Success();
        }

        protected override PipelineExecutionResult RunCore()
        {
            Console.WriteLine($"Running tests for action: {Name}");
            return PipelineExecutionResult.Success();
        }

        protected override PipelineExecutionResult PublishResult()
        {
            Console.WriteLine($"Publishing test results for action: {Name}");

            return PipelineExecutionResult.Success();
        }
    }
}

