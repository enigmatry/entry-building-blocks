# CSV Processing Library

A library that provides functionality for reading and writing CSV files, with support for complex mapping, validation, and transformation.

## Intended Usage

Use this library when you need to import or export data in CSV format, with strong typing and customizable mapping between your domain models and CSV records.

## Installation

Add the package to your project:

```bash
dotnet add package Enigmatry.Entry.Csv
```

## Usage Examples

### Reading a CSV file:

```csharp
using System;
using System.IO;
using System.Collections.Generic;
using Enigmatry.Entry.Csv;

// Define your model
public class Person
{
    public string FirstName { get; set; } = string.Empty;
    
    [Name("Last Name")]
    public string LastName { get; set; } = string.Empty;
    
    [Name("Date of Birth")]
    public DateTime BirthDate { get; set; }
    
    [Name("Email Address")]
    public string Email { get; set; } = string.Empty;
    
    [Ignore]
    public string InternalId { get; set; } = string.Empty;
}

// Read CSV file
public List<Person> ReadPeopleFromCsv(string filePath)
{
    // Create CSV helper with options
    var csvHelper = new CsvHelper<Person>(options => 
    {
        options.WithCulture(System.Globalization.CultureInfo.InvariantCulture);
    });
    
    using var fileStream = File.OpenRead(filePath);
    var people = csvHelper.GetRecords(fileStream).ToList();
    
    return people;
}
```

### Writing a CSV file:

```csharp
using System;
using System.IO;
using System.Collections.Generic;
using System.Text;
using Enigmatry.Entry.Csv;

// Create some data
var people = new List<Person>
{
    new Person 
    { 
        FirstName = "John", 
        LastName = "Doe", 
        BirthDate = new DateTime(1980, 1, 1), 
        Email = "john@example.com",
        InternalId = "JD001" // This will be ignored in the CSV output because of the [Ignore] attribute
    },
    new Person 
    { 
        FirstName = "Jane", 
        LastName = "Smith", 
        BirthDate = new DateTime(1985, 5, 5), 
        Email = "jane@example.com",
        InternalId = "JS002" 
    }
};

// Write to CSV file
public void ExportPeopleToCsv(List<Person> people, string outputFilePath)
{
    var csvHelper = new CsvHelper<Person>(options => 
    {
        options.WithEncoding(Encoding.UTF8);
        options.WithCulture(System.Globalization.CultureInfo.InvariantCulture);
    });
    
    // Generate CSV data
    byte[] csvData = csvHelper.WriteRecords(people);
    
    // Save to file
    File.WriteAllBytes(outputFilePath, csvData);
}
```

## Additional Examples

### Working with DateTimeOffset values:

```csharp
using System;
using System.Collections.Generic;
using Enigmatry.Entry.Csv;

public class Event
{
    public string Name { get; set; } = string.Empty;
    
    [Name("Event Time")]
    public DateTimeOffset EventTime { get; set; }
}

public void ProcessEventsCsv(string filePath)
{
    // The built-in DateTimeOffsetToLocalDateTimeConverter will automatically handle converting 
    // DateTimeOffset values to local date-time strings when writing CSV data
    var csvHelper = new CsvHelper<Event>(options => 
    {
        options.WithCulture(System.Globalization.CultureInfo.InvariantCulture);
    });
    
    // Example writing events with DateTimeOffset
    var events = new List<Event>
    {
        new Event { Name = "Conference", EventTime = DateTimeOffset.Now },
        new Event { Name = "Workshop", EventTime = DateTimeOffset.Now.AddDays(1) }
    };
    
    byte[] csvData = csvHelper.WriteRecords(events);
    
    // CSV will show local dates in "yyyy-MM-dd HH:mm:ss" format
}
```
