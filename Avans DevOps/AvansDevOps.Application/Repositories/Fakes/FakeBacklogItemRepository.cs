using Avans_DevOps.AvansDevOps.Domain.Entities;

namespace Avans_DevOps.AvansDevOps.Application.Repositories.Fakes
{
    public class FakeBacklogItemRepository : IBacklogItemRepository
    {
        private readonly ISprintRepository _sprintRepository;
        private readonly Dictionary<Guid, BacklogItem> _items = [];

        public FakeBacklogItemRepository(ISprintRepository sprintRepository)
        {
            _sprintRepository = sprintRepository;
        }
        

        public List<(Guid Id, BacklogItem Item)> GetAll()
        {
            return _items.Select(pair => (pair.Key, pair.Value)).ToList();
        }

        public BacklogItem? GetById(Guid id)
        {
            return _items.TryGetValue(id, out var item) ? item : null;
        }

        public Guid Create(BacklogItem item)
        {
            var id = item.Id;
            _items[id] = item;
            return id;
        }

        public bool Update(Guid id, BacklogItem item)
        {
            if (!_items.ContainsKey(id))
            {
                return false;
            }

            _items[id] = item;
            return true;
        }

        public bool Delete(Guid id)
        {
            return _items.Remove(id);
        }

        public Sprint? GetSprintForBacklogItem(Guid backlogItemId)
        {
            return _sprintRepository.GetAll()
                .FirstOrDefault(sprint => sprint.BacklogItemIds.Contains(backlogItemId));
        }
    }
}
