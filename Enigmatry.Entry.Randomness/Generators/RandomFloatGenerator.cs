using System;

namespace Enigmatry.Entry.Randomness.Generators
{
    public sealed class RandomFloatGenerator : BaseRandomGenerator
    {
        public RandomFloatGenerator() : base(typeof(float)) { }

        public override dynamic Generate()
        {
            var bytes = GenerateByteArray();
            return BitConverter.ToSingle(bytes, 0);
        }
    }
}
