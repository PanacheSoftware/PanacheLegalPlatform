using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IdentityServer4.AccessTokenValidation;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Ocelot.DependencyInjection;
using Ocelot.Middleware;
using PanacheSoftware.Core.Types;

namespace PanacheSoftware.API.Gateway
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            //services.AddAuthentication(IdentityServerAuthenticationDefaults.AuthenticationScheme)
            //    .AddIdentityServerAuthentication(options =>
            //    {
            //        // base-address of your identityserver
            //        options.Authority = "https://localhost:44397/";

            //        // name of the API resource
            //        options.ApiName = PanacheSoftwareScopeNames.APIGateway;
            //        options.ApiSecret = "DDDCB193-213C-43FB-967A-5A911D2EFC04";
            //        options.RequireHttpsMetadata = false;
            //        options.EnableCaching = true;
            //    });

            var authenticationProviderKey = "GatewayKey";
            Action<IdentityServerAuthenticationOptions> options = o =>
            {
                o.Authority = "https://localhost:44302/";
                o.ApiName = PanacheSoftwareScopeNames.APIGateway;
                o.SupportedTokens = SupportedTokens.Both;
                o.ApiSecret = "DDDCB193-213C-43FB-967A-5A911D2EFC04";
            };

            services.AddAuthentication()
                .AddIdentityServerAuthentication(authenticationProviderKey, options);

            services.AddControllers();
            services.AddOcelot(Configuration);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            //app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            app.UseAuthentication().UseOcelot().Wait();
        }
    }
}
