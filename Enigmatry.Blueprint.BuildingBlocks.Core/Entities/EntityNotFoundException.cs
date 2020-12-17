using System;

namespace Enigmatry.Blueprint.BuildingBlocks.Core.Entities
{
    public class EntityNotFoundException : Exception
    {
        public EntityNotFoundException(string entityName) => EntityName = entityName;

        public EntityNotFoundException(string entityName, string message)
            : base(message) =>
            EntityName = entityName;

        public EntityNotFoundException(string entityName, string message, Exception inner)
            : base(message, inner) =>
            EntityName = entityName;

        public string EntityName { get; }
    }
}
