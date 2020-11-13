using System;
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
