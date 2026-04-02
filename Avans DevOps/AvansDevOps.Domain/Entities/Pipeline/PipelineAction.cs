using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Avans_DevOps.AvansDevOps.Domain.Entities.Pipeline
{

    public abstract class PipelineAction : PipelineComponent
    {
        protected Dictionary<string, string> Settings { get; }

        protected PipelineAction(Guid id, string name, Dictionary<string, string>? settings = null)
            : base(id, name)
        {
            Settings = settings is null
                ? new Dictionary<string, string>()
                : new Dictionary<string, string>(settings);
        }

        protected sealed override PipelineExecutionResult ExecuteCore()
        {
            var validationResult = Validate();

            if (!validationResult.Succeeded)
                return validationResult;

            var preparationResult = Prepare();

            if (!preparationResult.Succeeded)
                return preparationResult;

            var coreResult = RunCore();

            if (!coreResult.Succeeded)
                return coreResult;

            return PublishResult();
        }

        protected virtual PipelineExecutionResult Validate()
        {
            return PipelineExecutionResult.Success();
        }

        protected virtual PipelineExecutionResult Prepare()
        {
            return PipelineExecutionResult.Success();
        }

        protected abstract PipelineExecutionResult RunCore();

        protected virtual PipelineExecutionResult PublishResult()
        {
            return PipelineExecutionResult.Success();
        }

        protected bool HasSetting(string key)
        {
            return Settings.ContainsKey(key);
        }

        protected string GetRequiredSetting(string key)
        {
            if (!Settings.TryGetValue(key, out var value) || string.IsNullOrWhiteSpace(value))
                throw new InvalidOperationException($"Required setting '{key}' is missing for action '{Name}'.");

            return value;
        }
    }
}
