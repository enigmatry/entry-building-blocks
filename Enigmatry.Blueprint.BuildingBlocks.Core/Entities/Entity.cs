using System;

namespace Enigmatry.Blueprint.BuildingBlocks.Core.Entities
{
    public abstract class Entity<TId> : EntityBase
    {
        public TId Id { get; set; } = default!;
    }

    public abstract class Entity : Entity<Guid>
    {
        protected Entity()
        {
            Id = SequentialGuidGenerator.Generate();
        }
    }
}
