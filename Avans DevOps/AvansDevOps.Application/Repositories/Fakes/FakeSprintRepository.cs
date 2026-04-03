using Avans_DevOps.AvansDevOps.Domain.Entities;
using Avans_DevOps.AvansDevOps.Domain.Enum;

namespace Avans_DevOps.AvansDevOps.Application.Repositories.Fakes
{
    public class FakeSprintRepository : ISprintRepository
    {
        private readonly List<Sprint> _sprints = [];

        public List<Sprint> GetAll()
        {
            return _sprints;
        }

        public Sprint? GetById(Guid id)
        {
            return _sprints.FirstOrDefault(sprint => sprint.Id == id);
        }

        public Sprint Create(Sprint sprint)
        {
            _sprints.Add(sprint);
            return sprint;
        }

        public bool Update(Guid id, Sprint sprint)
        {
            var index = _sprints.FindIndex(existingSprint => existingSprint.Id == id);
            if (index < 0)
            {
                return false;
            }
            _sprints[index] = sprint;
            return true;
        }

        public bool Delete(Guid id)
        {
            var sprint = GetById(id);
            return sprint != null && _sprints.Remove(sprint);
        }

        public List<SprintMember> GetMembers(Guid sprintId)
        {
            var sprint = GetById(sprintId);
            return sprint != null ? sprint.Members.ToList() : [];
        }

        public List<SprintMember> GetMembersByRole(Guid sprintId, SprintRole role)
        {
            var sprint = GetById(sprintId);
            if (sprint == null)
                return [];

            return sprint.Members
                .Where(member => member.SprintRole == role)
                .ToList();
        }

        
    }
}
