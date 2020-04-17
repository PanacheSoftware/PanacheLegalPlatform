using PanacheSoftware.Core.Domain.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace PanacheSoftware.Core.Domain.Language
{
    public class LanguageHeader : PanacheSoftwareEntity
    {
        public LanguageHeader()
        {
            LanguageItems = new HashSet<LanguageItem>();
        }

        public long TextCode { get; set; }
        public string Description { get; set; }
        public string Text { get; set; }

        public DateTime DateFrom { get; set; }
        public DateTime DateTo { get; set; }

        public virtual ICollection<LanguageItem> LanguageItems { get; set; }
    }
}
