using FluentValidation;

namespace Enigmatry.Entry.Core.Paging;

public class PagedRequestValidator<TRequest, TResponse> : AbstractValidator<TRequest> where TRequest : PagedRequest<TResponse>
{
    private readonly IList<int> _pageSizeOptions = [20, 50, 100];

    public PagedRequestValidator(params int[] pageSizeOptions)
    {
        if (pageSizeOptions.Length != 0)
        {
            _pageSizeOptions = pageSizeOptions;
        }

        RuleFor(x => x.PageSize)
            .Must(_pageSizeOptions.Contains)
            .WithMessage($"Page size must be one of the following: {string.Join(", ", _pageSizeOptions)}");
    }
}
