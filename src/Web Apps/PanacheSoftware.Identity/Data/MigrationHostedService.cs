using IdentityServer4.EntityFramework.DbContexts;
using IdentityServer4.EntityFramework.Mappers;
using IdentityServer4.Models;
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

                await ClearPersistedGrants(is4PersistanceDbContext);
                await SeedDatabaseFromConfigAsync(panacheSoftwareConfiguration, is4ConfigurationDbContext);
            }
        }

        public async Task<bool> ClearPersistedGrants(PersistedGrantDbContext is4PersistanceDbContext)
        {
            is4PersistanceDbContext.RemoveRange(is4PersistanceDbContext.PersistedGrants);

            await is4PersistanceDbContext.SaveChangesAsync();

            return true;
        }

        public async Task<bool> SeedDatabaseFromConfigAsync(PanacheSoftwareConfiguration panacheSoftwareConfiguration, ConfigurationDbContext configContext)
        {
            var config = new Config(panacheSoftwareConfiguration);

            foreach (var client in config.GetClients())
            {
                var existingClient = configContext.Clients.FirstOrDefault(c => c.ClientId == client.ClientId);
                if (existingClient != default)
                    configContext.Clients.Remove(existingClient);

                await configContext.Clients.AddAsync(client.ToEntity());
            }

            foreach (var resource in config.GetIdentityResources())
            {
                if (configContext.IdentityResources.FirstOrDefault(i => i.Name == resource.Name) == default)
                    await configContext.IdentityResources.AddAsync(resource.ToEntity());
            }

            foreach (var scope in config.GetApiScopes())
            {
                if (configContext.ApiScopes.FirstOrDefault(a => a.Name == scope.Name) == default)
                    await configContext.ApiScopes.AddAsync(scope.ToEntity());
            }

            foreach (var apiResource in config.GetApis())
            {
                if (configContext.ApiScopes.FirstOrDefault(a => a.Name == apiResource.Name) == default)
                    await configContext.ApiResources.AddAsync(apiResource.ToEntity());
            }


            await configContext.SaveChangesAsync();

            return true;
        }

        public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
    }
}
