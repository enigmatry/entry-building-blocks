using System;

namespace Enigmatry.BuildingBlocks.Tests.Core.Helpers
{
    [Serializable]
    internal class ObjectChildData
    {
#pragma warning disable CA5362
        public ObjectChildData? Child { get; set; }
#pragma warning restore CA5362
        public int Id { get; set; }
    }
}
