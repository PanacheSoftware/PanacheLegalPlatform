using PanacheSoftware.Core.Domain.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace PanacheSoftware.Core.Domain.File
{
    public class FileHeader : PanacheSoftwareEntity
    {
        public FileHeader()
        {
            FileVersions = new HashSet<FileVersion>();
            FileLinks = new HashSet<FileLink>();
            FileDetail = new FileDetail();
        }

        public virtual FileDetail FileDetail { get; set; }

        public virtual ICollection<FileVersion> FileVersions { get; set; }

        public virtual ICollection<FileLink> FileLinks { get; set; }
    }
}
