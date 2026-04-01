using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Avans_DevOps.AvansDevOps.Domain.Entities
{
    public class Project
    {
        public string Name { get; set; }
        public List<Project> Projects { get; set; } 
        public List<Sprint> Sprints { get; set; }
        public User ProductOwner { get; set; }




    }
}
