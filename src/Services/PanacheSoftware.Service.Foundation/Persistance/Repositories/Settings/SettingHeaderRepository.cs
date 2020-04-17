using Microsoft.EntityFrameworkCore;
using PanacheSoftware.Core.Domain.Settings;
using PanacheSoftware.Core.Types;
using PanacheSoftware.Database.Repositories;
using PanacheSoftware.Service.Foundation.Core.Repositories;
using PanacheSoftware.Service.Foundation.Persistance.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PanacheSoftware.Service.Foundation.Persistance.Repositories
{
    public class SettingHeaderRepository : PanacheSoftwareRepository<SettingHeader>, ISettingHeaderRepository
    {
        public SettingHeaderRepository(PanacheSoftwareServiceFoundationContext context) : base(context)
        {
        }

        public PanacheSoftwareServiceFoundationContext PanacheLegalFoundationContext
        {
            get { return Context as PanacheSoftwareServiceFoundationContext; }
        }

        public SettingHeader GetSettingHeader(Guid settingHeaderId, string settingType = "SYSTEM", bool includeUser = true, bool readOnly = true)
        {
            if (readOnly)
            {
                if (settingType == SettingTypes.USER && includeUser)
                {
                    return PanacheLegalFoundationContext.SettingHeaders
                        .Where(h => h.SettingType == settingType)
                        .Include(h => h.UserSettings)
                        .AsNoTracking()
                        .SingleOrDefault(h => h.Id == settingHeaderId);
                }

                return PanacheLegalFoundationContext.SettingHeaders
                        .Where(h => h.SettingType == settingType)
                        .AsNoTracking()
                        .SingleOrDefault(h => h.Id == settingHeaderId);
            }
            else
            {
                if (settingType == SettingTypes.USER && includeUser)
                {
                    return PanacheLegalFoundationContext.SettingHeaders
                        .Where(h => h.SettingType == settingType)
                        .Include(h => h.UserSettings)
                        .SingleOrDefault(h => h.Id == settingHeaderId);
                }

                return PanacheLegalFoundationContext.SettingHeaders
                        .Where(h => h.SettingType == settingType)
                        .SingleOrDefault(h => h.Id == settingHeaderId);
            }
        }

        public SettingHeader GetSettingHeader(string settingHeaderName, string settingType = "SYSTEM", bool includeUser = true, bool readOnly = true)
        {
            if (readOnly)
            {
                if (settingType == SettingTypes.USER && includeUser)
                {
                    return PanacheLegalFoundationContext.SettingHeaders
                        .Where(h => h.SettingType == settingType)
                        .Include(h => h.UserSettings)
                        .AsNoTracking()
                        .SingleOrDefault(h => h.Name == settingHeaderName);
                }

                return PanacheLegalFoundationContext.SettingHeaders
                        .Where(h => h.SettingType == settingType)
                        .AsNoTracking()
                        .SingleOrDefault(h => h.Name == settingHeaderName);
            }
            else
            {
                if (settingType == SettingTypes.USER && includeUser)
                {
                    return PanacheLegalFoundationContext.SettingHeaders
                        .Where(h => h.SettingType == settingType)
                        .Include(h => h.UserSettings)
                        .SingleOrDefault(h => h.Name == settingHeaderName);
                }

                return PanacheLegalFoundationContext.SettingHeaders
                        .Where(h => h.SettingType == settingType)
                        .SingleOrDefault(h => h.Name == settingHeaderName);
            }
        }

        public IEnumerable<SettingHeader> GetSettingHeaders(string settingType = "SYSTEM", bool includeUser = true, bool readOnly = true)
        {
            if (readOnly)
            {
                if (settingType == SettingTypes.USER && includeUser)
                {
                    return PanacheLegalFoundationContext.SettingHeaders
                        .Where(h => h.SettingType == settingType)
                        .Include(h => h.UserSettings)
                        .AsNoTracking();
                }

                return PanacheLegalFoundationContext.SettingHeaders
                        .Where(h => h.SettingType == settingType)
                        .AsNoTracking();
            }
            else
            {
                if (settingType == SettingTypes.USER && includeUser)
                {
                    return PanacheLegalFoundationContext.SettingHeaders
                        .Where(h => h.SettingType == settingType)
                        .Include(h => h.UserSettings);
                }

                return PanacheLegalFoundationContext.SettingHeaders
                        .Where(h => h.SettingType == settingType);
            }
        }
    }
}
