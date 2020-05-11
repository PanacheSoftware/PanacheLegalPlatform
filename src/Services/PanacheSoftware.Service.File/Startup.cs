using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using IdentityServer4.AccessTokenValidation;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using PanacheSoftware.Core.Types;
using PanacheSoftware.Http;
using PanacheSoftware.Service.File.Core.Repositories;
using PanacheSoftware.Service.File.Manager;
using PanacheSoftware.Service.File.Persistance;
using PanacheSoftware.Service.File.Persistance.Context;
using PanacheSoftware.Service.File.Persistance.Repositories.File;

namespace PanacheSoftware.Service.File
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

            services.AddDbContext<PanacheSoftwareServiceFileContext>(options => options.UseSqlServer(connectionString));

            services.AddMvc(options =>
            {
                options.EnableEndpointRouting = false;
            })
                .SetCompatibilityVersion(CompatibilityVersion.Version_3_0);

            services.AddAuthorization();

            services.AddAuthentication(IdentityServerAuthenticationDefaults.AuthenticationScheme)
                .AddIdentityServerAuthentication(options =>
                {
                    // base-address of your identityserver
                    options.Authority = "https://localhost:44397/";

                    // name of the API resource
                    options.ApiName = PanacheSoftwareScopeNames.FileService;
                    options.ApiSecret = "839C649E-4FE3-410C-B43F-69C017A52676";
                    options.RequireHttpsMetadata = false;
                    options.EnableCaching = true;
                });

            //services.AddAuthentication("Bearer")
            //    .AddJwtBearer("Bearer", options =>
            //    {
            //        options.Authority = "https://localhost:44397/";
            //        options.RequireHttpsMetadata = false;

            //        options.Audience = PanacheSoftwareScopeNames.TeamService;
            //    });

            //services.AddDbContext<PanacheSoftwareServiceTeamContext>(options =>
            //    options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));

            services.AddTransient<IUnitOfWork, UnitOfWork>();
            services.AddTransient<IFileHeaderRepository, FileHeaderRepository>();
            services.AddTransient<IFileDetailRepository, FileDetailRepository>();
            services.AddTransient<IFileLinkRepository, FileLinkRepository>();
            services.AddTransient<IFileVersionRepository, FileVersionRepository>();
            services.AddTransient<IFileManager, FileManager>();

            services.AddTransient<IUserProvider, UserProvider>();
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            services.AddAutoMapper(System.Reflection.Assembly.Load("PanacheSoftware.Core"));

            services.AddControllersWithViews().AddNewtonsoftJson();

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Panache Software File API", Version = "v1" });
                var filePath = Path.Combine(System.AppContext.BaseDirectory, "PanacheSoftware.Service.File.xml");
                c.IncludeXmlComments(filePath);

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
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Panache Software File API");
            });

            app.UseAuthentication();
            app.UseHttpsRedirection();
            app.UseMvc();
        }
    }
}
