using System;

namespace Enigmatry.BuildingBlocks.Randomness.Generators
{
    public class RandomDoubleGenerator : BaseRandomGenerator
    {
        public RandomDoubleGenerator() : base(typeof(double)) { }

        public override dynamic Generate()
        {
            var bytes = GenerateByteArray();
            return BitConverter.ToDouble(bytes, 0);
        }
    }
}
