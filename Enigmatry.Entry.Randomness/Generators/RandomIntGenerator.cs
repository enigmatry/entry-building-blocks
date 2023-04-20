using JetBrains.Annotations;
using System;

namespace Enigmatry.Entry.Randomness.Generators
{
    [PublicAPI]
    public sealed class RandomIntGenerator : BaseRandomGenerator
    {
        public RandomIntGenerator() : base(typeof(int)) { }

        public override dynamic Generate() => GenerateInteger(int.MinValue, int.MaxValue);

        public static int PositiveInt => GenerateInteger(1, int.MaxValue);
        public static int NegativeInt => PositiveInt * -1;

        public static int GenerateNonNegativeInt(int maxValue) => GeneratePositiveInteger(maxValue);

        public static int GeneratePositiveInt(int length)
        {
            if (length is > 10 or < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(length));
            }

            var limit = (int)Math.Pow(10, length);
            return GenerateInteger(1, limit - 1);
        }
    }
}
