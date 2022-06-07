using System;

namespace Enigmatry.BuildingBlocks.Randomness.Generators
{
    public class RandomSignedByteGenerator : BaseRandomGenerator
    {
        public RandomSignedByteGenerator() : base(typeof(sbyte)) { }

        public override dynamic Generate()
        {
            var randomInt = GenerateInteger(sbyte.MinValue, sbyte.MaxValue);
            return Convert.ToSByte(randomInt);
        }
    }
}
