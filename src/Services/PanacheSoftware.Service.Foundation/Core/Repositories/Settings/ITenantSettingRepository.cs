using PanacheSoftware.Core.Domain.Settings;
using PanacheSoftware.Database.Core.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PanacheSoftware.Service.Foundation.Core.Repositories
{
    public interface ITenantSettingRepository : IPanacheSoftwareRepository<TenantSetting>
    {
        TenantSetting GetTenantSetting(Guid tenantSettingId, bool readOnly = true);
        TenantSetting GetTenantSetting(string tenantSettingName, bool readOnly = true);
        IEnumerable<TenantSetting> GetTenantSettings(bool readOnly = true);
    }
}
