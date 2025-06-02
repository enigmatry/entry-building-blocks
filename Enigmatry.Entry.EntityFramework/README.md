# Entity Framework Core Extensions

A library that provides extensions and utilities for working with Entity Framework Core, enhancing the ORM capabilities with additional features and patterns.

## Intended Usage

Use this library to implement common Entity Framework Core patterns and extensions, such as repositories, unit of work, auditing, and soft delete functionality.

## Installation

Add the package to your project:

```bash
dotnet add package Enigmatry.Entry.EntityFramework
```

## Usage Example

Creating a repository:

```csharp
using Enigmatry.Entry.Core.Entities;
using Enigmatry.Entry.EntityFramework;
using Microsoft.EntityFrameworkCore;

// Define an entity
public class Customer : EntityWithTypedId<Guid>
{
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public bool IsActive { get; set; }
}

// Create a DbContext
public class ApplicationDbContext : BaseDbContext
{
    public DbSet<Customer> Customers { get; set; } = default!;
    
    public ApplicationDbContext(DbContextOptions options, EntitiesDbContextOptions entitiesDbContextOptions)
        : base(entitiesDbContextOptions, options)
    {
    }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        // Model builder is configured through base class
    }
}

// Use the repository pattern
public interface ICustomerRepository : IRepository<Customer, Guid>
{
    Task<Customer?> FindByEmailAsync(string email);
}

public class CustomerRepository : EntityFrameworkRepository<Customer, Guid>, ICustomerRepository
{
    public CustomerRepository(ApplicationDbContext dbContext)
        : base(dbContext)
    {
    }
    
    public async Task<Customer?> FindByEmailAsync(string email)
    {
        return await DbSet
            .FirstOrDefaultAsync(c => c.Email == email && c.IsActive);
    }
}
```

## Dependency Injection Example

Register Entity Framework services:

```csharp
using Enigmatry.Entry.Core.Data;
using Enigmatry.Entry.EntityFramework;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

public class Startup
{
    public void ConfigureServices(IServiceCollection services)
    {
        // Configure EntitiesDbContextOptions
        var entitiesOptions = new EntitiesDbContextOptions
        {
            ConfigurationAssembly = typeof(Customer).Assembly
        };
        services.AddSingleton(entitiesOptions);
        
        // Register DbContext
        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));
        
        // Register repositories
        services.AddScoped<ICustomerRepository, CustomerRepository>();
        
        // Add unit of work
        services.AddScoped<IUnitOfWork, EntityFrameworkUnitOfWork>();
        
        // Register DbContext as the concrete DbContext implementation for the unit of work
        services.AddScoped<DbContext>(sp => sp.GetRequiredService<ApplicationDbContext>());
    }
}
```
