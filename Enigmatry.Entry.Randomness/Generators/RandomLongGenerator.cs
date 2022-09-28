using System;

namespace Enigmatry.Entry.Randomness.Generators
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
