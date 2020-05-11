using PanacheSoftware.Core.Domain.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace PanacheSoftware.Core.Domain.File
{
    public class FileVersion : PanacheSoftwareEntity
    {
        public Guid FileHeaderId { get; set; }
        public string URI { get; set; }
        public byte[] Content { get; set; }

        public string UntrustedName { get; set; }
        public string TrustedName { get; set; }

        public long Size { get; set; } //Size in bytes

        public DateTime UploadDate { get; set; }

        public int VersionNumber { get; set; }
        public virtual FileHeader FileHeader { get; set; }
    }
}
