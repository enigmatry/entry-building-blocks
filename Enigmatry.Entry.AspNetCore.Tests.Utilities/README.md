# ASP.NET Core Tests Utilities

A library that provides testing utilities and helpers for ASP.NET Core applications, making it easier to write clean, effective tests for web applications and APIs.

## Intended Usage

Use this library when you need to write tests for ASP.NET Core applications. It provides utilities for mocking HTTP contexts, working with controllers, testing middleware, and more.

## Installation

Add the package to your project:

```bash
dotnet add package Enigmatry.Entry.AspNetCore.Tests.Utilities
```

## Usage Example

Testing exception handling in a controller:

```csharp
using Enigmatry.Entry.AspNetCore.Tests;
using Enigmatry.Entry.AspNetCore.Filters;
using Microsoft.AspNetCore.Mvc;
using NUnit.Framework;
using FakeItEasy;
using FluentValidation;

[TestFixture]
public class ExceptionHandlingTests
{    [Test]
    public void HandleExceptionsFilter_ShouldReturnBadRequest_WhenValidationExceptionOccurs()
    {
        // Arrange
        var exceptionContext = ExceptionContextBuilder.Create()
            .WithValidationException()
            .WithJsonRequest()
            .Build();
        var filter = new HandleExceptionsFilter();
        
        // Act
        filter.OnException(exceptionContext);
        
        // Assert        var result = exceptionContext.Result as BadRequestObjectResult;
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Value, Is.Not.Null);
    }
}
```

Testing HTTP contexts:

```csharp
using Enigmatry.Entry.AspNetCore.Tests;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Diagnostics;
using NUnit.Framework;

[TestFixture]
public class HttpContextTests
{    [Test]
    public void HttpContextBuilder_ShouldCreateValidContext_WithException()
    {
        // Arrange
        var exception = new InvalidOperationException("Test exception");
        
        // Act
        var context = HttpContextBuilder.Create()
            .With(exception)
            .Build();
          // Assert
        var feature = context.Features.Get<IExceptionHandlerPathFeature>();
        Assert.That(feature, Is.Not.Null);
        Assert.That(feature.Error.Message, Is.EqualTo("Test exception"));
        Assert.That(context.Request.Headers["Accept"], Is.EqualTo("application/json"));
    }
}
```

## Service Scope Example

```csharp
using Enigmatry.Entry.AspNetCore.Tests.Utilities;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

[TestFixture]
public class ServiceTests
{    [Test]
    public void ServiceScope_ResolvesService_Successfully()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddScoped<IMyService, MyService>();
        var provider = services.BuildServiceProvider();
        
        // Act
        using var scope = provider.CreateScope();
        var service = scope.Resolve<IMyService>();
          // Assert
        Assert.That(service, Is.Not.Null);
        Assert.That(service, Is.TypeOf<MyService>());
    }
}
```

## Database Utilities Example

```csharp
using Enigmatry.Entry.AspNetCore.Tests.Utilities.Database;
using Microsoft.Data.SqlClient;
using NUnit.Framework;

[TestFixture]
public class DatabaseTests
{    [Test]
    public void DatabaseHelpers_ProvidesDropAllSql()
    {
        // Arrange
        var connectionString = "Server=(localdb)\\MSSQLLocalDB;Database=TestDb;Integrated Security=true";
        
        // Act & Assert
        Assert.NotNull(DatabaseHelpers.DropAllSql);
        
        // Example of how you might use it in a real test
        // using var connection = new SqlConnection(connectionString);
        // connection.Open();
        // var command = new SqlCommand(DatabaseHelpers.DropAllSql, connection);
        // command.ExecuteNonQuery();
    }
}
```
