using System;
using System.Collections.Generic;

namespace Avans_DevOps.AvansDevOps.Domain.Interfaces
{
    public interface IBacklogWorkItemComponent
    {
        Guid Id { get; }
        string Title { get; }
        bool IsDone();
        IReadOnlyCollection<IBacklogWorkItemComponent> Children { get; }
        void Accept(IBacklogWorkItemVisitor visitor);
    }
}