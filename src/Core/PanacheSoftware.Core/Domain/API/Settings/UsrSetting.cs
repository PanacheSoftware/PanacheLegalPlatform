using System;
using System.Collections.Generic;
using System.Text;

namespace PanacheSoftware.Core.Domain.API.Settings
{
    public class UsrSetting
    {
        public UsrSetting()
        {
            Value = string.Empty;
        }

        public Guid Id { get; set; }

        public Guid UserId { get; set; }
        public Guid SettingHeaderId { get; set; }

        public string Value { get; set; }
    }
}
