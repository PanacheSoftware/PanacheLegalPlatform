using IdentityServer4.AccessTokenValidation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Ocelot.Administration;
using Ocelot.DependencyInjection;
using Ocelot.Middleware;
using PanacheSoftware.Core.Domain.Configuration;
using PanacheSoftware.Core.Types;
using System;

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

            var panacheSoftwareConfiguration = new PanacheSoftwareConfiguration();
            Configuration.Bind("PanacheSoftware", panacheSoftwareConfiguration);

            var authenticationProviderKey = "GatewayKey";
            Action<IdentityServerAuthenticationOptions> options = o =>
            {
                o.Authority = bool.Parse(panacheSoftwareConfiguration.CallMethod.UICallsSecure) ? panacheSoftwareConfiguration.Url.IdentityServerURLSecure : panacheSoftwareConfiguration.Url.IdentityServerURL;
                o.ApiName = PanacheSoftwareScopeNames.APIGateway;
                o.SupportedTokens = SupportedTokens.Both;
                o.ApiSecret = panacheSoftwareConfiguration.Secret.APIGatewaySecret;
                o.RequireHttpsMetadata = false;
            };

            Action<JwtBearerOptions> jwtOptions = o =>
            {
                o.Authority = bool.Parse(panacheSoftwareConfiguration.CallMethod.UICallsSecure) ? panacheSoftwareConfiguration.Url.IdentityServerURLSecure : panacheSoftwareConfiguration.Url.IdentityServerURL;
                o.Audience = PanacheSoftwareScopeNames.APIGateway;
                o.RequireHttpsMetadata = false;
                o.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateAudience = true,
                };
            };

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                //.AddJwtBearer(jwtOptions);
                .AddIdentityServerAuthentication(authenticationProviderKey, options);

            services.AddControllers();
            services
                .AddOcelot(Configuration)
                .AddAdministration("/administration", jwtOptions);
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

            app.UseAuthentication().UseOcelot().Wait();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            
        }
    }
}
