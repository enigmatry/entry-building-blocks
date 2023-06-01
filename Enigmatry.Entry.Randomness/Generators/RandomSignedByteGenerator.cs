using System;

namespace Enigmatry.Entry.Randomness.Generators
{
    public sealed class RandomSignedByteGenerator : BaseRandomGenerator
    {
        public RandomSignedByteGenerator() : base(typeof(sbyte)) { }

        public override dynamic Generate()
        {
            var randomInt = GenerateInteger(sbyte.MinValue, sbyte.MaxValue);
            return Convert.ToSByte(randomInt);
        }
    }
}
