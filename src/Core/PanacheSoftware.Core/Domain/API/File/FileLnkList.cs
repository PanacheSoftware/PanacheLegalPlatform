using System;
using System.Collections.Generic;
using System.Text;

namespace PanacheSoftware.Core.Domain.API.File
{
    public class FileLnkList
    {
        public FileLnkList()
        {
            FileLinks = new List<FileLnk>();
        }

        public List<FileLnk> FileLinks { get; set; }
    }
}
