using Enigmatry.Entry.Randomness.Contracts;
using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Enigmatry.Entry.Randomness.Generators
{
    [PublicAPI]
    public sealed class RandomArrayGenerator
    {
        private readonly Lazy<IEnumerable<IGenerateRandomness>> _availableGenerators;
        private readonly char[] _forbiddenChars = Array.Empty<char>();

        public RandomArrayGenerator(char[]? forbiddenChars = null)
        {
            _availableGenerators = new Lazy<IEnumerable<IGenerateRandomness>>(FillGenerators);

            if (forbiddenChars != null)
            {
                _forbiddenChars = forbiddenChars;
            }
        }

        public T[] Of<T>()
        {
            var length = RandomIntGenerator.GeneratePositiveInt(2);
            return Of<T>(length);
        }

        public T[] Of<T>(int length)
        {
            var generator = _availableGenerators.Value.Single(x => x.GeneratorType == typeof(T));
            var array = new T[length];

            for (var index = 0; index < length; index++)
            {
                array[index] = generator.Generate();
            }

            return array;
        }

        private IEnumerable<IGenerateRandomness> FillGenerators()
        {
            yield return new RandomIntGenerator();
            yield return new RandomStringGenerator(_forbiddenChars);
            yield return new RandomLongGenerator();
            yield return new RandomDoubleGenerator();
            yield return new RandomFloatGenerator();
            yield return new RandomShortGenerator();
            yield return new RandomUnsignedShortGenerator();
            yield return new RandomUnsignedIntGenerator();
            yield return new RandomUnsignedLongGenerator();
            yield return new RandomCharGenerator(_forbiddenChars);
            yield return new RandomBoolGenerator();
            yield return new RandomByteGenerator();
            yield return new RandomSignedByteGenerator();
        }
    }
}
