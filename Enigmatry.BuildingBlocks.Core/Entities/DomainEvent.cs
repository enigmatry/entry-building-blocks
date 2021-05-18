using MediatR;

namespace Enigmatry.BuildingBlocks.Core.Entities
{
    public abstract record DomainEvent : INotification
    {
    }
}
