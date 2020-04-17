using System;
using System.Collections.Generic;
using System.Text;

namespace PanacheSoftware.Core.Domain.API.Settings
{
    public class SettingList
    {
        public SettingList()
        {
            SettingHeaders = new List<SettingHead>();
        }

        public List<SettingHead> SettingHeaders { get; set; }
    }
}
