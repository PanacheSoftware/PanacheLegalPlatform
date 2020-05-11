using PanacheSoftware.Core.Types;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace PanacheSoftware.Core.Domain.API.File
{
    public class FileVer
    {
        public FileVer()
        {
            Status = StatusTypes.Open;
            URI = string.Empty;
            UntrustedName = string.Empty;
            TrustedName = string.Empty;
            Size = 0;
            UploadDate = DateTime.Now;
            VersionNumber = 0;
        }

        public Guid Id { get; set; }
        public Guid FileHeaderId { get; set; }
        public byte[] Content { get; set; }
        public string URI { get; set; }

        public string UntrustedName { get; set; }
        public string TrustedName { get; set; }

        public long Size { get; set; } //Size in bytes

        public DateTime UploadDate { get; set; }

        public int VersionNumber { get; set; }

        [Required]
        public string Status { get; set; }
    }
}
