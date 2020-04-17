using System;
using System.Collections.Generic;
using System.Text;

namespace PanacheSoftware.Core.Domain.API.Language
{
    public class LangQuery
    {
        public LangQuery()
        {
            TextCode = 0;
            Text = string.Empty;
            LanguageCode = string.Empty;
        }

        public long TextCode { get; set; }

        public string Text { get; set; }

        public string LanguageCode { get; set; }
    }
}
