using System;
using System.Collections.Generic;
using System.Text;

namespace PanacheSoftware.Core.Domain.File
{
    public class FileHeader
    {
        public FileHeader()
        {
            FileVersions = new HashSet<FileVersion>();
            FileDetail = new FileDetail();
        }

        public DateTime DateFrom { get; set; }
        public DateTime DateTo { get; set; }

        public virtual FileDetail FileDetail { get; set; }

        public virtual ICollection<FileVersion> FileVersions { get; set; }
    }
}
