using Enigmatry.Entry.Randomness.Contracts;
using JetBrains.Annotations;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;
using System.Security.Cryptography;

namespace Enigmatry.Entry.Randomness
{
    [PublicAPI]
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

        [SuppressMessage("Design", "CA1063:Implement IDisposable Correctly",
            Justification = "Dispose(true) is called in Generator's Dispose method.")]
        [SuppressMessage("Usage", "CA1816:Dispose methods should call SuppressFinalize",
            Justification = "No finalizer is being used nor will it be used and all derived classes are sealed.")]
        public void Dispose()
        {
            Dispose(true);
            Generator.Dispose();
        }

        protected virtual void Dispose(bool disposing)
        {
            // Generator's implementation is empty thus no action here too.
        }

        public abstract dynamic Generate();
    }
}
