using AutoMapper;
using Microsoft.AspNetCore.Mvc;

namespace Enigmatry.BuildingBlocks.AspNetCore
{
    public static class ActionResultExtensions
    {
        public static ActionResult<TDestination> ToActionResult<TDestination>(this TDestination? model) where TDestination : class =>
            model == null
                ? (ActionResult<TDestination>)new NotFoundResult()
                : new OkObjectResult(model);

        public static ActionResult<TDestination> MapToActionResult<TDestination>(this IMapper mapper, object? value) =>
            value == null ? (ActionResult<TDestination>)new NotFoundResult() :
                new OkObjectResult(mapper.Map<TDestination>(value));
    }
}
