using FluentValidation;
using JetBrains.Annotations;
using MediatR;

namespace Enigmatry.Entry.MediatR.Tests.Samples;

public class RequestWithoutResponse : IRequest
{
    public string Name { get; set; } = string.Empty;

    [UsedImplicitly]
    public class Validator : AbstractValidator<RequestWithoutResponse>
    {
        public Validator()
        {
            // minimum validation rule just to see if validation works
            RuleFor(x => x.Name).NotEmpty();
        }
    }
}
