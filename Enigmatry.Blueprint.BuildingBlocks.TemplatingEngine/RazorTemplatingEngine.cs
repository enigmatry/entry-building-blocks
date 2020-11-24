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
using System.IO;
using System.Threading.Tasks;

namespace Enigmatry.Blueprint.BuildingBlocks.TemplatingEngine
{
    public class RazorTemplatingEngine : ITemplatingEngine
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ITempDataProvider _tempDataProvider;
        private readonly IRazorViewEngine _viewEngine;

        public RazorTemplatingEngine(
            IRazorViewEngine viewEngine,
            ITempDataProvider tempDataProvider,
            IServiceProvider serviceProvider)
        {
            _viewEngine = viewEngine;
            _tempDataProvider = tempDataProvider;
            _serviceProvider = serviceProvider;
        }

        public async Task<string> RenderFromFileAsync<TModel>(string path, TModel model)
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
