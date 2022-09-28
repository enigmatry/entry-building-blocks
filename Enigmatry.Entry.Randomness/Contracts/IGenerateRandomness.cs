using System;

namespace Enigmatry.Entry.Randomness.Contracts
{
    public interface IGenerateRandomness
    {
        Type GeneratorType { get; }

        dynamic Generate();
    }
}
