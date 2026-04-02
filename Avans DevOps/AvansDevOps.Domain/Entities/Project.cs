using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Avans_DevOps.AvansDevOps.Domain.Entities
{
    public class Project
    {
        private readonly List<BacklogItem> _productBacklog = new();
        private readonly List<Sprint> _sprints = new();

        public Guid Id { get; }
        public string Name { get; private set; }
        public string Description { get; private set; }
        public User ProductOwner { get; private set; }

        public IReadOnlyCollection<BacklogItem> ProductBacklog => _productBacklog.AsReadOnly();
        public IReadOnlyCollection<Sprint> Sprints => _sprints.AsReadOnly();

        public Project(Guid id, string name, string description, User productOwner)
        {
            if (id == Guid.Empty)
                throw new ArgumentException("Project id cannot be empty.", nameof(id));

            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Project name cannot be empty.", nameof(name));

            ProductOwner = productOwner ?? throw new ArgumentNullException(nameof(productOwner));

            Id = id;
            Name = name;
            Description = description ?? string.Empty;
        }

        public void Rename(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Project name cannot be empty.", nameof(name));

            Name = name;
        }

        public void ChangeDescription(string description)
        {
            Description = description ?? string.Empty;
        }

        public void ChangeProductOwner(User productOwner)
        {
            ProductOwner = productOwner ?? throw new ArgumentNullException(nameof(productOwner));
        }

        public void AddBacklogItem(BacklogItem backlogItem)
        {
            if (backlogItem is null)
                throw new ArgumentNullException(nameof(backlogItem));

            if (_productBacklog.Any(x => x.Id == backlogItem.Id))
                throw new InvalidOperationException("This backlog item is already part of the product backlog.");

            _productBacklog.Add(backlogItem);
        }

        public void RemoveBacklogItem(Guid backlogItemId)
        {
            var backlogItem = _productBacklog.SingleOrDefault(x => x.Id == backlogItemId);

            if (backlogItem is null)
                throw new InvalidOperationException("Backlog item not found in this project.");

            _productBacklog.Remove(backlogItem);
        }

        public void MoveBacklogItem(Guid backlogItemId, int newIndex)
        {
            if (newIndex < 0 || newIndex >= _productBacklog.Count)
                throw new ArgumentOutOfRangeException(nameof(newIndex), "New index is out of range.");

            var backlogItem = _productBacklog.SingleOrDefault(x => x.Id == backlogItemId);

            if (backlogItem is null)
                throw new InvalidOperationException("Backlog item not found in this project.");

            _productBacklog.Remove(backlogItem);
            _productBacklog.Insert(newIndex, backlogItem);
        }

        public void AddSprint(Sprint sprint)
        {
            if (sprint is null)
                throw new ArgumentNullException(nameof(sprint));

            if (_sprints.Any(x => x.Id == sprint.Id))
                throw new InvalidOperationException("This sprint is already part of the project.");

            _sprints.Add(sprint);
        }

        public void RemoveSprint(Guid sprintId)
        {
            var sprint = _sprints.SingleOrDefault(x => x.Id == sprintId);

            if (sprint is null)
                throw new InvalidOperationException("Sprint not found in this project.");

            _sprints.Remove(sprint);
        }
    }

}

