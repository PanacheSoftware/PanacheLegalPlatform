using System;
using System.Collections.Generic;
using System.Text;

namespace PanacheSoftware.Core.Domain.API.Language
{
    public class LangList
    {
        public LangList()
        {
            LanguageHeaders = new List<LangHead>();
        }

        public List<LangHead> LanguageHeaders { get; set; }
    }
}
