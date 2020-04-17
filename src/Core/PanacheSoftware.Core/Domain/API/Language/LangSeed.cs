using System;
using System.Collections.Generic;
using System.Text;

namespace PanacheSoftware.Core.Domain.API.Language
{
    public class LangSeed
    {
        public LangSeed()
        {
            LangCodes = new List<LangCode>();
            LangHeaders = new List<LangHead>();
        }

        public List<LangCode> LangCodes { get; set; }
        public List<LangHead> LangHeaders { get; set; }
    }
}
