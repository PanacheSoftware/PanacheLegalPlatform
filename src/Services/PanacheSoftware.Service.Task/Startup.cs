using System;
using System.Collections.Generic;
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
using PanacheSoftware.Core.Domain.Configuration;
using PanacheSoftware.Core.Types;
using PanacheSoftware.Http;
using PanacheSoftware.Service.Task.Core;
using PanacheSoftware.Service.Task.Core.Repositories;
using PanacheSoftware.Service.Task.Manager;
using PanacheSoftware.Service.Task.Persistance;
using PanacheSoftware.Service.Task.Persistance.Context;
using PanacheSoftware.Service.Task.Persistance.Repositories.Task;
using PanacheSoftware.Service.Task.Persistance.Repositories.Template;

namespace PanacheSoftware.Service.Task
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
                    services.AddDbContext<PanacheSoftwareServiceTaskContext>(options =>
                        options.UseMySql(Configuration.GetConnectionString(DBProvider.MySQL), ServerVersion.AutoDetect(Configuration.GetConnectionString(DBProvider.MySQL))));
                    break;
                case DBProvider.MSSQL:
                    services.AddDbContext<PanacheSoftwareServiceTaskContext>(options =>
                        options.UseSqlServer(Configuration.GetConnectionString(DBProvider.MSSQL)));
                    break;
            }

            services.AddAuthorization();

            services.AddAuthentication(IdentityServerAuthenticationDefaults.AuthenticationScheme)
                .AddIdentityServerAuthentication(options =>
                {
                    // base-address of your identityserver
                    options.Authority = bool.Parse(panacheSoftwareConfiguration.CallMethod.UICallsSecure) ? panacheSoftwareConfiguration.Url.IdentityServerURLSecure : panacheSoftwareConfiguration.Url.IdentityServerURL;

                    // name of the API resource
                    options.ApiName = PanacheSoftwareScopeNames.TaskService;
                    options.ApiSecret = panacheSoftwareConfiguration.Secret.TaskServiceSecret;
                    options.RequireHttpsMetadata = false;
                    //options.EnableCaching = true;
                });

            services.AddMvc(options =>
            {
                options.EnableEndpointRouting = false;
            })
                .SetCompatibilityVersion(CompatibilityVersion.Version_3_0);

            services.AddTransient<IUnitOfWork, UnitOfWork>();
            services.AddTransient<ITaskGroupDetailRepository, TaskGroupDetailRepository>();
            services.AddTransient<ITaskGroupHeaderRepository, TaskGroupHeaderRepository>();
            services.AddTransient<ITaskHeaderRepository, TaskHeaderRepository>();
            services.AddTransient<ITaskDetailRepository, TaskDetailRepository>();
            services.AddTransient<ITemplateHeaderRepository, TemplateHeaderRepository>();
            services.AddTransient<ITemplateDetailRepository, TemplateDetailRepository>();
            services.AddTransient<ITemplateGroupHeaderRepository, TemplateGroupHeaderRepository>();
            services.AddTransient<ITemplateGroupDetailRepository, TemplateGroupDetailRepository>();
            services.AddTransient<ITemplateItemHeaderRepository, TemplateItemHeaderRepository>();
            services.AddTransient<ITemplateItemDetailRepository, TemplateItemDetailRepository>();
            services.AddTransient<ITaskManager, TaskManager>();
            services.AddTransient<ITemplateManager, TemplateManager>();

            services.AddTransient<IUserProvider, UserProvider>();
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddSingleton<IAPIHelper, APIHelper>();

            services.AddHostedService<MigrationHostedService>();

            services.AddAutoMapper(System.Reflection.Assembly.Load("PanacheSoftware.Core"));

            services.AddControllersWithViews().AddNewtonsoftJson();

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Panache Software Task API v1", Version = "v1" });

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
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Panache Software Task API v1");
            });

            app.UseAuthentication();
            //app.UseHttpsRedirection();
            app.UseMvc();
        }
    }
}
