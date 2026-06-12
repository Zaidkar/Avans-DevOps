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
        public User User { get; private set; }
        public SprintRole SprintRole { get; private set; }

        public SprintMember(User user, SprintRole role)
        {
            User = user ?? throw new ArgumentNullException(nameof(user));
            SprintRole = role;
        }
    }
}
