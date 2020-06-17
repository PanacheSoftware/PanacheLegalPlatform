using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using PanacheSoftware.Service.Task.Persistance.Context;
using System;
using System.Threading;

namespace PanacheSoftware.Service.Task
{
    public class MigrationHostedService : IHostedService
    {
        private readonly IServiceProvider _serviceProvider;

        public MigrationHostedService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public async System.Threading.Tasks.Task StartAsync(CancellationToken cancellationToken)
        {
            using(var scope = _serviceProvider.CreateScope())
            {
                var myDbContext = scope.ServiceProvider.GetRequiredService<PanacheSoftwareServiceTaskContext>();

                await myDbContext.Database.MigrateAsync();
            }
        }

        public System.Threading.Tasks.Task StopAsync(CancellationToken cancellationToken) => System.Threading.Tasks.Task.CompletedTask;
    }
}
