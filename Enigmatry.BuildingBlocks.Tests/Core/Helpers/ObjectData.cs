using System;
using System.Collections.Generic;
using System.Drawing;

namespace Enigmatry.BuildingBlocks.Tests.Core.Helpers
{
    [Serializable]
    internal class ObjectData
    {
        internal DateTime ExecutedOn { get; set; }
        internal IDictionary<string, int> People { get; set; } = new Dictionary<string, int>();
        internal Point Position { get; set; }
        internal ObjectChildData Children { get; set; } = new();
        internal int Id { get; set; }
        internal Guid Version { get; set; }
    }
}
