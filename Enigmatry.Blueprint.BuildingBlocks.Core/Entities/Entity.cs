using System.Collections.Generic;
using System.Linq;

namespace Enigmatry.Blueprint.BuildingBlocks.Core.Entities
{
    public abstract class Entity
    {
        // needs to be private so that EF does not map the field
        private readonly List<DomainEvent> _domainEvents = new();

        public IEnumerable<DomainEvent> GetDomainEvents() => _domainEvents;

        protected void AddDomainEvent(DomainEvent? eventItem)
        {
            if (eventItem == null)
            {
                return;
            }

            _domainEvents.Add(eventItem);
        }

        public void ClearDomainEvents() => _domainEvents.Clear();

        public void ClearDomainEvents<TEvent>() where TEvent : DomainEvent
        {
            var toRemove = _domainEvents.OfType<TEvent>().ToList();

            foreach (var item in toRemove)
            {
                _domainEvents.Remove(item);
            }
        }
    }

    public abstract class Entity<TId> : Entity
    {
        public TId Id { get; set; } = default!;
    }
}
