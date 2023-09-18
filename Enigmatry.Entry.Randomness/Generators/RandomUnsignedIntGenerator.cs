using System;

namespace Enigmatry.Entry.Randomness.Generators
{
    public sealed class RandomUnsignedIntGenerator : BaseRandomGenerator
    {
        public RandomUnsignedIntGenerator() : base(typeof(uint)) { }

        public override dynamic Generate()
        {
            var bytes = GenerateByteArray();
            return BitConverter.ToUInt32(bytes, 0);
        }
    }
}
