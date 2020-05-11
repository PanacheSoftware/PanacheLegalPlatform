using System;
using System.Collections.Generic;
using System.Text;

namespace PanacheSoftware.Core.Domain.API.File
{
    public class FileList
    {
        public FileList()
        {
            FileHeaders = new List<FileHead>();
        }

        public List<FileHead> FileHeaders { get; set; }
    }
}
