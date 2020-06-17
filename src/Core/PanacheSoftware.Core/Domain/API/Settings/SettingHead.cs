using PanacheSoftware.Core.Types;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace PanacheSoftware.Core.Domain.API.Settings
{
    public class SettingHead
    {
        public SettingHead()
        {
            Status = StatusTypes.Open;
            UserSettings = new List<UsrSetting>();
            TenantSettings = new List<TenSetting>();
        }

        public Guid Id { get; set; }

        public string Description { get; set; }
        public string Value { get; set; }
        public string DefaultValue { get; set; }
        public string SettingType { get; set; }
        public string Name { get; set; }

        public string Status { get; set; }

        public List<UsrSetting> UserSettings { get; set; }
        public List<TenSetting> TenantSettings { get; set; }
    }
}
