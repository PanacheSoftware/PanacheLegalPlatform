using PanacheSoftware.Core.Types;
using System;
using System.Collections.Generic;
using System.Text;

namespace PanacheSoftware.Core.Domain.API.File
{
    public class FileLnk
    {
        public FileLnk()
        {
            Status = StatusTypes.Open;
            LinkType = string.Empty;
        }

        public Guid Id { get; set; }
        public Guid LinkId { get; set; }
        public string LinkType { get; set; }
        public Guid FileHeaderId { get; set; }
        public string Status { get; set; }
    }
}
