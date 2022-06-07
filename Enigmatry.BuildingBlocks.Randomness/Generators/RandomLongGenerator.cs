using System;

namespace Enigmatry.BuildingBlocks.Randomness.Generators
{
    public class RandomLongGenerator : BaseRandomGenerator
    {
        public RandomLongGenerator() : base(typeof(long)) { }

        public override dynamic Generate()
        {
            var bytes = GenerateByteArray();
            return BitConverter.ToInt64(bytes, 0);
        }
    }
}
