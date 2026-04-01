using Avans_DevOps.AvansDevOps.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Avans_DevOps.AvansDevOps.Domain.States.SprintStates
{
    public abstract class SprintState
    {
        public abstract void start(Sprint sprint);
        public abstract void finish(Sprint sprint);
        public abstract void release(Sprint sprint);
    }
}
