using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using PanacheSoftware.Core.Domain.Configuration;
using PanacheSoftware.Core.Types;
using PanacheSoftware.Http;
using PanacheSoftware.Service.Client.Persistance;
using PanacheSoftware.Service.Foundation.Core;
using PanacheSoftware.Service.Foundation.Core.Repositories;
using PanacheSoftware.Service.Foundation.Persistance.Context;
using PanacheSoftware.Service.Foundation.Persistance.Repositories;
using System;
using System.Collections.Generic;
using System.IO;

namespace PanacheSoftware.Service.Foundation
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

            switch (panacheSoftwareConfiguration.DBProvider)
            {
                case DBProvider.MySQL:
                    services.AddDbContext<PanacheSoftwareServiceFoundationContext>(options =>
                        options.UseMySql(Configuration.GetConnectionString(DBProvider.MySQL), ServerVersion.AutoDetect(Configuration.GetConnectionString(DBProvider.MySQL))));
                    break;
                case DBProvider.MSSQL:
                    services.AddDbContext<PanacheSoftwareServiceFoundationContext>(options =>
                        options.UseSqlServer(Configuration.GetConnectionString(DBProvider.MSSQL)));
                    break;
            }

            services.AddMvc(options =>
            {
                options.EnableEndpointRouting = false;
            });
                //.SetCompatibilityVersion(CompatibilityVersion.Version_3_0);

            services.AddAuthorization();

            //services.AddAuthentication(IdentityServerAuthenticationDefaults.AuthenticationScheme)
            //    .AddIdentityServerAuthentication(options =>
            //    {
            //        // base-address of your identityserver
            //        options.Authority = bool.Parse(panacheSoftwareConfiguration.CallMethod.UICallsSecure) ? panacheSoftwareConfiguration.Url.IdentityServerURLSecure : panacheSoftwareConfiguration.Url.IdentityServerURL;

            //        // name of the API resource
            //        options.ApiName = PanacheSoftwareScopeNames.FoundationService;
            //        options.ApiSecret = panacheSoftwareConfiguration.Secret.FoundationServiceSecret;
            //        options.RequireHttpsMetadata = false;
            //        //options.EnableCaching = true;
            //    });

            Action<JwtBearerOptions> jwtOptions = o =>
            {
                o.Authority = bool.Parse(panacheSoftwareConfiguration.CallMethod.UICallsSecure) ? panacheSoftwareConfiguration.Url.IdentityServerURLSecure : panacheSoftwareConfiguration.Url.IdentityServerURL;
                o.Audience = PanacheSoftwareScopeNames.FoundationService;
                o.RequireHttpsMetadata = false;
                o.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateAudience = true,
                };
            };

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(jwtOptions);

            services.AddTransient<IUnitOfWork, UnitOfWork>();
            services.AddTransient<ILanguageHeaderRepository, LanguageHeaderRepository>();
            services.AddTransient<ILanguageCodeRepository, LanguageCodeRepository>();
            services.AddTransient<ILanguageItemRepository, LanguageItemRepository>();

            services.AddTransient<ISettingHeaderRepository, SettingHeaderRepository>();
            services.AddTransient<IUserSettingRepository, UserSettingRepository>();

            services.AddTransient<IUserProvider, UserProvider>();
            services.AddTransient<IStaticFileReader, StaticFileReader>();
            services.AddTransient<IHttpContextAccessor, HttpContextAccessor>();

            services.AddHostedService<MigrationHostedService>();

            services.AddAutoMapper(System.Reflection.Assembly.Load("PanacheSoftware.Core"));

            services.AddControllersWithViews().AddNewtonsoftJson();

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Panache Software Foundation API v1", Version = "v1" });

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
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseSwagger();

            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Panache Software Foundation API v1");
            });

            app.UseStaticFiles(new StaticFileOptions()
            {
                FileProvider = new PhysicalFileProvider(
                            Path.Combine(Directory.GetCurrentDirectory(), @"Data")),
                RequestPath = new PathString("/app-data")
            });

            app.UseAuthentication();
            //app.UseHttpsRedirection();
            app.UseMvc();
        }
    }
}
