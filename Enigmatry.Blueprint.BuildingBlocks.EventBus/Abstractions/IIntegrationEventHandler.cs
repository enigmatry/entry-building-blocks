using System.Threading.Tasks;
using Enigmatry.Blueprint.BuildingBlocks.EventBus.Events;

namespace Enigmatry.Blueprint.BuildingBlocks.EventBus.Abstractions
{
    public interface IIntegrationEventHandler<in TIntegrationEvent> : IIntegrationEventHandler
        where TIntegrationEvent : IntegrationEvent
    {
        Task Handle(TIntegrationEvent @event);
    }

    public interface IIntegrationEventHandler
    {
    }
}
