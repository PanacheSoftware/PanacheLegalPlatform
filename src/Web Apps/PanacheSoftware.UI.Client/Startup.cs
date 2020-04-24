using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using PanacheSoftware.Core.Types;
using PanacheSoftware.Http;
using PanacheSoftware.UI.Core.Helpers;

namespace PanacheSoftware.UI.Client
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });

            services.AddMvc(options =>
            {
                options.EnableEndpointRouting = false;
            })
                .SetCompatibilityVersion(CompatibilityVersion.Version_3_0)
                //.AddRazorPagesOptions(options =>
                //{
                //    options.AllowAreas = false;
                //})
                .ConfigureApiBehaviorOptions(options =>
                {
                    options.SuppressModelStateInvalidFilter = true;
                    //options.SuppressUseValidationProblemDetailsForInvalidModelStateResponses = true;
                });

            services.AddPanaceSoftwareResources();

            JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();

            services.AddAuthentication(options =>
            {
                options.DefaultScheme = "Cookies";
                options.DefaultChallengeScheme = "oidc";
            })
                .AddCookie("Cookies")
                .AddOpenIdConnect("oidc", options =>
                {
                    options.SignInScheme = "Cookies";
                    options.Authority = "https://localhost:44397/";
                    options.RequireHttpsMetadata = false;

                    options.ClientId = PanacheSoftwareScopeNames.ClientUI;
                    options.ClientSecret = "49C1A7E1-0C79-4A89-A3D6-A37998FB86B0";
                    options.ResponseType = "code id_token";

                    options.SaveTokens = true;
                    options.GetClaimsFromUserInfoEndpoint = true;

                    options.Scope.Add("openid");
                    options.Scope.Add("offline_access");
                    options.Scope.Add("email");
                    options.Scope.Add("IdentityServerApi");
                    options.Scope.Add(PanacheSoftwareScopeNames.IdentityResourceProfile);
                    options.Scope.Add(PanacheSoftwareScopeNames.ClientService);
                    options.Scope.Add(PanacheSoftwareScopeNames.TeamService);
                    options.Scope.Add(PanacheSoftwareScopeNames.FolderService);
                    options.Scope.Add(PanacheSoftwareScopeNames.FoundationService);
                    options.Scope.Add(PanacheSoftwareScopeNames.TaskService);

                    options.ClaimActions.MapUniqueJsonKey("tenantid", "tenantid");
                });

            services.AddSingleton<IAPIHelper, APIHelper>();
            services.AddTransient<IRazorPartialToStringRenderer, RazorPartialToStringRenderer>();
            services.AddTransient<IModelHelper, ModelHelper>();

            services.AddAutoMapper(System.Reflection.Assembly.Load("PanacheSoftware.Core"));
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseAuthentication();

            app.UseStaticFiles();
            app.UseCookiePolicy();

            //app.UseMvc();
            app.UseMvcWithDefaultRoute();

            //app.UseStaticFiles();
            //app.UseCookiePolicy();

            //app.UseRouting();

            //app.UseEndpoints(endpoints =>
            //{
            //    endpoints.MapRazorPages();
            //});
        }
    }
}