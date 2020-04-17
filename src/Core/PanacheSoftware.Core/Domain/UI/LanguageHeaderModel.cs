using Microsoft.AspNetCore.Mvc.Rendering;
using PanacheSoftware.Core.Domain.API.Language;
using System;
using System.Collections.Generic;
using System.Text;

namespace PanacheSoftware.Core.Domain.UI
{
    public class LanguageHeaderModel
    {
        public LangHead langHead { get; set; }
        public SelectList StatusList { get; set; }
        public SelectList LanguageCodeSelectList { get; set; }
        public LangQueryList langQueryList { get; set; }
    }
}
