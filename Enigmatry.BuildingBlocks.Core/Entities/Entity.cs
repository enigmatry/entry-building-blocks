using System;
using System.Collections.Generic;
using System.Linq;

namespace Enigmatry.BuildingBlocks.Core.Entities
{
    public abstract class Entity
    {
        // needs to be private so that EF does not map the field
        private readonly Dictionary<DomainEvent, DomainEvent> _domainEvents = new();

        public IEnumerable<DomainEvent> GetDomainEvents() => _domainEvents.Values;

        protected void AddDomainEvent(DomainEvent eventItem)
        {
            if (eventItem == null)
            {
                throw new ArgumentNullException(nameof(eventItem));
            }

            // prevents multiple events with the same data to be added
            // last event wins
            _domainEvents[eventItem] = eventItem;
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
}
