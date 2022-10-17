using System.Globalization;
using System.Text;
using FluentAssertions;
using NUnit.Framework;

namespace Enigmatry.Entry.Csv.Tests;

[Category("unit")]
public class CsvFixture
{
    [Test]
    public void TestWriteRecords()
    {
        Console.WriteLine(DateTimeOffset.Now);
        var users = new List<User>
        {
            new User {
                FirstName = "John",
                LastName = "Doe",
                Age = 30,
                LastLogon = new DateTimeOffset(2022, 4, 30, 9, 30, 0, TimeSpan.FromHours(2)),
                SomeDateTime = new DateTime(2022, 4, 27, 9, 30, 0)
            }
        };

        var helper = new CsvHelper<User>();
        var bytes = helper.WriteRecords(users, CultureInfo.GetCultureInfo("nl-NL"));
        var result = Encoding.UTF8.GetString(bytes, 0, bytes.Length);

        result.Should().Contain("FirstName;LastName;Age;Ingelogd op;SomeDateTime");
        result.Should().Contain("John;Doe;30;2022-04-30 09:30:00;2022-04-27 09:30:00");
    }

    [Test]
    public void TestReadRecords()
    {
        var csv = "FirstName;LastName;Age;Ingelogd op;SomeDateTime\n" +
                  "John;Doe;30;2022-04-30 09:30:00;2022-04-27 09:30:00";
        var bytes = Encoding.ASCII.GetBytes(csv);
        using var stream = new MemoryStream(bytes);

        var helper = new CsvHelper<User>();
        var result = helper.GetRecords(stream, CultureInfo.GetCultureInfo("nl-NL"));

        result.First().FirstName.Should().Be("John");
        result.First().LastName.Should().Be("Doe");
        result.First().Age.Should().Be(30);
    }
}
