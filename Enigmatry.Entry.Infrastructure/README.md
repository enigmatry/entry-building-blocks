# Infrastructure Library

A library that provides core infrastructure components and utilities for building .NET applications.

## Intended Usage

Use this library to access common infrastructure patterns, abstractions, and implementations for services like caching, background processing, messaging, and more.

## Installation

Add the package to your project:

```bash
dotnet add package Enigmatry.Entry.Infrastructure
```

## Usage Examples

### Using the TimeProvider:

```csharp
using Enigmatry.Entry.Core;
using Enigmatry.Entry.Infrastructure;

public class ReportGenerator
{
    private readonly ITimeProvider _timeProvider;
    
    public ReportGenerator(ITimeProvider timeProvider)
    {
        _timeProvider = timeProvider;
    }
    
    public Report GenerateDailyReport()
    {
        // Get current time in UTC
        DateTimeOffset now = _timeProvider.UtcNow;
        
        // Report is for the previous day
        DateTimeOffset reportDate = now.AddDays(-1).Date;
        
        var report = new Report
        {
            GeneratedAt = now,
            ReportDate = reportDate,
            Title = $"Daily Report for {reportDate:yyyy-MM-dd}"
        };
        
        return report;
    }
    
    public bool IsReportingPeriodActive()
    {
        // Using FixedUtcNow ensures the same timestamp is used for all checks in this request
        var fixedTime = _timeProvider.FixedUtcNow;
        
        // Reporting is active between 1AM and 5AM UTC
        var hour = fixedTime.Hour;
        return hour >= 1 && hour < 5;
    }
}
```
