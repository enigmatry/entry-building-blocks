# ASP.NET Core Tests System.Text.Json

A library that provides testing utilities for ASP.NET Core applications using System.Text.Json as the JSON serialization provider.

## Intended Usage

Use this library when testing ASP.NET Core applications that use System.Text.Json for JSON serialization. It provides helper methods for serializing and deserializing test data, as well as utilities for working with HTTP requests and responses.

## Installation

Add the package to your project:

```bash
dotnet add package Enigmatry.Entry.AspNetCore.Tests.SystemTextJson
```

## Usage Examples

### Deserializing HTTP Response Content

```csharp
using Enigmatry.Entry.AspNetCore.Tests.SystemTextJson.Http;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using NUnit.Framework;

[TestFixture]
public class HttpResponseDeserializationTests
{
    private readonly HttpClient _httpClient;

    public HttpResponseDeserializationTests()
    {
        _httpClient = new HttpClient();
        _httpClient.BaseAddress = new Uri("https://api.example.com/");
    }    [Test]
    public async Task DeserializeAsync_DeserializesResponseContent()
    {
        // Arrange & Act
        var response = await _httpClient.GetAsync("api/products");
        
        // Assert
        // DeserializeAsync doesn't check status code, just deserializes content        var products = await response.DeserializeAsync<List<Product>>();
        Assert.That(products, Is.Not.Null);
        Assert.That(products, Is.Not.Empty);
    }    [Test]
    public async Task DeserializeWithStatusCodeCheckAsync_ThrowsOnNonSuccessStatus()
    {
        // Arrange & Act
        var response = new HttpResponseMessage(System.Net.HttpStatusCode.NotFound);
        response.Content = new StringContent("Product not found");
        response.RequestMessage = new HttpRequestMessage(HttpMethod.Get, "api/products/999");
        
        // Assert
        // DeserializeWithStatusCodeCheckAsync will throw if status code is not success        Assert.ThrowsAsync<HttpRequestException>(async () => 
            await response.DeserializeWithStatusCodeCheckAsync<Product>());
    }    [Test]
    public async Task EnsureSuccessStatusCodeAsync_ThrowsWithDetailedMessage()
    {
        // Arrange
        var response = new HttpResponseMessage(System.Net.HttpStatusCode.BadRequest);
        response.Content = new StringContent("Invalid request parameters");
        response.RequestMessage = new HttpRequestMessage(HttpMethod.Get, "api/products");
        
        // Act & Assert
        // EnsureSuccessStatusCodeAsync enhances exception with content details        var exception = Assert.ThrowsAsync<HttpRequestException>(async () => 
            await response.EnsureSuccessStatusCodeAsync());
            
        Assert.That(exception.Message, Does.Contain("StatusCode: BadRequest"));
        Assert.That(exception.Message, Does.Contain("Invalid request parameters"));
    }
      [Test]
    public async Task TestResponseAssertions_UsingAssertionExtensions()
    {
        // Arrange
        var badRequestResponse = new HttpResponseMessage(System.Net.HttpStatusCode.BadRequest);
        var notFoundResponse = new HttpResponseMessage(System.Net.HttpStatusCode.NotFound);
        
        var validationResponse = new HttpResponseMessage(System.Net.HttpStatusCode.BadRequest);
        validationResponse.Content = new StringContent(@"{
            ""type"": ""https://tools.ietf.org/html/rfc7231#section-6.5.1"",
            ""title"": ""One or more validation errors occurred."",
            ""status"": 400,
            ""errors"": {
                ""Name"": [""The Name field is required.""]
            }
        }");
        
        // Act & Assert
        // Use the assertion extension methods
        badRequestResponse.BeBadRequest();
        notFoundResponse.BeNotFound();
        validationResponse.ContainValidationError("Name", "required");
    }
}
```

### Configuring JSON Serialization Options

The library uses a centralized `HttpSerializationOptions` class for configuring System.Text.Json serialization. By default, it's configured with `PropertyNameCaseInsensitive = true` to make JSON property mapping case-insensitive.

```csharp
using Enigmatry.Entry.AspNetCore.Tests.SystemTextJson.Http;
using System.Text.Json;
using System.Text.Json.Serialization;

// Configure the global JSON serialization settings
HttpSerializationOptions.Options = new JsonSerializerOptions
{
    PropertyNameCaseInsensitive = true,
    PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
    WriteIndented = true,
    DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
    Converters = 
    {
        new JsonStringEnumConverter()
    }
};

// These options will be used by all the extension methods in the library
// for both serialization and deserialization
```
```

## How It Works

The Enigmatry.Entry.AspNetCore.Tests.SystemTextJson library provides a set of extension methods that makes testing ASP.NET Core applications using System.Text.Json easier. Here's how the components work together:

1. **HttpResponseMessageExtensions** provides methods to deserialize HTTP responses with proper error handling.

2. **JsonHttpClientExtensions** simplifies making HTTP requests and handling responses, with automatic serialization of request bodies and deserialization of response bodies.

3. **HttpResponseMessageAssertionsExtensions** provides fluent assertions for testing HTTP responses, especially useful for validating error responses and validation errors.

4. **HttpSerializationOptions** provides a central place to configure JSON serialization options that are used throughout the library.

## Project Dependencies

This library depends on:
- System.Text.Json for JSON serialization
- Microsoft.AspNetCore.Mvc for ValidationProblemDetails
- System.Net.Http.Json for HTTP client integrations
- Shouldly for assertions

It's designed to work with XUnit, NUnit, or MSTest for testing ASP.NET Core applications.
```

### Using HTTP Client Extensions

```csharp
using Enigmatry.Entry.AspNetCore.Tests.SystemTextJson.Http;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using NUnit.Framework;

[TestFixture]
public class HttpClientExtensionsTests
{
    private readonly HttpClient _httpClient;

    public HttpClientExtensionsTests()
    {
        _httpClient = new HttpClient();
        _httpClient.BaseAddress = new Uri("https://api.example.com/");
    }

    [Fact]
    public async Task GetAsync_SimplifiesHttpGetWithDeserialization()
    {
        // GetAsync combines HTTP request and deserialization with status code check
        var product = await _httpClient.GetAsync<Product>("api/products/1");
        
        // Result is automatically deserialized and status checked
        Assert.NotNull(product);
        Assert.Equal(1, product.Id);
    }
    
    [Fact]
    public async Task GetAsync_WithParameters_AppendsQueryParameters()
    {
        // Using GetAsync with URI parameters
        var products = await _httpClient.GetAsync<List<Product>>(
            new Uri("https://api.example.com/api/products"), 
            new KeyValuePair<string, string>("category", "electronics"),
            new KeyValuePair<string, string>("inStock", "true")
        );
        
        Assert.NotNull(products);
        Assert.NotEmpty(products);
    }
    
    [Fact]
    public async Task PostAsync_SimplifiesHttpPostWithSerialization()
    {
        // Create a product to send
        var newProduct = new Product { Name = "New Product", Price = 19.99m };
        
        // PostAsync without response
        await _httpClient.PostAsync("api/products", newProduct);
        
        // PostAsync with typed response
        var createdProduct = await _httpClient.PostAsync<Product, Product>("api/products", newProduct);
        
        Assert.NotNull(createdProduct);
        Assert.NotEqual(0, createdProduct.Id);
    }
    
    [Fact]
    public async Task PutAsync_SimplifiesHttpPutWithSerialization()
    {
        // Create a product to update
        var updatedProduct = new Product { Id = 1, Name = "Updated Product", Price = 29.99m };
        
        // PutAsync without response
        await _httpClient.PutAsync("api/products/1", updatedProduct);
        
        // PutAsync with typed response
        var result = await _httpClient.PutAsync<Product, Product>("api/products/1", updatedProduct);
        
        Assert.NotNull(result);
    }
}
```
