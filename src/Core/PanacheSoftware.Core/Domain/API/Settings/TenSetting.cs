using System;
using System.Collections.Generic;
using System.Text;

namespace PanacheSoftware.Core.Domain.API.Settings
{
    public class TenSetting
    {
        public TenSetting()
        {
            Value = string.Empty;
            Name = string.Empty;
            Description = string.Empty;
        }

        public Guid Id { get; set; }

        public Guid SettingHeaderId { get; set; }

        public string Value { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
    }
}
