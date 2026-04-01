using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Routing;

namespace Enigmatry.Entry.TemplatingEngine;

public class RazorTemplatingEngine : ITemplatingEngine
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ITempDataProvider _tempDataProvider;
    private readonly IRazorViewEngine _viewEngine;
    private readonly IModelMetadataProvider _modelMetadataProvider;
    private static readonly Dictionary<string, object> EmptyViewBagDictionary = [];

    public RazorTemplatingEngine(
        IRazorViewEngine viewEngine,
        ITempDataProvider tempDataProvider,
        IServiceProvider serviceProvider,
        IModelMetadataProvider modelMetadataProvider)
    {
        _viewEngine = viewEngine;
        _tempDataProvider = tempDataProvider;
        _serviceProvider = serviceProvider;
        _modelMetadataProvider = modelMetadataProvider;
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

        var view = viewEngineResult.View;

        await using var output = new StringWriter();

        var viewData = new ViewDataDictionary(
            _modelMetadataProvider,
            new ModelStateDictionary())
        {
            Model = model
        };

        foreach (var keyValuePair in viewBagDictionary)
        {
            viewData[keyValuePair.Key] = keyValuePair.Value;
        }

        var viewContext = new ViewContext(
            actionContext,
            view,
            viewData,
            new TempDataDictionary(actionContext.HttpContext, _tempDataProvider),
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
