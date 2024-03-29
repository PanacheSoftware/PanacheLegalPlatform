﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PanacheSoftware.Core.Domain.UI
{
    public class AlertModel
    {
        public AlertModel()
        {
            ShowAlert = false;
            AlertMessage = string.Empty;
            AlertDescription = string.Empty;
            AlertType = "Info";
        }

        public bool ShowAlert { get; set; }
        public string AlertMessage { get; set; }
        public string AlertDescription { get; set; }
        public string AlertType { get; set; }
    }
}
