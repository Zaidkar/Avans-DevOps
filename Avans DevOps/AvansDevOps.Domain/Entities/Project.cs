using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Avans_DevOps.AvansDevOps.Domain.Entities
{
    public class Project
    {
        public string name { get; set; }
        public List<Project> projects { get; set;
        public List<Sprint> sprints { get; set; };



    }
}
