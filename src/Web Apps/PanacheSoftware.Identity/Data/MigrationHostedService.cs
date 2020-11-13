using IdentityServer4.EntityFramework.DbContexts;
using IdentityServer4.EntityFramework.Mappers;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using PanacheSoftware.Core.Domain.Configuration;
using PanacheSoftware.Identity.Data;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace PanacheSoftware.Identity
{
    public class MigrationHostedService : IHostedService
    {
        private readonly IServiceProvider _serviceProvider;

        public MigrationHostedService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            using(var scope = _serviceProvider.CreateScope())
            {
                var myDbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

                await myDbContext.Database.MigrateAsync(cancellationToken: cancellationToken);

                var is4ConfigurationDbContext = scope.ServiceProvider.GetRequiredService<ConfigurationDbContext>();

                await is4ConfigurationDbContext.Database.MigrateAsync(cancellationToken: cancellationToken);

                var is4PersistanceDbContext = scope.ServiceProvider.GetRequiredService<PersistedGrantDbContext>();

                await is4PersistanceDbContext.Database.MigrateAsync(cancellationToken: cancellationToken);

                var configuration = scope.ServiceProvider.GetRequiredService<IConfiguration>();
                var panacheSoftwareConfiguration = new PanacheSoftwareConfiguration();
                configuration.Bind("PanacheSoftware", panacheSoftwareConfiguration);

                await SeedDatabaseFromConfigAsync(panacheSoftwareConfiguration, is4ConfigurationDbContext);
            }
        }

        public async Task<bool> SeedDatabaseFromConfigAsync(PanacheSoftwareConfiguration panacheSoftwareConfiguration, ConfigurationDbContext configContext)
        {
            var config = new Config(panacheSoftwareConfiguration);

            if (!configContext.Clients.Any())
            {
                foreach (var client in config.GetClients())
                {
                    await configContext.Clients.AddAsync(client.ToEntity());
                }

                await configContext.SaveChangesAsync();
            }

            if (!configContext.IdentityResources.Any())
            {
                foreach (var resource in config.GetIdentityResources())
                {
                    await configContext.IdentityResources.AddAsync(resource.ToEntity());
                }

                await configContext.SaveChangesAsync();
            }

            if (!configContext.ApiScopes.Any())
            {
                foreach (var scope in config.GetApiScopes())
                {
                    await configContext.ApiScopes.AddAsync(scope.ToEntity());
                }

                await configContext.SaveChangesAsync();
            }

            if (!configContext.ApiResources.Any())
            {
                foreach (var apiResource in config.GetApis())
                {
                    await configContext.ApiResources.AddAsync(apiResource.ToEntity());
                }

                await configContext.SaveChangesAsync();
            }

            return true;
        }

        public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
    }
}
