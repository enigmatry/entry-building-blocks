using Enigmatry.Blueprint.BuildingBlocks.Core.Data;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Enigmatry.Blueprint.BuildingBlocks.AspNetCore.Filters
{
    public sealed class CancelSavingTransactionAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context) => CancelSavingIfModelInvalid(context);

        public override void OnActionExecuted(ActionExecutedContext context) => CancelSavingIfModelInvalid(context);

        private static void CancelSavingIfModelInvalid(FilterContext context)
        {
            if (!context.ModelState.IsValid)
            {
                var unitOfWork = context.HttpContext.Resolve<IUnitOfWork>();
                unitOfWork.CancelSaving();
            }
        }
    }
}
