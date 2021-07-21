using Microsoft.AspNetCore.Http;
using PanacheSoftware.Core.Domain.API.File;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
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
        [DataType(DataType.Url, ErrorMessage = "Must be a valid URL")]
        [RegularExpression(@"^((https?|ftp|smtp):\/\/)?(www.)?[a-z0-9]+\.[a-z]+(\/[a-zA-Z0-9#]+\/?)*$", ErrorMessage = "Not a valid url")]
        [DisplayFormat(ConvertEmptyStringToNull = false)]
        public string URI { get; set; }
    }
}
