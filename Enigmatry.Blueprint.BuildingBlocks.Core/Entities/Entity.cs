using System;

namespace Enigmatry.Blueprint.BuildingBlocks.Core.Entities
{
    public abstract class Entity<TId> : EntityBase where TId : struct
    {
        public TId Id { get; set; }
    }

    public abstract class Entity : Entity<Guid>
    {
        protected Entity()
        {
            Id = SequentialGuidGenerator.Generate();
        }
    }
}
