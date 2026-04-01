using Avans_DevOps.AvansDevOps.Domain.Entities;

namespace Avans_DevOps.AvansDevOps.Application.Repositories.Fakes
{
    public class FakeSprintRepository : ISprintRepository
    {
        private readonly List<Sprint> _sprints = [];
        private int _nextId = 1;

        public List<Sprint> GetAll()
        {
            return _sprints;
        }

        public Sprint? GetById(int id)
        {
            return _sprints.FirstOrDefault(sprint => sprint.Id == id);
        }

        public Sprint Create(Sprint sprint)
        {
            sprint.Id = _nextId++;
            _sprints.Add(sprint);
            return sprint;
        }

        public bool Update(int id, Sprint sprint)
        {
            var index = _sprints.FindIndex(existingSprint => existingSprint.Id == id);
            if (index < 0)
            {
                return false;
            }

            sprint.Id = id;
            _sprints[index] = sprint;
            return true;
        }

        public bool Delete(int id)
        {
            var sprint = GetById(id);
            return sprint != null && _sprints.Remove(sprint);
        }
    }
}
