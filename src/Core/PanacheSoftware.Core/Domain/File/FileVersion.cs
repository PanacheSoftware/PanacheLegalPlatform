using System;
using System.Collections.Generic;
using System.Text;

namespace PanacheSoftware.Core.Domain.File
{
    public class FileVersion
    {
        public byte[] Content { get; set; }

        public string UntrustedName { get; set; }

        public long Size { get; set; } //Size in bytes

        public DateTime UploadDate { get; set; }

        public int VersionNumber { get; set; }
    }
}
