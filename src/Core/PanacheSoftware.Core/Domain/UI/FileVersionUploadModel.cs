using Microsoft.AspNetCore.Http;
using PanacheSoftware.Core.Domain.API.File;
using System;
using System.Collections.Generic;
using System.Text;

namespace PanacheSoftware.Core.Domain.UI
{
    public class FileVersionUploadModel
    {
        public FileVersionUploadModel()
        {
            FileVersion = new FileVer();
            URI = string.Empty;
        }

        public FileVer FileVersion { get; set; }
        public IFormFile FormFile { get; set; }
        public string URI { get; set; }
        public string FileTitle { get; set; }
        public string Description { get; set; }
    }
}
