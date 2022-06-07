using System;
using System.Collections.Generic;
using System.Linq;

namespace Enigmatry.BuildingBlocks.Randomness.Generators
{
    public class RandomCharGenerator : BaseRandomGenerator
    {
        private const int FirstSignificantCharIndex = 32;
        private const int SignificantCharsCount = sbyte.MaxValue - FirstSignificantCharIndex;

        private readonly char[] _forbiddenChars = Array.Empty<char>();
        private readonly Lazy<IList<char>> _allAsciiCharacters = new(GetAllAsciiCharacters);

        public RandomCharGenerator(char[]? forbiddenChars = null) : base(typeof(char))
        {
            if (forbiddenChars != null)
            {
                _forbiddenChars = forbiddenChars;
            }
        }

        public override dynamic Generate()
        {
            while (true)
            {
                var randomIndex = GeneratePositiveInteger(SignificantCharsCount - 1);
                var generatedChar = _allAsciiCharacters.Value[randomIndex];

                if (_forbiddenChars.Contains(generatedChar))
                {
                    continue;
                }

                return generatedChar;
            }
        }

        private static IList<char> GetAllAsciiCharacters() =>
            Enumerable.Range(FirstSignificantCharIndex, SignificantCharsCount)
                .Select(Convert.ToChar)
                .ToList();
    }
}
