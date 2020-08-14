using IdentityServer4.EntityFramework.DbContexts;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using PanacheSoftware.Identity.Data;
using System;
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
            }
        }

        public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
    }
}
