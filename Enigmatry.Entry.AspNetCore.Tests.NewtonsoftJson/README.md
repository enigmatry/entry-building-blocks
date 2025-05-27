# ASP.NET Core Tests Newtonsoft.Json

A library that provides testing utilities for ASP.NET Core applications using Newtonsoft.Json as the JSON serialization provider.

## Intended Usage

Use this library when testing ASP.NET Core applications that use Newtonsoft.Json for JSON serialization. It includes helper methods for serializing and deserializing test data, as well as utilities for HTTP request and response handling.

## Installation

Add the package to your project:

```bash
dotnet add package Enigmatry.Entry.AspNetCore.Tests.NewtonsoftJson
```

## Usage Examples

### Deserializing HTTP Response Content

```csharp
using Enigmatry.Entry.AspNetCore.Tests.NewtonsoftJson.Http;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using NUnit.Framework;

[TestFixture]
public class ApiClientTests
{
    private readonly HttpClient _httpClient;

    public ApiClientTests()
    {
        _httpClient = new HttpClient();
        _httpClient.BaseAddress = new Uri("https://api.example.com/");
    }    [Test]
    public async Task GetProducts_ReturnsProducts_UsingDeserializeAsync()
    {
        // Arrange & Act
        var response = await _httpClient.GetAsync("api/products");
        
        // Assert
        // DeserializeAsync doesn't check status code, just deserializes content
        var products = await response.DeserializeAsync<List<Product>>();        Assert.That(products, Is.Not.Null);
        Assert.That(products, Is.Not.Empty);
    }    [Test]
    public async Task GetProduct_WithStatusCheck_ThrowsOnNonSuccessStatus()
    {
        // Arrange & Act
        var response = new HttpResponseMessage(System.Net.HttpStatusCode.NotFound);
        response.Content = new StringContent("Product not found");
        response.RequestMessage = new HttpRequestMessage(HttpMethod.Get, "api/products/999");
        
        // Assert
        // DeserializeWithStatusCodeCheckAsync will throw if status code is not success        Assert.ThrowsAsync<HttpRequestException>(async () => 
            await response.DeserializeWithStatusCodeCheckAsync<Product>());
    }    [Test]
    public async Task EnsureSuccessStatusCode_ThrowsWithContent_WhenNotSuccess()
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
    public async Task TestResponseAssertions_UsingExtensionMethods()
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

### Using HttpClient Extensions

```csharp
using Enigmatry.Entry.AspNetCore.Tests.NewtonsoftJson.Http;
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
    }    [Test]
    public async Task GetAsync_SimplifiesHttpGetWithDeserialization()
    {
        // The GetAsync extension method combines request and deserialization
        var product = await _httpClient.GetAsync<Product>("api/products/1");
        
        // Result is automatically deserialized and status checked        Assert.That(product, Is.Not.Null);
        Assert.That(product.Id, Is.EqualTo(1));
    }
      [Test]
    public async Task PostAsync_SimplifiesHttpPostWithSerialization()
    {
        // Create a product to send
        var newProduct = new Product { Name = "New Product", Price = 19.99m };
        
        // The PostAsync extension method handles serialization
        await _httpClient.PostAsync("api/products", newProduct);
        
        // There's also a version that returns a typed response
        var createdProduct = await _httpClient.PostAsync<Product, Product>("api/products", newProduct);
          Assert.That(createdProduct, Is.Not.Null);
        Assert.That(createdProduct.Id, Is.Not.EqualTo(0));
    }
      [Test]
    public async Task PutAsync_SimplifiesHttpPutWithSerialization()
    {
        // Create a product to update
        var updatedProduct = new Product { Id = 1, Name = "Updated Product", Price = 29.99m };
        
        // The PutAsync extension methods handle serialization
        await _httpClient.PutAsync("api/products/1", updatedProduct);
        
        // There's also a version that returns a typed response
        var result = await _httpClient.PutAsync<Product, Product>("api/products/1", updatedProduct);
        
        Assert.That(result, Is.Not.Null);
    }
}
```

## Configuring JSON Serialization Settings

The library uses a central `HttpSerializationSettings` class for controlling JSON serialization behavior:

```csharp
using Enigmatry.Entry.AspNetCore.Tests.NewtonsoftJson.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

// You can customize the JSON serialization settings used by all methods
HttpSerializationSettings.Settings = new JsonSerializerSettings 
{
    Formatting = Formatting.Indented,
    NullValueHandling = NullValueHandling.Ignore,
    DateTimeZoneHandling = DateTimeZoneHandling.Utc,
    Converters = new List<JsonConverter> 
    { 
        new StringEnumConverter() 
    }
};

// After configuration, all deserialization calls will use these settings
```

## Additional Information

This library is designed to work with the following dependencies:
- Newtonsoft.Json - For JSON serialization
- Microsoft.AspNetCore.Mvc - For ValidationProblemDetails 
- System.Net.Http.Formatting - For JsonMediaTypeFormatter

It works well with XUnit, NUnit, or MSTest for testing ASP.NET Core applications that use Newtonsoft.Json as their JSON serializer.
```
