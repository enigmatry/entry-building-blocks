using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace Enigmatry.BuildingBlocks.Core.Helpers
{
    public static class ObjectExtensions
    {
        public static T DeepClone<T>(this T desiredObject)
        {
            if (desiredObject == null)
            {
                throw new ArgumentNullException(nameof(desiredObject));
            }

            using var stream = new MemoryStream();
            var formatter = new BinaryFormatter();
            formatter.Serialize(stream, desiredObject);
            stream.Position = 0;

            return (T)formatter.Deserialize(stream);
        }
    }
}
