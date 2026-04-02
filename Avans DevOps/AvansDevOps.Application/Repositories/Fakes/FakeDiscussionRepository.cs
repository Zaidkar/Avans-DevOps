using Avans_DevOps.AvansDevOps.Domain.Entities;

namespace Avans_DevOps.AvansDevOps.Application.Repositories.Fakes
{
    internal class FakeDiscussionRepository : IDiscussionRepository
    {
        private readonly Dictionary<Guid, DiscussionThread> _threads = [];

        public List<(Guid Id, DiscussionThread Thread)> GetAll()
        {
            return _threads.Select(pair => (pair.Key, pair.Value)).ToList();
        }

        public DiscussionThread? GetById(Guid id)
        {
            return _threads.TryGetValue(id, out var thread) ? thread : null;
        }

        public Guid Create(DiscussionThread thread)
        {
            var id = thread.Id;
            _threads[id] = thread;
            return id;
        }

        public bool Update(Guid id, DiscussionThread thread)
        {
            if (!_threads.ContainsKey(id))
            {
                return false;
            }

            _threads[id] = thread;
            return true;
        }

        public bool Delete(Guid id)
        {
            return _threads.Remove(id);
        }
    }
}
