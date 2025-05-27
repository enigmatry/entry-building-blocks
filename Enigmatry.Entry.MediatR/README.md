# MediatR Extensions

A library that provides extensions and utilities for working with MediatR, enhancing the mediator pattern implementation with additional features.

## Intended Usage

Use this library when you want to extend MediatR with additional behaviors, such as validation, logging, performance monitoring, or transactional support.

## Installation

Add the package to your project:

```bash
dotnet add package Enigmatry.Entry.MediatR
```

## Usage Example

```csharp
using Enigmatry.Entry.MediatR;
using MediatR;

// Creating a command with validation
public class CreateUserCommand : IRequest<int>, IValidatable
{
    public string Username { get; set; }
    public string Email { get; set; }
    
    public ValidationResult Validate()
    {
        var result = new ValidationResult();
        
        if (string.IsNullOrEmpty(Username))
            result.AddError("Username is required");
            
        if (string.IsNullOrEmpty(Email) || !Email.Contains("@"))
            result.AddError("Valid email is required");
            
        return result;
    }
}

// Command handler
public class CreateUserCommandHandler : IRequestHandler<CreateUserCommand, int>
{
    public async Task<int> Handle(CreateUserCommand request, CancellationToken cancellationToken)
    {
        // Implementation
        return userId;
    }
}
```

## Dependency Injection Example

Register MediatR with the extension behaviors:

```csharp
using Enigmatry.Entry.MediatR;

public class Startup
{
    public void ConfigureServices(IServiceCollection services)
    {
        // Add MediatR
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(Startup).Assembly));
        
        // Add validation behavior
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
        
        // Add logging behavior
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(LoggingBehavior<,>));
    }
}
```
