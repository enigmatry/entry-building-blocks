# Smart Enums EntityFramework Integration

This library provides integration between Smart Enums and Entity Framework Core, enabling you to use Smart Enum types as properties in your entity models.

## Intended Usage

Use this library to seamlessly store and retrieve Smart Enum values when using Entity Framework Core as your ORM.

## Installation

Add the package to your project:

```bash
dotnet add package Enigmatry.Entry.SmartEnums.EntityFramework
```

## Usage Example

Configure your DbContext to use Smart Enum converters:

```csharp
using Ardalis.SmartEnum;
using Enigmatry.Entry.SmartEnums.Entities;
using Enigmatry.Entry.SmartEnums.EntityFramework;
using Microsoft.EntityFrameworkCore;

// Example SmartEnum
public class ProductCategory : SmartEnum<ProductCategory>
{
    public static readonly ProductCategory Electronics = new(1, nameof(Electronics));
    public static readonly ProductCategory Clothing = new(2, nameof(Clothing));
    public static readonly ProductCategory Books = new(3, nameof(Books));
    public static readonly ProductCategory HomeGoods = new(4, nameof(HomeGoods));

    private ProductCategory(int id, string name) : base(id, name) { }
}

// Entity with SmartEnum property
public class Product
{
    public int Id { get; set; }
    public string Name { get; set; }
    public ProductCategory Category { get; set; }
}

// Entity that uses SmartEnum as primary key
public class CategoryInfo : EntityWithEnumId<ProductCategory>
{
    public string Description { get; set; }
}

// Configuration for the CategoryInfo entity
public class CategoryInfoConfiguration : EntityWithEnumIdConfiguration<CategoryInfo, ProductCategory>
{
    public CategoryInfoConfiguration() : base(nameMaxLength: 50) { }
}

public class ApplicationDbContext : DbContext
{
    public DbSet<Product> Products { get; set; }
    public DbSet<CategoryInfo> CategoryInfos { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Configure all SmartEnums in the application
        modelBuilder.EntryConfigureSmartEnums();
        
        // Option 1: Configure a specific SmartEnum property
        modelBuilder.Entity<Product>()
            .Property(p => p.Category)
            .HasSmartEnumConversion();
            
        // Option 2: Apply the entity configuration
        modelBuilder.ApplyConfiguration(new CategoryInfoConfiguration());
    }
}
```

Using the newly configured model:

```csharp
// Create and save an entity with a SmartEnum property
using (var context = new ApplicationDbContext())
{
    var product = new Product
    { 
        Name = "Laptop",
        Category = ProductCategory.Electronics
    };
    
    context.Products.Add(product);
    await context.SaveChangesAsync();
}

// Query entities by SmartEnum value
using (var context = new ApplicationDbContext())
{
    var electronicsProducts = await context.Products
        .Where(p => p.Category == ProductCategory.Electronics)
        .ToListAsync();
}
```
