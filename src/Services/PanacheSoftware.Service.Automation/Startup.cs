using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using PanacheSoftware.Core.Domain.Configuration;
using PanacheSoftware.Core.Types;
using PanacheSoftware.Http;
using PanacheSoftware.Service.Automation.Manager;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PanacheSoftware.Service.Automation
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

            var panacheSoftwareConfiguration = new PanacheSoftwareConfiguration();
            Configuration.Bind("PanacheSoftware", panacheSoftwareConfiguration);

            services.AddAuthorization();

            //services.AddAuthentication(IdentityServerAuthenticationDefaults.AuthenticationScheme)
            //    .AddIdentityServerAuthentication(options =>
            //    {
            //        // base-address of your identityserver
            //        options.Authority = bool.Parse(panacheSoftwareConfiguration.CallMethod.UICallsSecure) ? panacheSoftwareConfiguration.Url.IdentityServerURLSecure : panacheSoftwareConfiguration.Url.IdentityServerURL;

            //        // name of the API resource
            //        options.ApiName = PanacheSoftwareScopeNames.TaskService;
            //        options.ApiSecret = panacheSoftwareConfiguration.Secret.TaskServiceSecret;
            //        options.RequireHttpsMetadata = false;
            //        //options.EnableCaching = true;
            //    });

            Action<JwtBearerOptions> jwtOptions = o =>
            {
                o.Authority = bool.Parse(panacheSoftwareConfiguration.CallMethod.UICallsSecure) ? panacheSoftwareConfiguration.Url.IdentityServerURLSecure : panacheSoftwareConfiguration.Url.IdentityServerURL;
                o.Audience = PanacheSoftwareScopeNames.AutomationService;
                o.RequireHttpsMetadata = false;
                o.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateAudience = true,
                };
            };

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(jwtOptions);

            services.AddMvc(options =>
            {
                options.EnableEndpointRouting = false;
            });
                //.SetCompatibilityVersion(CompatibilityVersion.Version_3_0);

            services.AddTransient<IUserProvider, UserProvider>();
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddSingleton<IAPIHelper, APIHelper>();
            services.AddTransient<IDocumentManager, DocumentManager>();

            //services.AddHostedService<MigrationHostedService>();

            services.AddAutoMapper(System.Reflection.Assembly.Load("PanacheSoftware.Core"));

            services.AddControllersWithViews().AddNewtonsoftJson();

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Panache Software Automation API v1", Version = "v1" });

                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Description = "JWT Authorization header using the Bearer scheme. \r\n\r\n Enter 'Bearer' [space] and then your token in the text input below.\r\n\r\nExample: \"Bearer 12345abcdef\"",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer"
                });

                c.AddSecurityRequirement(new OpenApiSecurityRequirement()
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            },
                            Scheme = "oauth2",
                            Name = "Bearer",
                            In = ParameterLocation.Header,

                        },
                        new List<string>()
                    }
                });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();              
            }
            else
            {
                app.UseHsts();
            }

            app.UseSwagger();
            app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Panache Software Automation API v1"));

            app.UseAuthentication();
            app.UseMvc();
        }
    }
}
