using Avans_DevOps.AvansDevOps.Domain.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Avans_DevOps.AvansDevOps.Domain.Entities
{
    public class SprintMember
    {
        public Guid Id { get; private set; }
        public User User { get; private set; }
        public SprintRole SprintRole { get; private set; }

        public SprintMember(Guid id, User user, SprintRole role)
        {
            if (id == Guid.Empty)
                throw new ArgumentException("Id cannot be empty.", nameof(id));

            User = user ?? throw new ArgumentNullException(nameof(user));
            SprintRole = role;
            Id = id;
        }
    }
}
