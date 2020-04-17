using PanacheSoftware.Core.Domain.Settings;
using PanacheSoftware.Database.Core.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PanacheSoftware.Service.Foundation.Core.Repositories
{
    public interface IUserSettingRepository : IPanacheSoftwareRepository<UserSetting>
    {
        UserSetting GetUserSetting(Guid userSettingId, Guid userId, bool readOnly = true);
        UserSetting GetUserSetting(string userSettingName, Guid userId, bool readOnly = true);
        IEnumerable<UserSetting> GetUserSettings(Guid userId, bool readOnly = true);
    }
}
