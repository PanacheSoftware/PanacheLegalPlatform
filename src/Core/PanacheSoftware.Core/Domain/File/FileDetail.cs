using System;
using System.Collections.Generic;
using System.Text;

namespace PanacheSoftware.Core.Domain.File
{
    public class FileDetail
    {
        public FileDetail()
        {

        }

        public string FileTitle { get; set; }
        public string Description { get; set; }
        public string FileType { get; set; }
        public string FileExtension { get; set; }

        public DateTime DateFrom { get; set; }
        public DateTime DateTo { get; set; }
    }
}
