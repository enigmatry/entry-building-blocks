using System;

namespace Enigmatry.BuildingBlocks.Randomness.Contracts
{
    public interface IGenerateRandomness
    {
        Type GeneratorType { get; }

        dynamic Generate();
    }
}
