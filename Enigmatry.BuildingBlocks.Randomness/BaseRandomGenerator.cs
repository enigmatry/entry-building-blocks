using Enigmatry.BuildingBlocks.Randomness.Contracts;
using System;
using System.Runtime.InteropServices;
using System.Security.Cryptography;

namespace Enigmatry.BuildingBlocks.Randomness
{
    public abstract class BaseRandomGenerator : IGenerateRandomness, IDisposable
    {
        public Type GeneratorType { get; }

        protected BaseRandomGenerator(Type type)
        {
            GeneratorType = type;
        }

        private static readonly RandomNumberGenerator Generator = RandomNumberGenerator.Create();

        protected static int GeneratePositiveInteger(int maximum) => GenerateInteger(0, maximum);

        protected static int GeneratePositiveInteger() => GenerateInteger(0, int.MaxValue);

        protected static int GenerateInteger(int minimum, int maximum)
        {
            var fourBytes = new byte[4];
            Generator.GetBytes(fourBytes);

            var scale = BitConverter.ToUInt32(fourBytes, 0);
            var result = minimum + ((maximum - minimum) * (scale / (uint.MaxValue + 1.0)));

            return (int)Math.Round(result, 0, MidpointRounding.AwayFromZero);
        }

        protected byte[] GenerateByteArray()
        {
            var generatorTypeSize = Marshal.SizeOf(GeneratorType);
            var bytes = new byte[generatorTypeSize];
            Generator.GetBytes(bytes);
            return bytes;
        }

        public void Dispose() => Generator.Dispose();

        public abstract dynamic Generate();
    }
}
