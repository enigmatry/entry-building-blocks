# Azure Search Integration

A library that provides integration with Azure Cognitive Search, enabling full-text search capabilities in your applications.

## Intended Usage

Use this library when you need to implement advanced search functionality using Azure Cognitive Search, including indexing, searching, filtering, faceting, and suggestions.

## Installation

Add the package to your project:

```bash
dotnet add package Enigmatry.Entry.AzureSearch
```

## Usage Example

Configuring a searchable model:

```csharp
using Azure.Search.Documents.Indexes;
using Azure.Search.Documents.Indexes.Models;

// Define a searchable model
public class ProductSearchModel
{
    [SimpleField(IsKey = true)]
    public string Id { get; set; } = string.Empty;
    
    [SearchableField(IsSortable = true)]
    public string Name { get; set; } = string.Empty;
    
    [SearchableField]
    public string Description { get; set; } = string.Empty;
    
    [SimpleField(IsSortable = true, IsFacetable = true, IsFilterable = true)]
    public decimal Price { get; set; }
    
    [SimpleField(IsFacetable = true, IsFilterable = true)]
    public string Category { get; set; } = string.Empty;
    
    [SimpleField(IsFilterable = true)]
    public bool IsAvailable { get; set; }
}
```

Using the search service:

```csharp
using System.Threading.Tasks;
using System.Collections.Generic;
using Azure.Search.Documents.Models;
using Enigmatry.Entry.AzureSearch;
using Enigmatry.Entry.AzureSearch.Abstractions;

public class ProductSearchService
{
    private readonly ISearchService<ProductSearchModel> _searchService;
    
    public ProductSearchService(ISearchService<ProductSearchModel> searchService)
    {
        _searchService = searchService;
    }
    
    public async Task UpdateProductAsync(ProductSearchModel product)
    {
        // Add/Update a single document to the index
        await _searchService.UpdateDocument(product);
    }
    
    public async Task UpdateProductsAsync(IEnumerable<ProductSearchModel> products)
    {
        // Add/Update multiple documents to the index
        await _searchService.UpdateDocuments(products);
    }
    
    public async Task<SearchResponse<ProductSearchModel>> SearchProductsAsync(string searchText)
    {
        // Create search options using the Azure SDK
        var options = new Azure.Search.Documents.SearchOptions
        {
            Filter = "IsAvailable eq true",
            OrderBy = { "Price asc" },
            IncludeFacets = true,
            Facets = { "Category" },
            Size = 10
        };
        
        // Use SearchText to properly format and escape search terms
        var formattedSearchText = SearchText.AsEscaped(searchText);
        
        // Execute the search and get results
        return await _searchService.Search(formattedSearchText, options);
    }
    
    public async Task<SearchResponse<ProductSearchModel>> SearchProductsWithPhraseAsync(string searchText)
    {
        // Use phrase search for exact matching (wrapped in quotes)
        var phraseSearchText = SearchText.AsPhraseSearch(searchText);
        
        return await _searchService.Search(phraseSearchText);
    }
}
```

## Configuration Example

```json
{
  "SearchSettings": {
    "SearchServiceEndPoint": "https://myapp-search.search.windows.net",
    "ApiKey": "your-azure-search-api-key"
  }
}
```

## Dependency Injection Example

```csharp
using Enigmatry.Entry.AzureSearch.Extensions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;

public class Startup
{
    public void ConfigureServices(IServiceCollection services, IConfiguration configuration)
    {
        // Add Azure Search core services
        services.AddAzureSearch(configuration.GetSection("SearchSettings"))
            // Register a document type with a specific index name
            .AddDocument<ProductSearchModel>("products");
            
        // Alternatively, configure settings in code
        // services.AddAzureSearch(options => {
        //     options.SearchServiceEndPoint = new Uri("https://myapp-search.search.windows.net");
        //     options.ApiKey = "your-azure-search-api-key";
        // }).AddDocument<ProductSearchModel>();
    }
}
```
