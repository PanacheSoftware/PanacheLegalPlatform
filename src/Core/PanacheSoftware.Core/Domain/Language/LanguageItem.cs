using PanacheSoftware.Core.Domain.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace PanacheSoftware.Core.Domain.Language
{
    public class LanguageItem : PanacheSoftwareEntity
    {
        public Guid LanguageHeaderId { get; set; }
        public virtual LanguageHeader LanguageHeader { get; set; }
        public string LanguageCodeId { get; set; }
        public string Text { get; set; }

        public DateTime DateFrom { get; set; }
        public DateTime DateTo { get; set; }
    }
}
