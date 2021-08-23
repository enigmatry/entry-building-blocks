using System;
using System.Collections.Generic;
using System.Linq;

namespace Enigmatry.BuildingBlocks.Validation.Helpers
{
    public static class Check
    {
        public static bool IsNumber(Type type) =>
            new List<Type>
            {
                typeof(short),
                typeof(int),
                typeof(long),
                typeof(decimal),
                typeof(double),
                typeof(float),
                typeof(byte)
            }
            .Any(x => x == type);
    }
}
