using PanacheSoftware.Core.Domain.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace PanacheSoftware.Core.Domain.File
{
    public class FileLink : PanacheSoftwareEntity
    {
        public Guid LinkId { get; set; }
        public string LinkType { get; set; }
        public Guid FileHeaderId { get; set; }
        public virtual FileHeader FileHeader { get; set; }
    }
}
