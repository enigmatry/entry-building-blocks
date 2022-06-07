using Enigmatry.BuildingBlocks.Core.Helpers;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace Enigmatry.BuildingBlocks.Tests.Core.Helpers
{
    [TestFixture]
    [Obsolete("Obsolete")]
    public class ObjectExtensionsTests
    {
        private static readonly IEnumerable<object> Data = ObjectDataSource.Get();
        private readonly object? _nullObject = null!;

        [Test]
        public void DataUriArrayCreationTypeGuard() =>
            Assert.Throws<ArgumentNullException>(() => _nullObject.DeepClone());

        [Test]
        [TestCaseSource(nameof(Data))]
        public void DeepCopyReturnsDifferentObject(object input)
        {
            var output = input.DeepClone();

            var outputBytes = SerializeObjectToByteArray(output);
            var inputBytes = SerializeObjectToByteArray(input);
            Assert.False(ReferenceEquals(input, output));
            Assert.AreEqual(inputBytes, outputBytes);
        }

        private static byte[] SerializeObjectToByteArray(object value)
        {
            var formatter = new BinaryFormatter();
            using var stream = new MemoryStream();

            formatter.Serialize(stream, value);
            return stream.ToArray();
        }
    }
}
