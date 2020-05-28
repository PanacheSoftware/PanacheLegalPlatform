using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using PanacheSoftware.Core.Domain.Language;
using PanacheSoftware.Core.Domain.Settings;
using PanacheSoftware.Http;
using PanacheSoftware.Service.Foundation.Core;
using PanacheSoftware.Service.Foundation.Persistance.Context;
using PanacheSoftware.Service.Foundation.Persistance.Data;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace PanacheSoftware.Service.Foundation
{
    public class MigrationHostedService : IHostedService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly IMapper _mapper;
        private readonly IStaticFileReader _staticFileReader;

        public MigrationHostedService(IServiceProvider serviceProvider, IMapper mapper, IStaticFileReader staticFileReader)
        {
            _serviceProvider = serviceProvider;
            _mapper = mapper;
            _staticFileReader = staticFileReader;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            using(var scope = _serviceProvider.CreateScope())
            {
                var myDbContext = scope.ServiceProvider.GetRequiredService<PanacheSoftwareServiceFoundationContext>();

                await myDbContext.Database.MigrateAsync();

                await SeedAsync(myDbContext);
            }
        }

        public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;

        private async Task SeedAsync(PanacheSoftwareServiceFoundationContext myDbContext)
        {
            if (!await myDbContext.LanguageCodes.AnyAsync() || !await myDbContext.LanguageHeaders.AnyAsync())
            {
                var languageSeed = new LanguageSeed(_staticFileReader);

                var langSeed = languageSeed.SeedData();

                if (!await myDbContext.LanguageCodes.AnyAsync())
                {
                    if (langSeed.LangCodes.Any())
                    {
                        foreach (var languageCode in langSeed.LangCodes)
                        {
                            await myDbContext.LanguageCodes.AddAsync(_mapper.Map<LanguageCode>(languageCode));
                        }

                        await myDbContext.SaveChangesAsync();
                    }
                }

                if (!await myDbContext.LanguageHeaders.AnyAsync())
                {
                    if (langSeed.LangHeaders.Any())
                    {
                        foreach (var languageHeader in langSeed.LangHeaders)
                        {
                            await myDbContext.LanguageHeaders.AddAsync(_mapper.Map<LanguageHeader>(languageHeader));
                        }

                        await myDbContext.SaveChangesAsync();
                    }
                }
            }

            if (!await myDbContext.SettingHeaders.AnyAsync())
            {
                var settingsSeed = new SettingsSeed(_staticFileReader);

                var settingSeed = settingsSeed.SeedData();

                if (!await myDbContext.SettingHeaders.AnyAsync())
                {
                    if (settingSeed.SettingHeaders.Any())
                    {
                        foreach (var settingHeader in settingSeed.SettingHeaders)
                        {
                            await myDbContext.SettingHeaders.AddAsync(_mapper.Map<SettingHeader>(settingHeader));
                        }

                        await myDbContext.SaveChangesAsync();
                    }
                }
            }
        }
    }
}
