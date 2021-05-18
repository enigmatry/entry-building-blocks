using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace Enigmatry.BuildingBlocks.Localization
{
    public static class LocalizationStartupExtensions
    {
        public static void AppUseCultures(this IApplicationBuilder app)
        {
            //IList<CultureInfo> supportedCultures = new List<CultureInfo>
            //{
            //    new CultureInfo("en-US"),
            //    new CultureInfo("nl"),
            //    new CultureInfo("nl-NL"),
            //};
            //app.UseRequestLocalization(new RequestLocalizationOptions
            //{
            //    DefaultRequestCulture = new RequestCulture("en-US"),
            //    SupportedCultures = supportedCultures,
            //    SupportedUICultures = supportedCultures
            //});
        }

        public static void AppAddLocalization(this IServiceCollection services)
        {
            // https://joonasw.net/view/aspnet-core-localization-deep-dive
            //services.AddLocalization(options => { options.ResourcesPath = "Resources"; });
        }

        public static IMvcBuilder AppAddLocalization(this IMvcBuilder mvcBuilder)
        {
            throw new NotImplementedException();
            //return mvcBuilder.AddDataAnnotationsLocalization(options =>
            //{
            //    options.DataAnnotationLocalizerProvider = (type, factory) =>
            //        factory.Create(typeof(SharedResource));
            //});
        }
    }
}
