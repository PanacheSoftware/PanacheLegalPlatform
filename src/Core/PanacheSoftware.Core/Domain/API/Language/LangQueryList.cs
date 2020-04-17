using System;
using System.Collections.Generic;
using System.Text;

namespace PanacheSoftware.Core.Domain.API.Language
{
    public class LangQueryList
    {
        public LangQueryList()
        {
            LangQuerys = new List<LangQuery>();
        }
        public List<LangQuery> LangQuerys { get; set; }
    }
}
