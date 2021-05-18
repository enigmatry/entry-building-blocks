using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Routing;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace Enigmatry.BuildingBlocks.TemplatingEngine
{
    public class RazorTemplatingEngine : ITemplatingEngine
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ITempDataProvider _tempDataProvider;
        private readonly IRazorViewEngine _viewEngine;
        private static readonly Dictionary<string, object> EmptyViewBagDictionary = new();

        public RazorTemplatingEngine(
            IRazorViewEngine viewEngine,
            ITempDataProvider tempDataProvider,
            IServiceProvider serviceProvider)
        {
            _viewEngine = viewEngine;
            _tempDataProvider = tempDataProvider;
            _serviceProvider = serviceProvider;
        }

        public Task<string> RenderFromFileAsync<TModel>(string path, TModel model)
            => RenderFromFileInternalAsync(path, model, EmptyViewBagDictionary);

        public Task<string> RenderFromFileAsync<TModel>(string path, TModel model, IDictionary<string, object> viewBagDictionary)
            => RenderFromFileInternalAsync(path, model, viewBagDictionary);


        private async Task<string> RenderFromFileInternalAsync<TModel>(string path, TModel model, IDictionary<string, object> viewBagDictionary)
        {
            var actionContext = GetActionContext();
            var viewEngineResult = _viewEngine.GetView(path, path, false);

            if (!viewEngineResult.Success)
            {
                throw new InvalidOperationException($"Couldn't find view '{path}'");
            }

            IView view = viewEngineResult.View;

            using var output = new StringWriter();
            var viewContext = new ViewContext(
                actionContext,
                view,
                new ViewDataDictionary<TModel>(
                    new EmptyModelMetadataProvider(),
                    new ModelStateDictionary())
                {
                    Model = model
                },
                new TempDataDictionary(
                    actionContext.HttpContext,
                    _tempDataProvider),
                output,
                new HtmlHelperOptions());

            foreach (var keyValuePair in viewBagDictionary)
            {
                viewContext.ViewData[keyValuePair.Key] = keyValuePair.Value;
            }

            await view.RenderAsync(viewContext);

            return output.ToString();
        }

        private ActionContext GetActionContext()
        {
            var httpContext = new DefaultHttpContext { RequestServices = _serviceProvider };
            return new ActionContext(httpContext, new RouteData(), new ActionDescriptor());
        }
    }
}
