using AutoMapper;
using IdentityServer4.AccessTokenValidation;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using PanacheSoftware.Core.Types;
using PanacheSoftware.Http;
using PanacheSoftware.Service.Folder.Core;
using PanacheSoftware.Service.Folder.Core.Repositories;
using PanacheSoftware.Service.Folder.Manager;
using PanacheSoftware.Service.Folder.Persistance;
using PanacheSoftware.Service.Folder.Persistance.Context;
using PanacheSoftware.Service.Folder.Persistance.Repositories.Folder;

namespace PanacheSoftware.Service.Folder
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
            string connectionString = Configuration.GetConnectionString("DefaultConnection");

            services.AddDbContext<PanacheSoftwareServiceFolderContext>(options => options.UseSqlServer(connectionString));

            services.AddAuthorization();

            services.AddAuthentication(IdentityServerAuthenticationDefaults.AuthenticationScheme)
                .AddIdentityServerAuthentication(options =>
                {
                    // base-address of your identityserver
                    options.Authority = "https://localhost:44397/";

                    // name of the API resource
                    options.ApiName = PanacheSoftwareScopeNames.FolderService;
                    options.ApiSecret = "22357BD0-32D6-429F-9A75-322046EE3FA2";
                    options.RequireHttpsMetadata = false;
                    options.EnableCaching = true;
                });

            //services.AddAuthentication("Bearer")
            //    .AddJwtBearer("Bearer", options =>
            //    {
            //        options.Authority = "https://localhost:44397/";
            //        options.RequireHttpsMetadata = false;

            //        options.Audience = PanacheSoftwareScopeNames.FolderService;
            //    });

            services.AddMvc(options =>
            {
                options.EnableEndpointRouting = false;
            })
                .SetCompatibilityVersion(CompatibilityVersion.Version_3_0);

            services.AddTransient<IUnitOfWork, UnitOfWork>();
            services.AddTransient<IFolderDetailRepository, FolderDetailRepository>();
            services.AddTransient<IFolderHeaderRepository, FolderHeaderRepository>();
            services.AddTransient<IFolderNodeRepository, FolderNodeRepository>();
            services.AddTransient<IFolderManager, FolderManager>();

            services.AddTransient<IUserProvider, UserProvider>();
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            services.AddAutoMapper(System.Reflection.Assembly.Load("PanacheSoftware.Core"));

            services.AddControllersWithViews().AddNewtonsoftJson();
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

            app.UseAuthentication();
            app.UseHttpsRedirection();
            app.UseMvc();
        }
    }
}
