using Microsoft.AspNetCore.Http;
using PanacheSoftware.Core.Domain.API.File;
using System;
using System.Collections.Generic;
using System.Text;

namespace PanacheSoftware.Core.Domain.UI
{
    public class FileUploadModel
    {
        public FileUploadModel()
        {
            FileHeader = new FileHead();
            linkId = Guid.Empty;
            linkType = string.Empty;
            URI = string.Empty;
        }

        public FileHead FileHeader { get; set; }
        public IFormFile FormFile { get; set; }
        public Guid linkId { get; set; }
        public string linkType { get; set; }
        public string URI { get; set; }
    }
}
