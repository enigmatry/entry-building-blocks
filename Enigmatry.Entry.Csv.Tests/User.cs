namespace Enigmatry.Entry.Csv.Tests;

public class User
{
    public string FirstName { get; init; } = string.Empty;
    [Ignore] public string IgnoreMe { get; init; } = "Ignore me please!";
    public string LastName { get; init; } = string.Empty;
    public int Age { get; init; }
    public string Username { get; init; } = string.Empty;

    [Name("Ingelogd op")] public DateTimeOffset LastLogon { get; init; }
    public DateTime SomeDateTime { get; init; }
}
