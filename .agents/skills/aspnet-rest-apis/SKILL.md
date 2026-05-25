---
name: aspnet-rest-apis
description: Enigmatry Entry Building Blocks .NET 10 Web API patterns covering MediatR, Autofac, FluentValidation, and vertical slice architecture. Use this when adding or modifying .NET API features, handlers, validators, or controllers.
---

# Entry Building Blocks .NET API Patterns

## Feature folder structure

Queries live in `Api/Features/{Feature}/`, commands and domain logic in `Domain/{Feature}/Commands/`:

```
Api/Features/Products/
  GetProducts.cs              // list query — static class
  GetProductDetails.cs        // detail query — static class
  ProductsController.cs       // thin — only mediator.Send()

Domain/Products/Commands/
  ProductCreateOrUpdate.cs              // static class: Command, Result, Validator
  ProductCreateOrUpdateCommandHandler.cs // separate file
  RemoveProduct.cs
```

## Queries — static class in Api/Features/

```csharp
public static class GetProductDetails
{
    [PublicAPI]
    public class Request : IQuery<Response>
    {
        public Guid Id { get; set; }
    }

    [PublicAPI]
    public class Response
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = String.Empty;
        public ProductType Type { get; set; }
    }

    [UsedImplicitly]
    public class RequestHandler(IRepository<Product> productRepository)
        : IRequestHandler<Request, Response>
    {
        public async Task<Response> Handle(Request request, CancellationToken cancellationToken)
        {
            var product = await productRepository.FindByIdAsync(request.Id)
                ?? throw new EntityNotFoundException(nameof(Product), request.Id.ToString());
            return new Response { Id = product.Id, Name = product.Name, Type = product.Type };
        }
    }
}
```

For **list queries**, use `PagedRequest` — the handler is a plain `IRequestHandler<Request, PagedResponse<T>>`:

```csharp
public static class GetProducts
{
    [PublicAPI]
    public class Request : PagedRequest<Response.Item>, IQuery<PagedResponse<Response.Item>>
    {
        public string? Name { get; set; }
    }

    [PublicAPI]
    public static class Response
    {
        [PublicAPI]
        public class Item
        {
            public Guid Id { get; set; }
            public string Name { get; set; } = String.Empty;
        }
    }

    [UsedImplicitly]
    public class RequestHandler(IRepository<Product> productRepository)
        : IRequestHandler<Request, PagedResponse<Response.Item>>
    {
        public async Task<PagedResponse<Response.Item>> Handle(Request request, CancellationToken cancellationToken) =>
            await productRepository.QueryAll()
                .WhereIf(!string.IsNullOrEmpty(request.Name), p => p.Name.Contains(request.Name!))
                .Select(p => new Response.Item { Id = p.Id, Name = p.Name })
                .ToPagedResponseAsync(request, cancellationToken);
    }
}
```

## Commands — static class in Domain/{Feature}/Commands/

Commands implement `ICommand` (void) or `ICommand<TResponse>` — never raw `IRequest<T>`:

```csharp
public static class ProductCreateOrUpdate
{
    [PublicAPI]
    public class Command : ICommand<Result>
    {
        public Guid? Id { get; set; }
        public string Name { get; set; } = String.Empty;
        public ProductType Type { get; set; }
    }

    [PublicAPI]
    public class Result
    {
        public Guid Id { get; set; }
    }

    [UsedImplicitly]
    public class Validator : AbstractValidator<Command>
    {
        public Validator()
        {
            RuleFor(x => x.Name).NotEmpty().MaximumLength(Product.NameMaxLength);
            When(x => x.Type is ProductType.Food, () =>
            {
                RuleFor(x => x.ExpiresOn).NotNull();
            });
        }
    }
}
```

The **command handler** lives in a separate file in the same folder:

```csharp
[UsedImplicitly]
public class ProductCreateOrUpdateCommandHandler(IRepository<Product> productRepository)
    : IRequestHandler<ProductCreateOrUpdate.Command, ProductCreateOrUpdate.Result>
{
    public async Task<ProductCreateOrUpdate.Result> Handle(
        ProductCreateOrUpdate.Command request, CancellationToken cancellationToken)
    {
        Product product;
        if (request.Id.HasValue)
        {
            product = await productRepository.FindByIdAsync(request.Id.Value)
                ?? throw new EntityNotFoundException(nameof(Product), request.Id.Value.ToString());
            product.Update(request);
        }
        else
        {
            product = Product.Create(request);
            productRepository.Add(product);
        }

        return new ProductCreateOrUpdate.Result { Id = product.Id };
    }
}
```

## Domain entities

Entities inherit from `EntityWithGuidId` (auto-generates sequential GUIDs in the constructor). Use private setters, expose a `Create(Command)` factory and an `Update(Command)` method, and raise domain events via `AddDomainEvent()`. Domain events are `abstract record`s:

```csharp
public abstract record ProductDomainEvent(Product Product) : DomainEvent;
public record ProductCreatedDomainEvent(Product Product) : ProductDomainEvent(Product);
public record ProductUpdatedDomainEvent(Product Product) : ProductDomainEvent(Product);

public class Product : EntityWithGuidId
{
    public const int NameMaxLength = 200;

    public string Name { get; private set; } = String.Empty;
    public ProductStatus Status { get; private set; } = ProductStatus.Active;

    public static Product Create(ProductCreateOrUpdate.Command request)
    {
        var product = new Product { Name = request.Name, Status = ProductStatus.Active };
        product.AddDomainEvent(new ProductCreatedDomainEvent(product));
        return product;
    }

    public void Update(ProductCreateOrUpdate.Command request)
    {
        Name = request.Name;
        AddDomainEvent(new ProductUpdatedDomainEvent(this));
    }
}
```

## Controllers — always thin

```csharp
[Produces(MediaTypeNames.Application.Json)]
[Route("api/[controller]")]
public class ProductsController(IMediator mediator) : Controller
{
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [UserHasPermission(PermissionId.ProductsRead)]
    public async Task<ActionResult<PagedResponse<GetProducts.Response.Item>>> Search(
        [FromQuery] GetProducts.Request query)
    {
        var response = await mediator.Send(query);
        return response.ToActionResult();
    }

    [HttpPost]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [UserHasPermission(PermissionId.ProductsWrite)]
    public async Task<ActionResult<ProductCreateOrUpdate.Result>> Post(
        ProductCreateOrUpdate.Command command)
    {
        var result = await mediator.Send(command);
        return result;
    }
}
```

- Use primary-constructor injection for `IMediator`.
- Return `response.ToActionResult()` for queries (returns 404 when null, 200 otherwise); return result directly for commands.
- Decorate every endpoint with `[UserHasPermission(PermissionId.X)]`.
- Never put business logic in a controller.

## Autofac modules

Register services in Autofac modules, not via `IServiceCollection`:

```csharp
public class MyFeatureModule : Module
{
    protected override void Load(ContainerBuilder builder)
    {
        builder.RegisterType<MyService>().As<IMyService>().InstancePerLifetimeScope();
    }
}
```

## EF Core configurations

```csharp
public class ProductConfiguration : IEntityTypeConfiguration<Product>
{
    public void Configure(EntityTypeBuilder<Product> builder)
    {
        builder.Property(x => x.Name).IsRequired().HasMaxLength(Product.NameMaxLength);
        builder.Property(x => x.Status)
            .HasSentinel(ProductStatus.Active)
            .HasDefaultValue(ProductStatus.Active);
        builder.HasIndex(x => x.Code).IsUnique();
    }
}
```

## What NOT to do

- Do not put logic in controllers — use `IRequestHandler<T>`.
- Do not use `IRequest<T>` directly for commands — use `ICommand` or `ICommand<TResponse>`.
- Do not use `IRequest<T>` directly for queries — use `IQuery<TResponse>`.
- Do not use `services.AddSingleton/Scoped` for services — use Autofac modules.
- Do not add `!` null-forgiving operators.
- Do not use static `Log.Information(...)` — inject `ILogger<T>`.
- Do not create a command/query without a validator when it has user inputs.
- Do not add `Version=` to a `<PackageReference>` in a `.csproj` — all versions go in `Directory.Packages.props`.
- Do not omit `[PublicAPI]` on request/response/DTO types or `[UsedImplicitly]` on handlers and validators.
