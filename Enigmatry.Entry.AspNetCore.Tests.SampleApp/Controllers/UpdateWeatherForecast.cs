using System;
using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using JetBrains.Annotations;
using MediatR;

namespace Enigmatry.Entry.AspNetCore.Tests.SampleApp.Controllers;

public static class UpdateWeatherForecast
{
    [PublicAPI]
    public class Request : IRequest<Response>
    {
        private Guid? GuidProperty { get; set; }
        public string? StringProperty { get; set; }
        public int? IntProperty { get; set; }

        [UsedImplicitly]
        public class Validator : AbstractValidator<Request>
        {
            public Validator()
            {
                RuleFor(x => x.GuidProperty).NotEmpty();
                RuleFor(x => x.StringProperty).NotEmpty();
                RuleFor(x => x.IntProperty).NotEmpty();
            }
        }
    }

    public class Response
    {
    }

    [UsedImplicitly]
    public class RequestHandler : IRequestHandler<Request, Response>
    {
        public Task<Response> Handle(Request request, CancellationToken cancellationToken) =>
            Task.FromResult(new Response());
    }
}
