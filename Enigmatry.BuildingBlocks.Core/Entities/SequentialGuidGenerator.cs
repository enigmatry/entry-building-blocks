using System;
using System.Threading;

namespace Enigmatry.BuildingBlocks.Core.Entities
{
    /// <summary>
    ///     Generates sequential <see cref="Guid" /> values using the same algorithm as NEWSEQUENTIALID()
    ///     in Microsoft SQL Server. This is useful when entities are being saved to a database where sequential
    ///     GUIDs will provide a performance benefit. The generated values are non-temporary, meaning they will
    ///     be saved to the database.
    /// </summary>
    public static class SequentialGuidGenerator
    {
        /// <summary>
        ///     Gets a value to be assigned to a property.
        /// </summary>
        /// <returns> The value to be assigned to a property. </returns>
        public static Guid Generate()
        {
            var counter = DateTime.UtcNow.Ticks;
            var guidBytes = Guid.NewGuid().ToByteArray();
            var counterBytes = BitConverter.GetBytes(Interlocked.Increment(ref counter));

            if (!BitConverter.IsLittleEndian)
                Array.Reverse(counterBytes);

            guidBytes[08] = counterBytes[1];
            guidBytes[09] = counterBytes[0];
            guidBytes[10] = counterBytes[7];
            guidBytes[11] = counterBytes[6];
            guidBytes[12] = counterBytes[5];
            guidBytes[13] = counterBytes[4];
            guidBytes[14] = counterBytes[3];
            guidBytes[15] = counterBytes[2];

            return new Guid(guidBytes);
        }
    }
}
