using System.Linq;

namespace Enigmatry.Entry.Randomness.Generators
{
    public class RandomBoolGenerator : BaseRandomGenerator
    {
        public RandomBoolGenerator() : base(typeof(bool)) { }

        public override dynamic Generate()
        {
            // .First() must be used due to: https://stackoverflow.com/questions/13285942/different-bool-size
            var byteGenerated = GenerateByteArray().First();
            return byteGenerated % 2 == 1;
        }

        public bool Next => (bool)Generate();
    }
}
