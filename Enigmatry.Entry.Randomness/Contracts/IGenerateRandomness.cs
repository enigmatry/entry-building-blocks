namespace Enigmatry.Entry.Randomness.Contracts;

public interface IGenerateRandomness
{
    public Type GeneratorType { get; }

    public dynamic Generate();
}
