using System.Threading.Tasks;
using Enigmatry.Entry.Core.Data;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Enigmatry.Entry.AspNetCore.Filters
{
    public sealed class TransactionFilterAttribute : ActionFilterAttribute
    {
        public override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            ActionExecutedContext resultContext = await next();

            var unitOfWork = context.HttpContext.Resolve<IUnitOfWork>();

            if (resultContext.Exception == null &&
                context.HttpContext.Response.StatusCode >= 200 &&
                context.HttpContext.Response.StatusCode < 300 &&
                context.ModelState.IsValid)
                await unitOfWork.SaveChangesAsync();
        }
    }
}
