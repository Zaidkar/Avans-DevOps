using Avans_DevOps.AvansDevOps.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Avans_DevOps.AvansDevOps.Domain.States.SprintStates
{
    public abstract class BacklogItemState
    {
        public abstract void readyForTesting(BacklogItem item);
        public abstract void testing(BacklogItem item);
        public abstract void tested(BacklogItem item);
        public abstract void done(BacklogItem item);
        public abstract void toDo(BacklogItem item);
    }
}
