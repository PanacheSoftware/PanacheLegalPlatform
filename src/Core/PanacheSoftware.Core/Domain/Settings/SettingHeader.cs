using PanacheSoftware.Core.Domain.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace PanacheSoftware.Core.Domain.Settings
{
    public class SettingHeader : PanacheSoftwareEntity
    {
        public SettingHeader()
        {
            UserSettings = new HashSet<UserSetting>();
        }

        public string Description { get; set; }
        public string Value { get; set; }
        public string DefaultValue { get; set; }
        public string SettingType { get; set; }
        public string Name { get; set; }

        public virtual ICollection<UserSetting> UserSettings { get; set; }
    }
}
