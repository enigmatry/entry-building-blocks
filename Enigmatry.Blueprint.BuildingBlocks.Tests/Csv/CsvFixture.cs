using Enigmatry.Blueprint.BuildingBlocks.Csv;
using FluentAssertions;
using NUnit.Framework;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;

namespace Enigmatry.Blueprint.BuildingBlocks.Tests.Csv
{
    public class CsvFixture
    {
        [Test]
        public void TestWriteRecords()
        {
            var users = new List<User>()
            {
                new User() {
                    FirstName = "John",
                    LastName = "Doe",
                    Age = 30
                }
            };

            var helper = new CsvHelper<User>();
            var bytes = helper.WriteRecords(users, CultureInfo.InvariantCulture);
            var result = Encoding.UTF8.GetString(bytes, 0, bytes.Length);

            result.Should().Contain("FirstName,LastName,Age");
            result.Should().Contain("John,Doe,30");
        }

        [Test]
        public void TestReadRecords()
        {
            var csv = "FirstName,LastName,Age\nJohn,Doe,30";
            var bytes = Encoding.ASCII.GetBytes(csv);
            using var stream = new MemoryStream(bytes);

            var helper = new CsvHelper<User>();
            var result = helper.GetRecords(stream, CultureInfo.InvariantCulture);

            result.First().FirstName.Should().Be("John");
            result.First().LastName.Should().Be("Doe");
            result.First().Age.Should().Be(30);
        }
    }
}
