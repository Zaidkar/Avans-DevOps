using Avans_DevOps.AvansDevOps.Domain.Entities;

namespace Avans_DevOps.AvansDevOps.Application.Repositories.Fakes
{
    public class FakeBacklogItemRepository : IBacklogItemRepository
    {
        private readonly Dictionary<int, BacklogItem> _items = [];
        private int _nextId = 1;

        public List<(int Id, BacklogItem Item)> GetAll()
        {
            return _items.Select(pair => (pair.Key, pair.Value)).ToList();
        }

        public BacklogItem? GetById(int id)
        {
            return _items.TryGetValue(id, out var item) ? item : null;
        }

        public int Create(BacklogItem item)
        {
            var id = _nextId++;
            _items[id] = item;
            return id;
        }

        public bool Update(int id, BacklogItem item)
        {
            if (!_items.ContainsKey(id))
            {
                return false;
            }

            _items[id] = item;
            return true;
        }

        public bool Delete(int id)
        {
            return _items.Remove(id);
        }
    }
}
