using PanacheSoftware.Core.Types;
using System;
using System.Collections.Generic;
using System.Text;

namespace PanacheSoftware.Core.Domain.API.File
{
    public class FileDet
    {
        public FileDet()
        {
            Status = StatusTypes.Open;
            FileTitle = string.Empty;
            Description = string.Empty;
            FileType = string.Empty;
            FileExtension = string.Empty;
        }

        public Guid Id { get; set; }
        public Guid FileHeaderId { get; set; }
        public string FileTitle { get; set; }
        public string Description { get; set; }
        public string FileType { get; set; }
        public string FileExtension { get; set; }
        public string Status { get; set; }
    }
}
