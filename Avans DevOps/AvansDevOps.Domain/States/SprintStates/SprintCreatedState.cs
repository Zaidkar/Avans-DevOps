using Avans_DevOps.AvansDevOps.Domain.Entities;
using Avans_DevOps.AvansDevOps.Domain.Entities.Pipeline;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Avans_DevOps.AvansDevOps.Domain.Enum;

namespace Avans_DevOps.AvansDevOps.Domain.States.SprintStates
{
    public class SprintCreatedState : SprintState
    {
        public override string Name => "Created";

        public override void Rename(Sprint sprint, string name)
        {
            sprint.SetName(name);
        }

        public override void ChangePlanning(Sprint sprint, DateOnly startDate, DateOnly endDate)
        {
            sprint.SetPlanning(startDate, endDate);
        }

        public override void AddMember(Sprint sprint, User user, SprintRole sprintRole)
        {
            sprint.AddMemberInternal(user,  sprintRole);
        }

        public override void RemoveMember(Sprint sprint, Guid userId)
        {
            sprint.RemoveMemberInternal(userId);
        }

        public override void AddBacklogItem(Sprint sprint, BacklogItem backlogItem)
        {
            sprint.AddBacklogItemInternal(backlogItem);
        }

        public override void RemoveBacklogItem(Sprint sprint,  BacklogItem backlogItem)
        {
            sprint.RemoveBacklogItemInternal(backlogItem);
        }

        public override void AssignPipeline(Sprint sprint, PipelineDefinition pipeline)
        {
            if (!sprint.IsReleaseSprint())
                throw new InvalidOperationException("Only release sprints can have a pipeline assigned.");

            sprint.AssignPipelineInternal(pipeline);
        }   

        public override void Start(Sprint sprint)
        {
            sprint.SetActiveState();
        }
    }
}
