using System.Threading.Tasks;

namespace Enigmatry.Blueprint.BuildingBlocks.EventBus.Abstractions
{
    public interface IDynamicIntegrationEventHandler
    {
        Task Handle(dynamic eventData);
    }
}
