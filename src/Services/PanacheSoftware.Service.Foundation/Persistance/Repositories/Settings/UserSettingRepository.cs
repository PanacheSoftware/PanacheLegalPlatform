using Microsoft.EntityFrameworkCore;
using PanacheSoftware.Core.Domain.Settings;
using PanacheSoftware.Core.Types;
using PanacheSoftware.Database.Repositories;
using PanacheSoftware.Service.Foundation.Core;
using PanacheSoftware.Service.Foundation.Core.Repositories;
using PanacheSoftware.Service.Foundation.Persistance.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PanacheSoftware.Service.Foundation.Persistance.Repositories
{
    public class UserSettingRepository : PanacheSoftwareRepository<UserSetting>, IUserSettingRepository
    {
        public UserSettingRepository(PanacheSoftwareServiceFoundationContext context) : base(context)
        {
        }

        public PanacheSoftwareServiceFoundationContext PanacheLegalFoundationContext
        {
            get { return Context as PanacheSoftwareServiceFoundationContext; }
        }

        public UserSetting GetUserSetting(Guid userSettingId, Guid userId, bool readOnly = true)
        {
            UserSetting userSetting;

            if (readOnly)
            {
                userSetting = PanacheLegalFoundationContext.UserSettings
                    .Where(u => u.Id == userSettingId)
                    .Where(u => u.UserId == userId)
                    .AsNoTracking()
                    .SingleOrDefault();
            }
            else
            {
                userSetting = PanacheLegalFoundationContext.UserSettings
                    .Where(u => u.Id == userSettingId)
                    .Where(u => u.UserId == userId)
                    .SingleOrDefault();
            }

            if (userSetting == null)
            {
                SettingHeader settingHeader = PanacheLegalFoundationContext.SettingHeaders
                    .Where(h => h.Id == userSettingId)
                    .Where(h => h.SettingType == SettingTypes.USER)
                    .AsNoTracking()
                    .SingleOrDefault();

                if (settingHeader != null)
                {
                    return new UserSetting()
                    {
                        SettingHeaderId = settingHeader.Id,
                        Status = StatusTypes.Open,
                        Value = settingHeader.Value
                    };
                }
            }

            return userSetting;
        }

        public UserSetting GetUserSetting(string userSettingName, Guid userId, bool readOnly = true)
        {
            UserSetting userSetting = null;

            SettingHeader settingHeader = PanacheLegalFoundationContext.SettingHeaders
                .Where(h => h.Name == userSettingName)
                .Where(h => h.SettingType == SettingTypes.USER)
                .Include(h => h.UserSettings)
                .FirstOrDefault();

            if(settingHeader != null)
            {
                userSetting = settingHeader.UserSettings.Where(u => u.UserId == userId).FirstOrDefault();

                if(userSetting == null)
                {
                    userSetting = new UserSetting()
                    {
                        SettingHeaderId = settingHeader.Id,
                        Status = StatusTypes.Open,
                        Value = settingHeader.Value
                    };
                }
                else
                {
                    if (readOnly)
                        return PanacheLegalFoundationContext.UserSettings.Where(u => u.Id == userSetting.Id).AsNoTracking().FirstOrDefault();

                    return PanacheLegalFoundationContext.UserSettings.Where(u => u.Id == userSetting.Id).FirstOrDefault();
                }
            }

            return userSetting;
        }

        public IEnumerable<UserSetting> GetUserSettings(Guid userId, bool readOnly = true)
        {
            List<UserSetting> userSettings = new List<UserSetting>();

            List<SettingHeader> settingHeaders = PanacheLegalFoundationContext.SettingHeaders
                .Where(h => h.SettingType == SettingTypes.USER)
                .Include(u => u.UserSettings)
                .AsNoTracking()
                .ToList();

            foreach (var settingHeader in settingHeaders)
            {
                UserSetting foundUserSetting = settingHeader.UserSettings.Where(u => u.UserId == userId).FirstOrDefault();

                if(foundUserSetting != null)
                {
                    if (readOnly)
                        userSettings.Add(PanacheLegalFoundationContext.UserSettings.Where(u => u.Id == foundUserSetting.Id).AsNoTracking().FirstOrDefault());

                    userSettings.Add(PanacheLegalFoundationContext.UserSettings.Where(u => u.Id == foundUserSetting.Id).FirstOrDefault());
                }
                else
                {
                    userSettings.Add(new UserSetting()
                    {
                        SettingHeaderId = settingHeader.Id,
                        Status = StatusTypes.Open,
                        Value = settingHeader.Value
                    });
                }
            }

            return userSettings;
        }
    }
}
