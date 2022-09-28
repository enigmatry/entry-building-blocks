namespace Enigmatry.Entry.Randomness.Generators
{
    public class RandomStringGenerator : BaseRandomGenerator
    {
        private readonly RandomIntGenerator _intGenerator = new();
        private readonly RandomCharGenerator _charGenerator;

        public RandomStringGenerator(char[]? forbiddenChars = null) : base(typeof(string))
        {
            _charGenerator = new RandomCharGenerator(forbiddenChars);
        }

        public override dynamic Generate()
        {
            var stringLength = _intGenerator.GeneratePositiveInt(2);
            return GenerateWithGivenLength(stringLength);
        }

        public string Generate(int stringLength) => GenerateWithGivenLength(stringLength);

        private string GenerateWithGivenLength(int length)
        {
            var gottenChars = new char[length];
            for (var index = 0; index < length; index++)
            {
                gottenChars[index] = _charGenerator.Generate();
            }

            return new string(gottenChars);
        }
    }
}
