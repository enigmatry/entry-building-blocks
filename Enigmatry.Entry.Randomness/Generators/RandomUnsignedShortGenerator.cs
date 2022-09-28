namespace Enigmatry.Entry.Randomness.Generators
{
    public class RandomUnsignedShortGenerator : BaseRandomGenerator
    {
        public RandomUnsignedShortGenerator() : base(typeof(ushort)) { }

        public override dynamic Generate() => (ushort)GenerateInteger(ushort.MinValue, ushort.MaxValue);
    }
}
