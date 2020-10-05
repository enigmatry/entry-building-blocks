using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using CsvHelper;

namespace Enigmatry.Blueprint.BuildingBlocks.Csv
{
    public class CsvHelper<T>
    {
        public IEnumerable<T> GetRecords(Stream stream, CultureInfo culture)
        {
            using var textReader = new StreamReader(stream);
            using var reader = new CsvReader(textReader, culture);
            return reader.GetRecords<T>().ToList();
        }

        public byte[] WriteRecords(IEnumerable<T> records, CultureInfo culture)
        {
            using var memoryStream = new MemoryStream();
            using var streamWriter = new StreamWriter(memoryStream);
            using var writer = new CsvWriter(streamWriter, culture);

            writer.WriteRecords(records);
            streamWriter.Flush();
            return memoryStream.ToArray();
        }
    }
}
