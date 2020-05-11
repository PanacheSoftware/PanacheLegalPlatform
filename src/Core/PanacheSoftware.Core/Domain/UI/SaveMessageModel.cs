using PanacheSoftware.Core.Domain.API.Language;
using System;
using System.Collections.Generic;
using System.Text;

namespace PanacheSoftware.Core.Domain.UI
{
    public class SaveMessageModel
    {
        public string SaveState { get; set; }
        public string ErrorString { get; set; }
        public int HistoryLength { get; set; }
        public LangQueryList SaveLangQueryList { get; set; }
    }
}
