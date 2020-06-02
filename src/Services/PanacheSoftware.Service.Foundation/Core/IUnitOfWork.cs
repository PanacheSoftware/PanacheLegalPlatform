using System;
using PanacheSoftware.Database.Core;
using PanacheSoftware.Service.Foundation.Core.Repositories;

namespace PanacheSoftware.Service.Foundation.Core
{
    public interface IUnitOfWork : IPanacheSoftwareUnitOfWork
    {
        ILanguageHeaderRepository LanguageHeaders { get; }
        ILanguageCodeRepository LanguageCodes { get; }
        ILanguageItemRepository LanguageItems { get; }

        ISettingHeaderRepository SettingHeaders { get; }
        IUserSettingRepository UserSettings { get; }
        ITenantSettingRepository TenantSettings { get; }
    }
}
