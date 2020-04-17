using System;
using System.Collections.Generic;
using System.Text;

namespace PanacheSoftware.Core.Domain.API.Settings
{
    public class SettingSeed
    {
        public SettingSeed()
        {
            SettingHeaders = new List<SettingHead>();
        }

        public List<SettingHead> SettingHeaders { get; set; }
    }
}
