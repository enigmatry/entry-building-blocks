using System;

namespace Enigmatry.Entry.Randomness.Generators
{
    public class RandomIntGenerator : BaseRandomGenerator
    {
        public RandomIntGenerator() : base(typeof(int)) { }

        public override dynamic Generate() => GenerateInteger(int.MinValue, int.MaxValue);

        public int PositiveInt => GenerateInteger(1, int.MaxValue);
        public int NegativeInt => PositiveInt * -1;

        public int GenerateNonNegativeInt(int maxValue) => GeneratePositiveInteger(maxValue);

        public int GeneratePositiveInt(int length)
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
