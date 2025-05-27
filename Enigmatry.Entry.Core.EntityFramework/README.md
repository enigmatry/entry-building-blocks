# Core EntityFramework Library

This library provides extension methods and utilities for Entity Framework Core, focusing on querying and data seeding.

## Intended Usage

Use this library when you need additional query capabilities with Entity Framework Core, such as entity not found handling, mapping query results, and pagination support.

## Installation

Add the package to your project:

```bash
dotnet add package Enigmatry.Entry.Core.EntityFramework
```

## Usage Examples

### Using Query Extensions

```csharp
using Enigmatry.Entry.Core.EntityFramework;
using Enigmatry.Entry.Core.Entities;
using Enigmatry.Entry.Core.Paging;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

// Sample entity class
public class Product 
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public bool IsAvailable { get; set; }
}

public class ProductDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public decimal Price { get; set; }
}

public class ProductService
{
    private readonly DbContext _dbContext;
    private readonly IMapper _mapper;
    
    public ProductService(DbContext dbContext, IMapper mapper)
    {
        _dbContext = dbContext;
        _mapper = mapper;
    }
    
    // Find a product by ID, throw exception if not found
    public async Task<Product> GetProductById(int id)
    {
        return await _dbContext.Set<Product>()
            .Where(p => p.Id == id)
            .SingleOrNotFoundAsync();
    }

    // Map a single entity to a DTO
    public async Task<ProductDto> GetProductDtoById(int id)
    {
        return await _dbContext.Set<Product>()
            .Where(p => p.Id == id)
            .SingleOrDefaultMappedAsync<Product, ProductDto>(_mapper);
    }
    
    // Map a collection of entities to DTOs
    public async Task<List<ProductDto>> GetAvailableProductDtos()
    {
        return await _dbContext.Set<Product>()
            .Where(p => p.IsAvailable)
            .ToListMappedAsync<Product, ProductDto>(_mapper);
    }
    
    // Get a paged response of products
    public async Task<PagedResponse<Product>> GetPagedProducts(int pageNumber, int pageSize)
    {
        var request = new PagedRequest
        {
            PageNumber = pageNumber,
            PageSize = pageSize,
            SortBy = "Name",
            SortDirection = "asc"
        };
        
        return await _dbContext.Set<Product>()
            .Where(p => p.IsAvailable)
            .ToPagedResponseAsync(request);
    }
}

```

## Required Dependencies

This library builds on Entity Framework Core and has the following dependencies:

1. Entity Framework Core
2. System.Linq.Dynamic.Core - For dynamic sorting
3. AutoMapper - For mapping entities to DTOs

## Dependency Injection Example

```csharp
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using AutoMapper;

public class Startup
{
    public void ConfigureServices(IServiceCollection services)
    {
        // Register DbContext
        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));
            
        // Register AutoMapper (required for mapping extensions)
        services.AddAutoMapper(typeof(Startup).Assembly);
    }
}
```

### Using the Seeding Interface

```csharp
using Enigmatry.Entry.Core.EntityFramework.Seeding;
using Microsoft.EntityFrameworkCore;

// Create a seeder class
public class ProductSeeder : ISeeding
{
    public void Seed(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Product>().HasData(
            new Product 
            { 
                Id = 1, 
                Name = "Product 1", 
                Price = 19.99m, 
                IsAvailable = true 
            },
            new Product 
            { 
                Id = 2, 
                Name = "Product 2", 
                Price = 29.99m, 
                IsAvailable = true 
            },
            new Product 
            { 
                Id = 3, 
                Name = "Product 3", 
                Price = 39.99m, 
                IsAvailable = false 
            }
        );
    }
}

// Use the seeder in your DbContext
public class ApplicationDbContext : DbContext
{
    public DbSet<Product> Products { get; set; } = default!;
    
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        // Apply the seeder
        new ProductSeeder().Seed(modelBuilder);
    }
}
```
