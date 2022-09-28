using System;

namespace Enigmatry.Entry.Randomness.Generators
{
    public class RandomFloatGenerator : BaseRandomGenerator
    {
        public RandomFloatGenerator() : base(typeof(float)) { }

        public override dynamic Generate()
        {
            var bytes = GenerateByteArray();
            return BitConverter.ToSingle(bytes, 0);
        }
    }
}
