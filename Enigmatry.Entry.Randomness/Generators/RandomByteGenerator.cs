using System.Linq;

namespace Enigmatry.Entry.Randomness.Generators
{
    public class RandomByteGenerator : BaseRandomGenerator
    {
        public RandomByteGenerator() : base(typeof(byte)) { }

        public override dynamic Generate() => GenerateByteArray().Single();
    }
}
