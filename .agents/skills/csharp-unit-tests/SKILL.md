---
name: csharp-unit-tests
description: Best practices for C# unit and integration testing with NUnit, FluentAssertions and NSubstitute. Use this when writing or reviewing C# tests.
---

# C# Unit Testing

## Stack

- **Test runner**: NUnit 4 (`[Test]` / `[TestCase]` / `[TestCaseSource]`)
- **Assertions**: FluentAssertions — always prefer over `Assert.*`
- **Mocks**: NSubstitute — use `Substitute.For<T>()` in `[SetUp]`
- **Snapshots**: Verify.NUnit — use for integration tests and complex output verification
- **Integration**: `Microsoft.AspNetCore.Mvc.Testing` (`WebApplicationFactory<T>`)

## File and class conventions

- Name test files and classes with the `Fixture` suffix: `Section.cs` → `SectionFixture.cs`
- **Classes named with the `Fixture` suffix do not need `[TestFixture]`** — NUnit discovers them automatically via their `[Test]` methods
- Mirror the production folder structure under the test project root
- Test classes are `internal sealed`

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
    var act = () => new Section(1000, 1000);

    act.Should().Throw<ArgumentException>();
}
```

```csharp
[Test]
public void ToStringReturnsEndValue()
{
    var section = new Section(0, 2500);

    var result = section.ToString();

    result.Should().Be("2500");
}
```

## Parameterized tests — [TestCase] and [TestCaseSource]

**Always scan for duplication before writing a new `[Test]` method.** If two or more tests share identical body structure and differ only in one or more literal values (strings, numbers, enum values), they MUST be collapsed into a single `[TestCase]`-parameterized method.

```csharp
// ❌ avoid — identical structure, only the expected string differs
[Test] public void ActiveStatusHasCorrectName()   { ... status.Name.Should().Be("Active"); }
[Test] public void InactiveStatusHasCorrectName() { ... status.Name.Should().Be("Inactive"); }

// ✅ correct — collapsed into one parameterized test
[TestCase(UserStatusId.Active,   "Active")]
[TestCase(UserStatusId.Inactive, "Inactive")]
public void StatusHasCorrectName(UserStatusId id, string expected)
{
    var status = UserStatus.FromValue(id.Value);

    status.Name.Should().Be(expected);
}
```

Use `[TestCase]` whenever the same assertion logic applies to multiple input values:
```csharp
[TestCase("")]
[TestCase("   ")]
[TestCase(null)]
public void CreateUserWithEmptyNameThrows(string? name)
{
    var act = () => new UserBuilder().WithFullName(name!).Build();

    act.Should().Throw<ArgumentException>();
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
    var act = () => new UserBuilder().WithEmailAddress(email).Build();

    act.Should().Throw<ArgumentException>();
}
```

## Mocks

Create mocks in `[SetUp]`; never share mutable mock state across tests:
```csharp
private IMyService _myService = null!;

[SetUp]
public void SetUp() => _myService = Substitute.For<IMyService>();
```

## Exception assertions

Always use a lambda + `.Should().Throw<T>()` — never `Assert.Throws`:
```csharp
var act = () => new Section(500, 100);

act.Should().Throw<ArgumentException>();
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
- Do **not** use Verify for simple unit tests — use explicit FluentAssertions there.

## Integration tests

- Inherit from `IntegrationFixtureBase` (in `Api.Tests/Infrastructure/Api/`) which wraps `WebApplicationFactory`, `TestDatabase` (Testcontainers SQL Server), and per-test scope management.
- Seed data in a `[SetUp]` method using the builder pattern (e.g. `new UserBuilder().With*().Build()`) and `AddAndSaveChanges(entity)`.
- Use `Client.GetAsync<T>(url)` / `Client.PostAsync<TRequest, TResponse>(url, body)` helpers from the test HTTP client.
- Assert response bodies with `await Verify(response)` (Verify.NUnit snapshot files).
- Mark integration test classes with `[Category("integration")]` so they can be filtered in CI: `dotnet test --filter "Category=integration"`.

```csharp
[Category("integration")]
public class ProductsControllerFixture : IntegrationFixtureBase
{
    private Product _product = null!;

    [SetUp]
    public void SetUp()
    {
        _product = new ProductBuilder()
            .WithName("Test Product")
            .WithCode("BKXX001");
        AddAndSaveChanges(_product);
    }

    [Test]
    public async Task TestGetById()
    {
        var response = await Client.GetAsync<GetProductDetails.Response>($"api/products/{_product.Id}");

        await Verify(response);
    }
}
```

## What NOT to do

- Do not use underscores in test method names.
- Do not add `[TestFixture]` to classes whose names end with `Fixture`.
- Do not write `// Arrange`, `// Act`, `// Assert` comments.
- Do not use `Assert.That` — use FluentAssertions only.
- Do not leave empty catch blocks.
- Do not write separate `[Test]` methods for cases that differ only in input values — use `[TestCase]` or `[TestCaseSource]` instead. **Always check for this before writing any new `[Test]` method.**
