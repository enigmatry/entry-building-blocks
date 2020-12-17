using MediatR;

namespace Enigmatry.Blueprint.BuildingBlocks.Core.Entities
{
    public abstract record DomainEvent : INotification
    {
    }
}
