using Microsoft.EntityFrameworkCore;
using PanacheSoftware.Core.Domain.Core;
using PanacheSoftware.Database.Persistance;
using PanacheSoftware.Http;
using PanacheSoftware.Service.Foundation.Core;
using PanacheSoftware.Service.Foundation.Core.Repositories;
using PanacheSoftware.Service.Foundation.Persistance.Context;
using PanacheSoftware.Service.Foundation.Persistance.Repositories;
using System;
using System.Linq;

namespace PanacheSoftware.Service.Client.Persistance
{
    public class UnitOfWork : PanacheSoftwareUnitOfWork, IUnitOfWork
    {
        public UnitOfWork(PanacheSoftwareServiceFoundationContext context, IUserProvider userProvider) : base(context, userProvider)
        {
            LanguageHeaders = new LanguageHeaderRepository(context);
            LanguageCodes = new LanguageCodeRepository(context);
            LanguageItems = new LanguageItemRepository(context);
            SettingHeaders = new SettingHeaderRepository(context);
            UserSettings = new UserSettingRepository(context);
        }

        public ILanguageHeaderRepository LanguageHeaders { get; private set; }

        public ILanguageCodeRepository LanguageCodes { get; private set; }

        public ILanguageItemRepository LanguageItems { get; private set; }

        public ISettingHeaderRepository SettingHeaders { get; private set; }

        public IUserSettingRepository UserSettings { get; private set; }

    }
}
