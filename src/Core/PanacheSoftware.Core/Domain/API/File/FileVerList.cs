using System;
using System.Collections.Generic;
using System.Text;

namespace PanacheSoftware.Core.Domain.API.File
{
    public class FileVerList
    {
        public FileVerList()
        {
            FileVersions = new List<FileVer>();
        }

        public List<FileVer> FileVersions { get; set; }
    }
}
