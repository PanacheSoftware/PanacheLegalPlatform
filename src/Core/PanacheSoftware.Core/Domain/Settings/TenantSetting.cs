using PanacheSoftware.Core.Domain.Core;
using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace PanacheSoftware.Core.Domain.Settings
{
    public class TenantSetting : PanacheSoftwareEntity
    {
        public TenantSetting()
        {
            Value = string.Empty;
            Name = string.Empty;
            Description = string.Empty;
        }

        public Guid SettingHeaderId { get; set; }

        public virtual SettingHeader SettingHeader { get; set; }

        public string Value { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
    }
}
