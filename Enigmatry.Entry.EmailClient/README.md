# Email Client Library

A library that provides email functionality, supporting templating, attachments, and multiple email service providers.

## Intended Usage

Use this library to send emails from your application with support for HTML templates, attachments, and queue-based processing.

## Installation

Add the package to your project:

```bash
dotnet add package Enigmatry.Entry.Email
```

## Usage Example

```csharp
using System.Threading.Tasks;
using Enigmatry.Entry.Email;

public class UserService
{
    private readonly IEmailClient _emailClient;
    
    public UserService(IEmailClient emailClient)
    {
        _emailClient = emailClient;
    }
    
    public async Task SendWelcomeEmailAsync(string email, string username)
    {
        // Create an email message with recipient, subject and body
        var emailMessage = new EmailMessage(
            to: email,
            subject: "Welcome to Our Service",
            body: $"<h1>Welcome, {username}!</h1><p>Thank you for registering with our service.</p>"
        );
        
        // Send the email
        await _emailClient.SendAsync(emailMessage);
    }
    
    public async Task SendInvoiceEmailAsync(string email, string invoiceNumber)
    {
        // Create email message
        var emailMessage = new EmailMessage(
            to: email,
            subject: $"Invoice #{invoiceNumber}",
            body: $"<p>Please find your invoice #{invoiceNumber} attached to this email.</p>"
        );
        
        // Read the PDF file
        byte[] pdfData = await File.ReadAllBytesAsync($"invoices/Invoice-{invoiceNumber}.pdf");
        
        // Add attachment
        emailMessage.Attachments.Add(
            new EmailMessageAttachment(
                fileName: $"Invoice-{invoiceNumber}.pdf",
                data: pdfData,
                contentType: "application/pdf"
            )
        );
        
        // Send the email
        await _emailClient.SendAsync(emailMessage);
    }
}
```

## Configuration Example

```json
{
  "App": {
    "Smtp": {
      "Server": "smtp.example.com",
      "Port": 587,
      "Username": "username",
      "Password": "password",
      "From": "noreply@example.com",
      "UsePickupDirectory": false,
      "PickupDirectoryLocation": "C:\\Temp\\EmailPickup",
      "CatchAllAddress": "debug@example.com"
    }
  }
}
```

## Dependency Injection Example

```csharp
using Enigmatry.Entry.Email;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

public class Startup
{
    public void ConfigureServices(IServiceCollection services, IConfiguration configuration)
    {
        // Register email services
        services.AddEntryEmailClient(configuration);
    }
}
```
