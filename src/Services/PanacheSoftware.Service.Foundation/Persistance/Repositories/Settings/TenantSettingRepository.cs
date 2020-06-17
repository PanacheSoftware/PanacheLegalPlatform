using Microsoft.EntityFrameworkCore;
using PanacheSoftware.Core.Domain.Settings;
using PanacheSoftware.Core.Types;
using PanacheSoftware.Database.Repositories;
using PanacheSoftware.Http;
using PanacheSoftware.Service.Foundation.Core.Repositories;
using PanacheSoftware.Service.Foundation.Persistance.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PanacheSoftware.Service.Foundation.Persistance.Repositories
{
    public class TenantSettingRepository : PanacheSoftwareRepository<TenantSetting>, ITenantSettingRepository
    {
        private readonly IUserProvider _userProvider;

        public TenantSettingRepository(PanacheSoftwareServiceFoundationContext context, IUserProvider userProvider) : base(context)
        {
            _userProvider = userProvider;
        }

        public PanacheSoftwareServiceFoundationContext PanacheLegalFoundationContext
        {
            get { return Context as PanacheSoftwareServiceFoundationContext; }
        }

        public TenantSetting GetTenantSetting(Guid tenantSettingId, bool readOnly = true)
        {
            TenantSetting tenantSetting;

            if (readOnly)
            {
                tenantSetting = PanacheLegalFoundationContext.TenantSettings
                    .Where(t => t.Id == tenantSettingId)
                    .AsNoTracking()
                    .SingleOrDefault();
            }
            else
            {
                tenantSetting = PanacheLegalFoundationContext.TenantSettings
                    .Where(t => t.Id == tenantSettingId)
                    .SingleOrDefault();
            }

            if (tenantSetting == null)
            {
                SettingHeader settingHeader = PanacheLegalFoundationContext.SettingHeaders
                    .Where(h => h.Id == tenantSettingId)
                    .Where(h => h.SettingType == SettingTypes.SYSTEM)
                    .AsNoTracking()
                    .SingleOrDefault();

                if (settingHeader != null)
                {
                    return new TenantSetting()
                    {
                        SettingHeaderId = settingHeader.Id,
                        Status = StatusTypes.Open,
                        Value = settingHeader.DefaultValue,
                        Name = settingHeader.Name,
                        Description = settingHeader.Description
                    };
                }
            }

            return tenantSetting;
        }

        public TenantSetting GetTenantSetting(string tenantSettingName, bool readOnly = true)
        {
            TenantSetting tenantSetting = null;

            SettingHeader settingHeader = PanacheLegalFoundationContext.SettingHeaders
                .Where(h => h.Name == tenantSettingName)
                .Where(h => h.SettingType == SettingTypes.SYSTEM)
                .Include(t => t.TenantSettings)
                .FirstOrDefault();

            if (settingHeader != null)
            {
                tenantSetting = settingHeader.TenantSettings.FirstOrDefault();

                if (tenantSetting == null)
                {
                    tenantSetting = new TenantSetting()
                    {
                        SettingHeaderId = settingHeader.Id,
                        Status = StatusTypes.Open,
                        Value = settingHeader.DefaultValue,
                        Name = settingHeader.Name,
                        Description = settingHeader.Description
                    };
                }
                else
                {
                    if (readOnly)
                        return PanacheLegalFoundationContext.TenantSettings.Where(t => t.Id == tenantSetting.Id).AsNoTracking().FirstOrDefault();

                    return PanacheLegalFoundationContext.TenantSettings.Where(t => t.Id == tenantSetting.Id).FirstOrDefault();
                }
            }

            return tenantSetting;
        }

        public IEnumerable<TenantSetting> GetTenantSettings(bool readOnly = true)
        {
            List<TenantSetting> tenantSettings = new List<TenantSetting>();

            List<SettingHeader> settingHeaders = PanacheLegalFoundationContext.SettingHeaders
                .Where(h => h.SettingType == SettingTypes.SYSTEM)
                .Include(t => t.TenantSettings)
                .AsNoTracking()
                .ToList();

            foreach (var settingHeader in settingHeaders)
            {
                TenantSetting foundTenantSetting = settingHeader.TenantSettings.FirstOrDefault();

                if (foundTenantSetting != null)
                {
                    if (readOnly)
                    {
                        tenantSettings.Add(PanacheLegalFoundationContext.TenantSettings.Where(t => t.Id == foundTenantSetting.Id).AsNoTracking().FirstOrDefault());
                    }
                    else
                    {
                        tenantSettings.Add(PanacheLegalFoundationContext.TenantSettings.Where(t => t.Id == foundTenantSetting.Id).FirstOrDefault());
                    }
                }
                else
                {
                    tenantSettings.Add(new TenantSetting()
                    {
                        SettingHeaderId = settingHeader.Id,
                        Status = StatusTypes.Open,
                        Value = settingHeader.DefaultValue,
                        Name = settingHeader.Name,
                        Description = settingHeader.Description
                    });
                }
            }

            return tenantSettings;
        }
    }
}
