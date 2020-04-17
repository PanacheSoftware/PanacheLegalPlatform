using System;
using System.Collections.Generic;
using System.Text;

namespace PanacheSoftware.Core.Domain.API.Settings
{
    public class UsrSettingList
    {
        public UsrSettingList()
        {
            UserSettings = new List<UsrSetting>();
        }

        public List<UsrSetting> UserSettings { get; set; }
    }
}
