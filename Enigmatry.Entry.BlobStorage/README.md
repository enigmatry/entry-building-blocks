# Blob Storage Library

A library that provides abstraction and implementation for storing and retrieving blobs (binary large objects) using various storage providers.

## Intended Usage

Use this library when you need to store and manage binary data such as files, images, documents, or other media content.

## Installation

Add the package to your project:

```bash
dotnet add package Enigmatry.Entry.BlobStorage
```

## Usage Example

```csharp
using System.IO;
using System.Threading.Tasks;
using System.Collections.Generic;
using Enigmatry.Entry.BlobStorage;

public class DocumentService
{
    private readonly IBlobStorage _blobStorage;
    
    public DocumentService(IBlobStorage blobStorage)
    {
        _blobStorage = blobStorage;
    }
    
    public async Task<string> UploadDocumentAsync(Stream documentStream, string fileName)
    {
        // The relative path where the blob will be stored
        string relativePath = $"documents/{fileName}";
        
        // Upload the blob to storage
        await _blobStorage.AddAsync(relativePath, documentStream, override: true);
        
        // Return the full resource path
        return _blobStorage.BuildResourcePath(relativePath);
    }
      public async Task<Stream> GetDocumentAsync(string relativePath)
    {
        // Check if the blob exists
        if (!await _blobStorage.ExistsAsync(relativePath))
        {
            throw new FileNotFoundException($"Document not found: {relativePath}");
        }
        
        // Retrieve the blob content as a stream
        return await _blobStorage.GetAsync(relativePath);
    }
    
    public async Task<bool> DeleteDocumentAsync(string relativePath)
    {
        // Delete the blob and return whether it was successfully deleted
        return await _blobStorage.RemoveAsync(relativePath);
    }
    
    public async Task<IEnumerable<Models.BlobDetails>> ListDocumentsAsync()
    {
        // List all blobs in the documents directory
        return await _blobStorage.GetListAsync("documents/*");
    }
}
```

## Configuration Example

```json
{
  "App": {
    "AzureBlobStorage": {
      "AccountName": "mystorageaccount",
      "AccountKey": "your-secret-account-key",
      "SasDuration": "01:00:00",
      "FileSizeLimit": 10485760,
      "CacheTimeout": 3600
    }
  }
}
```

## Dependency Injection Example

```csharp
using Enigmatry.Entry.BlobStorage.Azure;
using Microsoft.Extensions.DependencyInjection;

public class Startup
{
    public void ConfigureServices(IServiceCollection services)
    {
        // Register a public Azure blob storage container
        services.AddEntryPublicAzBlobStorage("documents");
        
        // Or register a private Azure blob storage container
        services.AddEntryPrivateAzBlobStorage("private-documents");
    }
}
```
