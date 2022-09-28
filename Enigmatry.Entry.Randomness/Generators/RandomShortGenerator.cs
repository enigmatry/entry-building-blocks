namespace Enigmatry.Entry.Randomness.Generators
{
    public class RandomShortGenerator : BaseRandomGenerator
    {
        public RandomShortGenerator() : base(typeof(short)) { }

        public override dynamic Generate() => (short)GenerateInteger(short.MinValue, short.MaxValue);
    }
}
