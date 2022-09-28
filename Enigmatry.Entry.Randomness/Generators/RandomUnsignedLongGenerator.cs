using System;

namespace Enigmatry.Entry.Randomness.Generators
{
    public class RandomUnsignedLongGenerator : BaseRandomGenerator
    {
        public RandomUnsignedLongGenerator() : base(typeof(ulong)) { }

        public override dynamic Generate()
        {
            var bytes = GenerateByteArray();
            return BitConverter.ToUInt64(bytes, 0);
        }
    }
}
