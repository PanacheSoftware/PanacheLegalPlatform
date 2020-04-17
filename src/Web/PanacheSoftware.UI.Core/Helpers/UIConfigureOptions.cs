using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Text;

namespace PanacheSoftware.UI.Core.Helpers
{
    internal class UIConfigureOptions : IPostConfigureOptions<StaticFileOptions>
    {
        private readonly IWebHostEnvironment _environment;

        public UIConfigureOptions(IWebHostEnvironment environment)
        {
            _environment = environment;
        }

        public void PostConfigure(string name, StaticFileOptions options)
        {
            options.ContentTypeProvider = options.ContentTypeProvider ?? new FileExtensionContentTypeProvider();

            if(options.FileProvider == null && _environment.WebRootFileProvider == null)
            {
                throw new InvalidOperationException("Missing FileProvider");
            }

            options.FileProvider = options.FileProvider ?? _environment.WebRootFileProvider;

            var filesProvider = new ManifestEmbeddedFileProvider(GetType().Assembly, "Resources");
            options.FileProvider = new CompositeFileProvider(options.FileProvider, filesProvider);
        }
    }

    public static class UIServiceCollectionExtensions
    {
        public static void AddPanaceSoftwareResources(this IServiceCollection services)
        {
            services.ConfigureOptions(typeof(UIConfigureOptions));
        }
    }
}
