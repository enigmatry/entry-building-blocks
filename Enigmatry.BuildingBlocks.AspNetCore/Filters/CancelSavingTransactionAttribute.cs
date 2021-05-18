using Enigmatry.BuildingBlocks.Core.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Enigmatry.BuildingBlocks.AspNetCore.Filters
{
    public sealed class CancelSavingTransactionAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context) => CancelSavingIfModelInvalid(context);

        public override void OnActionExecuted(ActionExecutedContext context) => CancelSavingIfModelInvalid(context);

        private static void CancelSavingIfModelInvalid(ActionContext context)
        {
            if (!context.ModelState.IsValid)
            {
                var unitOfWork = context.HttpContext.Resolve<IUnitOfWork>();
                unitOfWork.CancelSaving();
            }
        }
    }
}
