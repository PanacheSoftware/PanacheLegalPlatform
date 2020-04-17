using PanacheSoftware.Database.Core.Repositories;
using PanacheSoftware.Core.Domain.Settings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PanacheSoftware.Core.Types;

namespace PanacheSoftware.Service.Foundation.Core.Repositories
{
    public interface ISettingHeaderRepository : IPanacheSoftwareRepository<SettingHeader>
    {
        SettingHeader GetSettingHeader(Guid settingHeaderId, string settingType = SettingTypes.SYSTEM, bool includeUser = true, bool readOnly = true);
        SettingHeader GetSettingHeader(string settingHeaderName, string settingType = SettingTypes.SYSTEM, bool includeUser = true, bool readOnly = true);
        IEnumerable<SettingHeader> GetSettingHeaders(string settingType = SettingTypes.SYSTEM, bool includeUser = true, bool readOnly = true);
    }
}
