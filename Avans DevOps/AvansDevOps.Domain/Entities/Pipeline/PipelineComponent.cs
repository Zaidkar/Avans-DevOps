using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Avans_DevOps.AvansDevOps.Domain.Entities.Pipeline
{
    public abstract class PipelineComponent
    {
        public Guid Id { get; }
        public string Name { get; protected set; }
        public bool HasExecuted { get; private set; }

        protected PipelineComponent(Guid id, string name)
        {
            if (id == Guid.Empty)
                throw new ArgumentException("Pipeline component id cannot be empty.", nameof(id));

            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Pipeline component name cannot be empty.", nameof(name));

            Id = id;
            Name = name;
        }

        public PipelineExecutionResult Execute()
        {
            var result = ExecuteCore();

            HasExecuted = true;

            return result;
        }

        protected abstract PipelineExecutionResult ExecuteCore();

        public virtual void Add(PipelineComponent component)
        {
            throw new InvalidOperationException("This pipeline component does not support child components.");
        }

        public virtual void Remove(PipelineComponent component)
        {
            throw new InvalidOperationException("This pipeline component does not support child components.");
        }
    }
}

