# ASP.NET Core Extensions

A library that provides extensions and utilities for ASP.NET Core applications, enhancing the framework with additional features and patterns.

## Intended Usage

Use this library to extend your ASP.NET Core applications with features such as enhanced exception handling, HTTPS security configuration, action result extensions, and transaction handling.

## Installation

Add the package to your project:

```bash
dotnet add package Enigmatry.Entry.AspNetCore
```

## Usage Examples

### Using the Exception Handling Extensions

```csharp
using Enigmatry.Entry.AspNetCore.Exceptions;
using Microsoft.AspNetCore.Builder;

public class Startup
{
    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        // Add global exception handling middleware
        app.UseEntryExceptionHandler();
        
        // Other middleware
        //...
    }
}
```

### Using HTTPS Configuration Extensions

```csharp
using Enigmatry.Entry.AspNetCore.Security;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

public class Startup
{
    public void ConfigureServices(IServiceCollection services)
    {
        // Setup HTTPS with appropriate settings based on environment
        services.AddEntryHttps(Environment);
    }
    
    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        // Configure HTTPS middleware with appropriate settings for the environment
        app.UseEntryHttps(env);
        
        // Other middleware...
    }
}
```

### Using Action Result Extensions

```csharp
using Enigmatry.Entry.AspNetCore;
using Microsoft.AspNetCore.Mvc;
using AutoMapper;

[ApiController]
[Route("api/products")]
public class ProductController : ControllerBase
{
    private readonly ProductService _productService;
    private readonly IMapper _mapper;
    
    public ProductController(ProductService productService, IMapper mapper)
    {
        _productService = productService;
        _mapper = mapper;
    }

    [HttpGet("{id}")]
    public ActionResult<ProductDto> GetProduct(int id)
    {
        // Use extension method to automatically return NotFound if null
        var product = _productService.GetProductById(id);
        return product.ToActionResult();
    }
    
    [HttpGet("{id}/details")]
    public ActionResult<ProductDetailsDto> GetProductDetails(int id)
    {
        // Use extension method to map and handle not found in one call
        var product = _productService.GetProductById(id);
        return _mapper.MapToActionResult<ProductDetailsDto>(product);
    }
}
```

### Using Transaction Filter Attributes

```csharp
using Enigmatry.Entry.AspNetCore.Filters;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/orders")]
public class OrdersController : ControllerBase
{
    private readonly OrderService _orderService;
    
    public OrdersController(OrderService orderService)
    {
        _orderService = orderService;
    }

    [HttpPost]
    [TransactionFilter] // Automatically starts and commits a transaction
    public async Task<ActionResult<OrderDto>> CreateOrder(CreateOrderRequest request)
    {
        var order = await _orderService.CreateOrderAsync(request);
        return Created($"api/orders/{order.Id}", order);
    }
    
    [HttpDelete("{id}")]
    [CancelSavingTransaction] // Prevents transaction from being saved when needed
    public async Task<IActionResult> CancelOrder(int id)
    {
        await _orderService.CancelOrderAsync(id);
        return NoContent();
    }
}
```

## Summary of Available Features

The Enigmatry.Entry.AspNetCore package provides several utilities and extensions:

1. **Exception Handling**:
   - `UseEntryExceptionHandler()` - Configures global exception handling
   
2. **HTTPS Security**:
   - `AddEntryHttps()` - Configures HTTPS services with environment-specific settings
   - `UseEntryHttps()` - Adds HTTPS middleware with environment-specific settings
   
3. **Action Result Extensions**:
   - `ToActionResult()` - Converts models to ActionResult, handling null cases
   - `MapToActionResult<T>()` - Maps and converts models to ActionResult in one call
   
4. **Transaction Handling**:
   - `[TransactionFilter]` - Filter attribute for transaction management
   - `[CancelSavingTransaction]` - Prevents saving in a transaction

5. **Validation Extensions**:
   - `CamelCasePropertyNameResolver` - Helps with FluentValidation integration

## Integration Example

Here's how to integrate multiple features from the package:

```csharp
using Enigmatry.Entry.AspNetCore.Exceptions;
using Enigmatry.Entry.AspNetCore.Security;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

public class Startup
{
    public void ConfigureServices(IServiceCollection services)
    {
        // Add HTTPS services
        services.AddEntryHttps(Environment);
        
        // Add other services
        services.AddControllers();
        services.AddAutoMapper(typeof(Startup).Assembly);
    }
    
    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        // Add exception handling
        app.UseEntryExceptionHandler();
        
        // Add HTTPS middleware
        app.UseEntryHttps(env);
        
        // Other middleware
        app.UseRouting();
        app.UseAuthorization();
        app.UseEndpoints(endpoints => 
        {
            endpoints.MapControllers();
        });
    }
}
```
