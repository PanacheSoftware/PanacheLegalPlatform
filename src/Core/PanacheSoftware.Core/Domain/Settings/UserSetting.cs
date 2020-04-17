using PanacheSoftware.Core.Domain.Core;
using System;

namespace PanacheSoftware.Core.Domain.Settings
{
    public class UserSetting : PanacheSoftwareEntity
    {
        public UserSetting()
        {
            Value = string.Empty;
        }

        public Guid UserId { get; set; }
        public Guid SettingHeaderId { get; set; }

        public virtual SettingHeader SettingHeader { get; set; }

        public string Value { get; set; }
    }
}
