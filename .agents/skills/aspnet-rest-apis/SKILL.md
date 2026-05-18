---
name: aspnet-rest-apis
description: Enigmatry Entry Blueprint .NET 9 Web API patterns covering MediatR, Autofac, FluentValidation, and vertical slice architecture. Use this when adding or modifying .NET API features, handlers, validators, or controllers.
---

# Blueprint .NET API Patterns

## Feature folder structure

Queries live in `Api/Features/{Feature}/`, commands and domain logic in `Domain/{Feature}/Commands/`:

```
Api/Features/Products/
  GetProducts.cs              // list query — static class
  GetProductDetails.cs        // detail query — static class
  IsProductCodeUnique.cs      // custom query — static class
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
    public class MappingProfile : Profile
    {
        public MappingProfile() => CreateMap<Product, Response>();
    }

    [UsedImplicitly]
    public class RequestHandler(IRepository<Product> productRepository, IMapper mapper)
        : IRequestHandler<Request, Response>
    {
        public async Task<Response> Handle(Request request, CancellationToken cancellationToken)
        {
            var response = await productRepository.QueryAll()
                .QueryById(request.Id)
                .SingleOrDefaultMappedAsync<Product, Response>(mapper, cancellationToken: cancellationToken);
            return response;
        }
    }
}
```

For **list queries**, use `PagedRequest` and `IPagedRequestHandler`:

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
        public class Item { public Guid Id { get; set; } ... }

        [UsedImplicitly]
        public class MappingProfile : Profile
        {
            public MappingProfile() => CreateMap<Product, Item>();
        }
    }

    [UsedImplicitly]
    public class RequestHandler : IPagedRequestHandler<Request, Response.Item>
    {
        public async Task<PagedResponse<Response.Item>> Handle(Request request, CancellationToken cancellationToken) =>
            await _productRepository.QueryAll()
                .ProjectTo<Response.Item>(_mapper.ConfigurationProvider, cancellationToken)
                .ToPagedResponseAsync(request, cancellationToken);
    }
}
```

## Commands — static class in Domain/{Feature}/Commands/

```csharp
public static class ProductCreateOrUpdate
{
    [PublicAPI]
    public class Command : IRequest<Result>
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
public class ProductCreateOrUpdateCommandHandler
    : IRequestHandler<ProductCreateOrUpdate.Command, ProductCreateOrUpdate.Result>
{
    private readonly IRepository<Product, Guid> _productRepository;

    public ProductCreateOrUpdateCommandHandler(IRepository<Product, Guid> productRepository)
    {
        _productRepository = productRepository;
    }

    public async Task<ProductCreateOrUpdate.Result> Handle(
        ProductCreateOrUpdate.Command request, CancellationToken cancellationToken)
    {
        Product result;
        if (request.Id.HasValue)
        {
            result = await _productRepository.FindByIdAsync(request.Id.Value)
                     ?? throw new InvalidOperationException("Could not find product by Id");
            result.Update(request);
        }
        else
        {
            result = Product.Create(request);
            _productRepository.Add(result);
        }

        return new ProductCreateOrUpdate.Result { Id = result.Id };
    }
}
```

## Domain entities

Entities inherit from `EntityWithCreatedUpdated`, use private setters, expose a `Create(Command)` factory and an `Update(Command)` method, and raise domain events via `AddDomainEvent()`:

```csharp
public class Product : EntityWithCreatedUpdated
{
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
- Return `response.ToActionResult()` for queries; return result directly for commands.
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

Any class whose name ends with `Service` is **auto-registered** by `ServiceModule` — no manual registration needed for those.

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
        builder.HasCreatedByAndUpdatedBy();
    }
}
```

## What NOT to do

- Do not put logic in controllers — use `IRequestHandler<T>`.
- Do not use `services.AddSingleton/Scoped` for services — use Autofac modules.
- Do not add `!` null-forgiving operators.
- Do not use static `Log.Information(...)` — inject `ILogger<T>`.
- Do not create a command/query without a validator when it has user inputs.
- Do not add `Version=` to a `<PackageReference>` in a `.csproj` — all versions go in `Directory.Packages.props`.
- Do not omit `[PublicAPI]` on request/response/DTO types or `[UsedImplicitly]` on handlers and validators.
