using Avans_DevOps.AvansDevOps.Domain.Entities;
using Avans_DevOps.AvansDevOps.Domain.Enum;

namespace Avans_DevOps.AvansDevOps.Application.Repositories
{
    public interface ISprintRepository
    {
        List<Sprint> GetAll();
        Sprint? GetById(Guid id);
        Sprint Create(Sprint sprint);
        bool Update(Guid id, Sprint sprint);
        bool Delete(Guid id);
        List<SprintMember> GetMembers(Guid sprintId);
        List<SprintMember> GetMembersByRole(Guid sprintId, SprintRole role);
        
    }
}
