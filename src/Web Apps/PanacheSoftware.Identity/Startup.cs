using System;
using System.Collections.Generic;
using AutoMapper;
using IdentityServer4.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using PanacheSoftware.Core.Domain.Configuration;
using PanacheSoftware.Core.Domain.Identity;
using PanacheSoftware.Core.Extensions;
using PanacheSoftware.Core.Types;
using PanacheSoftware.Http;
using PanacheSoftware.Identity.Data;
using PanacheSoftware.Identity.Manager;
using PanacheSoftware.Identity.Services;
using PanacheSoftware.UI.Core.Helpers;

namespace PanacheSoftware.Identity
{
    public class Startup
    {
        public Startup(IConfiguration configuration, IWebHostEnvironment environment)
        {
            Configuration = configuration;
            Environment = environment;
        }

        public IConfiguration Configuration { get; }
        public IWebHostEnvironment Environment { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            var panacheSoftwareConfiguration = new PanacheSoftwareConfiguration();
            Configuration.Bind("PanacheSoftware", panacheSoftwareConfiguration);

            switch (panacheSoftwareConfiguration.DBProvider)
            {
                case DBProvider.MySQL:
                    services.AddDbContext<ApplicationDbContext>(options =>
                        options.UseMySql(Configuration.GetConnectionString(DBProvider.MySQL), ServerVersion.AutoDetect(Configuration.GetConnectionString(DBProvider.MySQL))));
                    break;
                case DBProvider.MSSQL:
                    services.AddDbContext<ApplicationDbContext>(options =>
                        options.UseSqlServer(Configuration.GetConnectionString(DBProvider.MSSQL)));
                    break;
            }

            services.AddIdentity<ApplicationUser, ApplicationRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();

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


            var identityServerConfig = new Config(panacheSoftwareConfiguration);

            var builder = services.AddIdentityServer(options =>
                {
                    options.Events.RaiseErrorEvents = true;
                    options.Events.RaiseInformationEvents = true;
                    options.Events.RaiseFailureEvents = true;
                    options.Events.RaiseSuccessEvents = true;
                    options.InputLengthRestrictions.Scope = 1000;
                })
                .AddConfigurationStore(options =>
                {
                    switch (panacheSoftwareConfiguration.DBProvider)
                    {
                        case DBProvider.MySQL:
                            options.ConfigureDbContext = builder =>
                                builder.UseMySql(Configuration.GetConnectionString(DBProvider.MySQL), ServerVersion.AutoDetect(Configuration.GetConnectionString(DBProvider.MySQL)), b => b.MigrationsAssembly(typeof(Startup).Assembly.FullName));
                            break;
                        case DBProvider.MSSQL:
                            options.ConfigureDbContext = builder =>
                                builder.UseSqlServer(Configuration.GetConnectionString(DBProvider.MSSQL), b => b.MigrationsAssembly(typeof(Startup).Assembly.FullName));
                            break;
                    }
                })
                .AddOperationalStore(options =>
                {
                    switch (panacheSoftwareConfiguration.DBProvider)
                    {
                        case DBProvider.MySQL:
                            options.ConfigureDbContext = builder =>
                                builder.UseMySql(Configuration.GetConnectionString(DBProvider.MySQL), ServerVersion.AutoDetect(Configuration.GetConnectionString(DBProvider.MySQL)), b => b.MigrationsAssembly(typeof(Startup).Assembly.FullName));
                            break;
                        case DBProvider.MSSQL:
                            options.ConfigureDbContext = builder =>
                                builder.UseSqlServer(Configuration.GetConnectionString(DBProvider.MSSQL), b => b.MigrationsAssembly(typeof(Startup).Assembly.FullName));
                            break;
                    }

                    options.EnableTokenCleanup = true;
                })
                .AddAspNetIdentity<ApplicationUser>();


            services.ConfigureNonBreakingSameSiteCookies();

            services.AddAutoMapper(System.Reflection.Assembly.Load("PanacheSoftware.Core"));

            if (Environment.IsDevelopment())
            {
                builder.AddDeveloperSigningCredential();
            }
            else
            {
                throw new Exception("need to configure key material");
            }

            services.AddAuthentication();

            services.AddLocalApiAuthentication();

            services.AddTransient<IApplicationUserManager, ApplicationUserManager>();
            services.AddTransient<IUserProvider, UserProvider>();
            services.AddTransient<IAPIHelper, APIHelper>();
            //services.AddScoped<APIModelValidate>();

            services.AddHostedService<MigrationHostedService>();

            services.AddControllersWithViews().AddNewtonsoftJson();

            //services.AddScoped<IProfileService, ProfileService>();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Panache Software Identity API v1", Version = "v1" });

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
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseSwagger();

            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Panache Software Identity API v1");
            });

            //app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseCookiePolicy();
            app.UseIdentityServer();
            //app.UseMvc(routes =>
            //{
            //    routes.MapRoute("default", "{controller=Home}/{action=Index}/{id?}");
            //});
            app.UseMvcWithDefaultRoute();
        }
    }
}
