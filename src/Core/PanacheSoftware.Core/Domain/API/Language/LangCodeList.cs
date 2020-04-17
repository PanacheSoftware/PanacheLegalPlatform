using System;
using System.Collections.Generic;
using System.Text;

namespace PanacheSoftware.Core.Domain.API.Language
{
    public class LangCodeList
    {
        public LangCodeList()
        {
            LanguageCodes = new List<LangCode>();
        }

        public List<LangCode> LanguageCodes { get; set; }
    }
}
