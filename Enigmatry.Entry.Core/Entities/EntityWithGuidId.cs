using System;

namespace Enigmatry.Entry.Core.Entities
{
    public abstract class EntityWithGuidId : EntityWithTypedId<Guid>
    {
        protected EntityWithGuidId()
        {
            Id = SequentialGuidGenerator.Generate();
        }
    }
}
