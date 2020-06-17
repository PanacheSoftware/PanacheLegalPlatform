using System;
using System.Collections.Generic;
using System.Text;

namespace PanacheSoftware.Core.Domain.API.Settings
{
    public class TenantSettingList
    {
        public TenantSettingList()
        {
            TenantSettings = new List<TenSetting>();
        }

        public List<TenSetting> TenantSettings { get; set; }
    }
}
