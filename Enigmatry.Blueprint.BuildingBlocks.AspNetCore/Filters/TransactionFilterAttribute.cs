using System.Threading.Tasks;
using Enigmatry.Blueprint.BuildingBlocks.Core.Data;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Enigmatry.Blueprint.BuildingBlocks.AspNetCore.Filters
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
