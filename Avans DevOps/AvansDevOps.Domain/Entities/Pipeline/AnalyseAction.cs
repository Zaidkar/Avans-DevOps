using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Avans_DevOps.AvansDevOps.Domain.Entities.Pipeline
{
    public class AnalyseAction : PipelineAction
    {
        public AnalyseAction(Guid id, string name, Dictionary<string, string>? settings = null)
            : base(id, name, settings)
        {
        }

        protected override PipelineExecutionResult Validate()
        {
            if (!HasSetting("AnalysisTool"))
                return PipelineExecutionResult.Failure("Analyse action requires an AnalysisTool setting.");

            return PipelineExecutionResult.Success();
        }

        protected override PipelineExecutionResult RunCore()
        {
            Console.WriteLine($"Running analysis for action: {Name}");
            return PipelineExecutionResult.Success();
        }

        protected override PipelineExecutionResult PublishResult()
        {
            Console.WriteLine($"Publishing analysis results for action: {Name}");
            return PipelineExecutionResult.Success();
        }
    }
}
