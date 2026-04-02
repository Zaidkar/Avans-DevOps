using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Avans_DevOps.AvansDevOps.Domain.Entities.Pipeline
{
    public class PipelineDefinition : PipelineComponent
    {
        private readonly List<PipelineComponent> _components = new();

        public IReadOnlyCollection<PipelineComponent> Components => _components.AsReadOnly();

        public PipelineDefinition(Guid id, string name)
            : base(id, name)
        {
        }

        public override void Add(PipelineComponent component)
        {
            if (component is null)
                throw new ArgumentNullException(nameof(component));

            _components.Add(component);
        }

        public override void Remove(PipelineComponent component)
        {
            if (component is null)
                throw new ArgumentNullException(nameof(component));

            _components.Remove(component);
        }

        protected override PipelineExecutionResult ExecuteCore()
        {
            foreach (var component in _components)
            {
                var result = component.Execute();

                if (!result.Succeeded)
                    return result;
            }

            return PipelineExecutionResult.Success();
        }
    }
}
