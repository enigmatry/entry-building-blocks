using MediatR;

namespace Enigmatry.Entry.Core.Entities
{
    public abstract record DomainEvent : INotification
    {
    }
}
