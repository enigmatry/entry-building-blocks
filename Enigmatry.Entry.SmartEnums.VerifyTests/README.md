# Smart Enums VerifyTests

A library that provides testing utilities for Smart Enums using the Verify testing approach, allowing for snapshot testing of Smart Enum behaviors and serialization.

## Intended Usage

Use this library when you need to write tests for Smart Enum types using the Verify snapshot testing approach. It helps validate that your Smart Enums behave consistently and serialize correctly.

## Installation

Add the package to your project:

```bash
dotnet add package Enigmatry.Entry.SmartEnums.VerifyTests
```

## Usage Example

```csharp
using Enigmatry.Entry.SmartEnums;
using Enigmatry.Entry.SmartEnums.VerifyTests;
using System.Reflection;
using Argon;
using NUnit.Framework;
using VerifyNUnit;

namespace YourProject.Tests
{
    [TestFixture]
    public class ProductCategoryTests
    {
        private VerifySettings verifySettings;
        
        [SetUp]
        public void Setup()
        {
            // Create and configure VerifySettings for tests
            verifySettings = new VerifySettings();
            
            // Register SmartEnum converters for the Argon JSON serializer used by VerifyTests
            var converters = new List<JsonConverter>();
            converters.EntryAddSmartEnumJsonConverters(new[] { Assembly.GetExecutingAssembly() });
            
            // Add the converters to the serialization pipeline
            verifySettings.UseJsonSerializerSettings(settings => 
            {
                foreach (var converter in converters)
                {
                    settings.Converters.Add(converter);
                }
            });
        }        
        
        [Test]
        public Task Should_Verify_All_ProductCategories()
        {
            // Arrange & Act
            var allCategories = ProductCategory.List();
            
            // Assert
            // The SmartEnumWriteOnlyJsonConverter will serialize each SmartEnum using its Name property
            return Verifier.Verify(allCategories, verifySettings);
        }
        
        [Test]
        public Task Should_Verify_ProductCategory_Properties()
        {
            // Arrange & Act
            var electronics = ProductCategory.Electronics;
            
            // Assert
            return Verifier.Verify(electronics, verifySettings);
        }
        
        [Test]
        public Task Should_Serialize_SmartEnum_With_Custom_Properties()
        {
            // Arrange
            var category = ProductCategory.Electronics;
            
            // Act - Create a test object with the SmartEnum
            var testObject = new 
            {
                Category = category,
                CategoryName = category.Name,
                CategoryId = category.Value,
                DisplayName = category.DisplayName,
                IsDigital = category.IsDigitalProduct
            };
            
            // Assert
            // The SmartEnumWriteOnlyJsonConverter will handle proper serialization
            return Verifier.Verify(testObject, verifySettings);
        }
    }
    
    // Example Smart Enum based on typical Enigmatry.Entry.SmartEnums pattern
    public class ProductCategory : SmartEnum<ProductCategory>
    {
        public static readonly ProductCategory Electronics = new(1, nameof(Electronics), "Electronics & Tech", true);
        public static readonly ProductCategory Clothing = new(2, nameof(Clothing), "Apparel & Fashion", false);
        public static readonly ProductCategory Books = new(3, nameof(Books), "Books & Media", false);
        public static readonly ProductCategory DigitalContent = new(4, nameof(DigitalContent), "Digital Downloads", true);

        public string DisplayName { get; }
        public bool IsDigitalProduct { get; }

        private ProductCategory(int id, string name, string displayName, bool isDigitalProduct) 
            : base(id, name)
        {
            DisplayName = displayName;
            IsDigitalProduct = isDigitalProduct;
        }
    }
}
```
