---
name: csharp-unit-tests
description: Best practices for C# unit and integration testing with NUnit, Shouldly and FakeItEasy. Use this when writing or reviewing C# tests.
---

# C# Unit Testing

## Stack

- **Test runner**: NUnit 4 (`[Test]` / `[TestCase]` / `[TestCaseSource]`)
- **Assertions**: Shouldly — always prefer over `Assert.*` (`result.ShouldBe(...)`, `collection.ShouldContain(...)`)
- **Mocks**: FakeItEasy — use `A.Fake<T>()` in `[SetUp]`
- **Snapshots**: Verify.NUnit — use for integration tests and complex output verification
- **Integration**: `Microsoft.AspNetCore.Mvc.Testing` (`WebApplicationFactory<T>`)

## File and class conventions

- Name test files and classes with the `Fixture` suffix: `Section.cs` → `SectionFixture.cs`
- **Classes named with the `Fixture` suffix do not need `[TestFixture]`** — NUnit discovers them automatically via their `[Test]` methods
- Mirror the production folder structure under the test project root
- Unit test classes are `public class`; integration test classes that contain internal test infrastructure are `internal class`

## Naming — no underscores

Test method names use **descriptive camelCase** — no underscores, no prefixes like `Constructor_` or `ToString_`:

```csharp
// ✅ correct
WhenEndExceedsMaxLengthThrows
ToStringReturnsEndValue
MissingSectionsLineThrows
WithValidRangeCreatesSection

// ❌ avoid
Constructor_WhenEndExceedsMaxLength_ThrowsArgumentOutOfRangeException
Parse_MissingSectionsLine_ThrowsFormatException
```

## Test structure — AAA without comments

Separate Arrange / Act / Assert with a **blank line only** — never write `// Arrange`, `// Act`, or `// Assert` comments:

```csharp
[Test]
public void WhenStartEqualsEndThrows()
{
    Should.Throw<ArgumentException>(() => new Section(1000, 1000));
}
```

```csharp
[Test]
public void ToStringReturnsEndValue()
{
    var section = new Section(0, 2500);

    var result = section.ToString();

    result.ShouldBe("2500");
}
```

## Parameterized tests — [TestCase] and [TestCaseSource]

**Always scan for duplication before writing a new `[Test]` method.** If two or more tests share identical body structure and differ only in one or more literal values (strings, numbers, enum values), they MUST be collapsed into a single `[TestCase]`-parameterized method.

```csharp
// ❌ avoid — identical structure, only the expected string differs
[Test] public void ActiveStatusHasCorrectName()   { ... status.Name.ShouldBe("Active"); }
[Test] public void InactiveStatusHasCorrectName() { ... status.Name.ShouldBe("Inactive"); }

// ✅ correct — collapsed into one parameterized test
[TestCase(UserStatusId.Active,   "Active")]
[TestCase(UserStatusId.Inactive, "Inactive")]
public void StatusHasCorrectName(UserStatusId id, string expected)
{
    var status = UserStatus.FromValue(id.Value);

    status.Name.ShouldBe(expected);
}
```

Use `[TestCase]` whenever the same assertion logic applies to multiple input values:
```csharp
[TestCase("")]
[TestCase("   ")]
[TestCase(null)]
public void CreateUserWithEmptyNameThrows(string? name)
{
    Should.Throw<ArgumentException>(() => new UserBuilder().WithFullName(name!).Build());
}
```

Use `[TestCaseSource]` when test data is more complex or reused across multiple tests:
```csharp
private static readonly object[][] InvalidEmails =
[
    ["not-an-email"],
    ["missing@domain"],
    ["@nodomain.com"],
];

[TestCaseSource(nameof(InvalidEmails))]
public void CreateUserWithInvalidEmailThrows(string email)
{
    Should.Throw<ArgumentException>(() => new UserBuilder().WithEmailAddress(email).Build());
}
```

## Mocks

Create fakes in `[SetUp]`; never share mutable fake state across tests:
```csharp
private IMyService _myService = null!;

[SetUp]
public void SetUp() => _myService = A.Fake<IMyService>();
```

Configure return values and verify calls with `A.CallTo`:
```csharp
A.CallTo(() => _myService.GetAsync(id)).Returns(expected);

A.CallTo(() => _myService.SaveAsync(A<MyEntity>._)).MustHaveHappenedOnceExactly();
```

## Exception assertions

Use `Should.Throw<T>` for synchronous code and `Should.ThrowAsync<T>` for async — never `Assert.Throws`:
```csharp
Should.Throw<ArgumentException>(() => new Section(500, 100));
```

```csharp
await Should.ThrowAsync<InvalidOperationException>(async () => await _service.ProcessAsync(null));
```

## Snapshot testing with Verify.NUnit

Use Verify for integration tests and any test that validates complex output (multi-line strings, JSON responses, serialized objects). Verify stores approved snapshots in `.verified.txt` files next to the test source.

```csharp
[Test]
public async Task GetConfigurationMatchesSnapshot()
{
    var response = await _client.GetAsync("/configuration");
    var body = await response.Content.ReadAsStringAsync();

    var settings = new VerifySettings();
    settings.ScrubLinesContaining("time-dependent content");
    await Verify(body, settings);
}
```

- On first run, Verify creates a `.received.txt` file — review it and rename/copy to `.verified.txt` to approve.
- Commit `.verified.txt` files alongside the tests.
- Do **not** use Verify for simple unit tests — use explicit Shouldly assertions there.

## Integration tests

- Create a base fixture class that wraps `WebApplicationFactory<Program>`, sets up `HttpClient` in `[SetUp]`, and disposes everything in `[TearDown]`.
- Override services in `WithWebHostBuilder(builder => builder.ConfigureServices(...))` to substitute real infrastructure with test doubles.
- Use `Client.GetAsync(url)` / `Client.PostAsync(url, content)` directly on the `HttpClient`.
- Assert HTTP status codes with Shouldly: `response.StatusCode.ShouldBe(HttpStatusCode.OK)`.
- Assert response bodies with `await Verify(body)` (Verify.NUnit snapshot files) for complex outputs.
- Mark integration test classes with `[Category("integration")]` so they can be filtered in CI: `dotnet test --filter "TestCategory=integration"`.

```csharp
[Category("integration")]
internal class WeatherForecastControllerFixture : SampleAppFixtureBase
{
    [Test]
    public async Task GetForecastReturnsOk()
    {
        var response = await Client.GetAsync("WeatherForecast");

        response.StatusCode.ShouldBe(HttpStatusCode.OK);
    }

    [Test]
    public async Task GetForecastMatchesSnapshot()
    {
        var response = await Client.GetAsync("WeatherForecast");
        var body = await response.Content.ReadAsStringAsync();

        await Verify(body);
    }
}
```

## What NOT to do

- Do not use underscores in test method names.
- Do not add `[TestFixture]` to classes whose names end with `Fixture`.
- Do not write `// Arrange`, `// Act`, `// Assert` comments.
- Do not use `Assert.That` — use Shouldly only.
- Do not leave empty catch blocks.
- Do not write separate `[Test]` methods for cases that differ only in input values — use `[TestCase]` or `[TestCaseSource]` instead. **Always check for this before writing any new `[Test]` method.**
