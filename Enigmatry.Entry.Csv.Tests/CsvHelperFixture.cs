using System.Globalization;
using System.Text;
using FluentAssertions;
using NUnit.Framework;

namespace Enigmatry.Entry.Csv.Tests;

[Category("unit")]
public class CsvHelperFixture
{
    [TestCaseSource(nameof(WriteTestCases))]
    public void WriteRecords(CsvWriteTestCase testCase)
    {
        var helper = ACsvHelper(testCase.Options);

        var result = WriteRecords(helper, testCase.Users);

        foreach (var csvRow in testCase.CsvRows)
        {
            result.Should().Contain(csvRow);
        }
    }

    [Test]
    public void WriteRecordsToStream_ShouldNotDisposeStream()
    {
        List<User> users = [AUser];
        var helper = ACsvHelper(options => options.WithEncoding(Encoding.UTF8));

        var result = helper.WriteRecordsToStream(users);

        var readAction = () => result.ReadByte();
        readAction.Should().NotThrow<ObjectDisposedException>();
    }

    [TestCaseSource(nameof(WriteTestCases))]
    public void WriteRecordsToStream(CsvWriteTestCase testCase)
    {
        var helper = ACsvHelper(testCase.Options);

        var result = WriteRecordsToStream(helper, testCase.Users);

        foreach (var csvRow in testCase.CsvRows)
        {
            result.Should().Contain(csvRow);
        }
    }

    private static IEnumerable<TestCaseData> WriteTestCases()
    {
        yield return AWriteTestCase(
            [
                "FirstName;LastName;Age;Username;Ingelogd op;SomeDateTime",
                "John;Doe;30;john.doe;2023-12-19 09:30:00;2022-04-27 09:30:00"
            ],
            [AUser],
            options => options.WithEncoding(Encoding.UTF8).WithCulture(DutchCulture),
            "DutchCulture");
        yield return AWriteTestCase(
            [
                "Ime;Prezime;Starost;Korisničko ime;Datum logovanja;Neki datum",
                "John;Doe;30;john.doe;2023-12-19 09:30:00;2022-04-27 09:30:00"
            ],
            [AUser],
            options => options.WithEncoding(Encoding.UTF8).WithCulture(SerbianCulture).WithHeaderNameReplacer(SimulateSerbianStringLocalizer),
            "SerbianCulture");
        yield return AWriteTestCase(
            [
                "FirstName-FirstName;LastName-LastName;Age-Age;Username-Username;Ingelogd op-Ingelogd op;SomeDateTime-SomeDateTime",
                "John;Doe;30;john.doe;2023-12-19 09:30:00;2022-04-27 09:30:00"
            ],
            [AUser],
            options => options.WithEncoding(Encoding.UTF8).WithCulture(DutchCulture)
                .WithHeaderNameReplacer(DuplicateHeaderNameReplacer),
            "DutchCulture_ReplaceColumnNames");
    }

    [TestCaseSource(nameof(ReadTestCases))]
    public void ReadRecords(CsvReadTestCase testCase)
    {
        using var stream = AsCsvStream(testCase.Csv);
        var helper = ACsvHelper(testCase.OptionsBuilder);

        var row = helper.GetRecords(stream).First();
        var user = testCase.User;

        row.FirstName.Should().Be(user.FirstName);
        row.LastName.Should().Be(user.LastName);
        row.Age.Should().Be(user.Age);
        row.Username.Should().Be(user.Username);
    }

    private static IEnumerable<TestCaseData> ReadTestCases()
    {
        yield return AReadTestCase(
            "FirstName;LastName;Age;Username;Ingelogd op;SomeDateTime\n" +
            "John;Doe;30;john.doe;2022-04-30 09:30:00;2022-04-27 09:30:00",
            AUser,
            options => options.WithCulture(DutchCulture),
            "DutchCulture");
        yield return AReadTestCase(
            "Ime;Prezime;Starost;Korisničko ime;Datum logovanja;Neki datum\n" +
            "John;Doe;30;john.doe;2022-04-30 09:30:00;2022-04-27 09:30:00",
            AUser,
            options => options.WithCulture(SerbianCulture).WithHeaderNameReplacer(SimulateSerbianStringLocalizer),
            "SerbianCulture");
        yield return AReadTestCase(
            "FirstName-FirstName;LastName-LastName;Age-Age;Username-Username;Ingelogd op-Ingelogd op;SomeDateTime-SomeDateTime\n" +
            "John;Doe;30;john.doe;2022-04-30 09:30:00;2022-04-27 09:30:00",
            AUser,
            options => options.WithCulture(DutchCulture).WithHeaderNameReplacer(DuplicateHeaderNameReplacer),
            "DutchCulture_ReplaceColumnNames");
    }

    private static TestCaseData AReadTestCase(string csv, User user, Action<CsvHelperOptions> optionsBuilder,
        string testName) =>
        new(new CsvReadTestCase(csv, user, optionsBuilder)) { TestName = testName };

    public record CsvReadTestCase(string Csv, User User, Action<CsvHelperOptions> OptionsBuilder);

    private static TestCaseData AWriteTestCase(IReadOnlyCollection<string> csvRows, IReadOnlyCollection<User> users,
        Action<CsvHelperOptions> optionsBuilder,
        string testName) =>
        new(new CsvWriteTestCase(csvRows, users, optionsBuilder)) { TestName = testName };

    public record CsvWriteTestCase(
        IReadOnlyCollection<string> CsvRows,
        IReadOnlyCollection<User> Users,
        Action<CsvHelperOptions> Options);

    private static CsvHelper<User> ACsvHelper(Action<CsvHelperOptions> optionsBuilder) => new(optionsBuilder);

    private static string WriteRecords<T>(CsvHelper<T> helper, IEnumerable<T> records)
    {
        var bytes = helper.WriteRecords(records);
        var result = Encoding.UTF8.GetString(bytes, 0, bytes.Length);
        return result;
    }

    private static string WriteRecordsToStream<T>(CsvHelper<T> helper, IEnumerable<T> records)
    {
        using var stream = helper.WriteRecordsToStream(records);
        var bytes = stream.ToArray();
        var result = Encoding.UTF8.GetString(bytes, 0, bytes.Length);
        return result;
    }

    private static CultureInfo DutchCulture => CultureInfo.GetCultureInfo("nl-NL");
    private static CultureInfo SerbianCulture => CultureInfo.GetCultureInfo("sr-Latn-RS");

    private static MemoryStream AsCsvStream(string csv)
    {
        var bytes = Encoding.UTF8.GetBytes(csv);
        var stream = new MemoryStream(bytes);
        return stream;
    }

    private static User AUser =>
        new()
        {
            FirstName = "John",
            LastName = "Doe",
            Age = 30,
            Username = "john.doe",
            LastLogon = new DateTimeOffset(new DateTime(2023, 12, 19, 9, 30, 0, DateTimeKind.Local)),
            SomeDateTime = new DateTime(2022, 4, 27, 9, 30, 0, DateTimeKind.Local)
        };

    private static Func<string, string> DuplicateHeaderNameReplacer => name => $"{name}-{name}";

    private static string SimulateSerbianStringLocalizer(string name) =>
        name switch
        {
            "FirstName" => "Ime",
            "LastName" => "Prezime",
            "Age" => "Starost",
            "Username" => "Korisničko ime",
            "Ingelogd op" => "Datum logovanja",
            "SomeDateTime" => "Neki datum",
            _ => name
        };
}
