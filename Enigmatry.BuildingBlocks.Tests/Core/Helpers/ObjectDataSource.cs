using System;
using System.Collections.Generic;
using System.Drawing;

namespace Enigmatry.BuildingBlocks.Tests.Core.Helpers
{
    internal static class ObjectDataSource
    {
        public static IEnumerable<object> Get()
        {
            yield return new object();
            yield return new ObjectData
            {
                ExecutedOn = DateTime.Now,
                People = new Dictionary<string, int> { { "John", 1 }, { "Mary", 2 } },
                Position = new Point(2, 2),
                Children = {
                    Child = new ObjectChildData { Child = new ObjectChildData { Id = 2 }, Id = 3 },
                    Id = 1
                },
                Id = 568,
                Version = Guid.NewGuid()
            };
        }
    }
}
