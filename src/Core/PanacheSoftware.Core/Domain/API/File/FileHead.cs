using PanacheSoftware.Core.Types;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace PanacheSoftware.Core.Domain.API.File
{
    public class FileHead
    {
        public FileHead()
        {
            FileDetail = new FileDet();
            FileVersions = new List<FileVer>();
            FileLinks = new List<FileLnk>();
            Status = StatusTypes.Open;
        }

        public Guid Id { get; set; }

        public FileDet FileDetail { get; set; }

        public List<FileVer> FileVersions { get; set; }

        public List<FileLnk> FileLinks { get; set; }

        [Required]
        public string Status { get; set; }

    }
}
