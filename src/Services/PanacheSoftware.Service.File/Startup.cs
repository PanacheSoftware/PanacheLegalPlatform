using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
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
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using PanacheSoftware.Core.Domain.Configuration;
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
            var panacheSoftwareConfiguration = new PanacheSoftwareConfiguration();
            Configuration.Bind("PanacheSoftware", panacheSoftwareConfiguration);

            switch (panacheSoftwareConfiguration.DBProvider)
            {
                case DBProvider.MySQL:
                    services.AddDbContext<PanacheSoftwareServiceFileContext>(options =>
                        options.UseMySql(Configuration.GetConnectionString(DBProvider.MySQL), ServerVersion.AutoDetect(Configuration.GetConnectionString(DBProvider.MySQL))));
                    break;
                case DBProvider.MSSQL:
                    services.AddDbContext<PanacheSoftwareServiceFileContext>(options =>
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
            //        options.ApiName = PanacheSoftwareScopeNames.FileService;
            //        options.ApiSecret = panacheSoftwareConfiguration.Secret.FileServiceSecret;
            //        options.RequireHttpsMetadata = false;
            //        //options.EnableCaching = true;
            //    });

            Action<JwtBearerOptions> jwtOptions = o =>
            {
                o.Authority = bool.Parse(panacheSoftwareConfiguration.CallMethod.UICallsSecure) ? panacheSoftwareConfiguration.Url.IdentityServerURLSecure : panacheSoftwareConfiguration.Url.IdentityServerURL;
                o.Audience = PanacheSoftwareScopeNames.FileService;
                o.RequireHttpsMetadata = false;
                o.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateAudience = true,
                };
            };

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(jwtOptions);

            services.AddTransient<IUnitOfWork, UnitOfWork>();
            services.AddTransient<IFileHeaderRepository, FileHeaderRepository>();
            services.AddTransient<IFileDetailRepository, FileDetailRepository>();
            services.AddTransient<IFileLinkRepository, FileLinkRepository>();
            services.AddTransient<IFileVersionRepository, FileVersionRepository>();
            services.AddTransient<IFileManager, FileManager>();

            services.AddTransient<IUserProvider, UserProvider>();
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            services.AddHostedService<MigrationHostedService>();

            services.AddAutoMapper(System.Reflection.Assembly.Load("PanacheSoftware.Core"));

            services.AddControllersWithViews().AddNewtonsoftJson();

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Panache Software File API v1", Version = "v1" });

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
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Panache Software File API v1");
            });

            app.UseAuthentication();
            //app.UseHttpsRedirection();
            app.UseMvc();
        }
    }
}
