using Avans_DevOps.AvansDevOps.Domain.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Avans_DevOps.AvansDevOps.Domain.Entities
{
    public class ScmReference
    {
        public Guid Id { get; }
        public ScmReferenceType Type { get; }
        public string Value { get; private set; }
        public string Provider { get; }
        public string? Description { get; private set; }

        public ScmReference(
            Guid id,
            ScmReferenceType type,
            string value,
            string? description = null,
            string provider = "Generic")
        {
            if (id == Guid.Empty)
                throw new ArgumentException("SCM reference id cannot be empty.", nameof(id));

            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("SCM reference value cannot be empty.", nameof(value));

            if (string.IsNullOrWhiteSpace(provider))
                throw new ArgumentException("SCM provider cannot be empty.", nameof(provider));

            Id = id;
            Type = type;
            Value = value;
            Description = description;
            Provider = provider;
        }

        public void ChangeDescription(string? description)
        {
            Description = description;
        }
    }
}
