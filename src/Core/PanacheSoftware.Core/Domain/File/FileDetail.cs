using PanacheSoftware.Core.Domain.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace PanacheSoftware.Core.Domain.File
{
    public class FileDetail : PanacheSoftwareEntity
    {
        public Guid FileHeaderId { get; set; }
        public virtual FileHeader FileHeader { get; set; }
        public string FileTitle { get; set; }
        public string Description { get; set; }
        public string FileType { get; set; }
        public string FileExtension { get; set; }
    }
}
