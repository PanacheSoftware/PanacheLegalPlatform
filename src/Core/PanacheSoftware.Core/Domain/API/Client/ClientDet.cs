using PanacheSoftware.Core.Types;
using System;
using System.ComponentModel.DataAnnotations;

namespace PanacheSoftware.Core.Domain.API.Client
{
    public class ClientDet
    {
        public ClientDet()
        {
            DateFrom = DateTime.Today;
            DateTo = DateTime.Today.AddYears(1);
            Status = StatusTypes.Open;
            Base64Image = Base64Images.PanacheSoftwareDot;
        }

        public Guid Id { get; set; }
        public Guid ClientHeaderId { get; set; }
        [DataType(DataType.Url, ErrorMessage = "Must be a valid URL")]
        [RegularExpression(@"[(http(s)?):\/\/(www\.)?a-zA-Z0-9@:%._\+~#=]{2,256}\.[a-z]{2,6}\b([-a-zA-Z0-9@:%_\+.~#?&//=]*)", ErrorMessage = "Not a valid url")]
        [DisplayFormat(ConvertEmptyStringToNull = false)]
        public string url { get; set; }
        [DisplayFormat(ConvertEmptyStringToNull = false)]
        public string Base64Image { get; set; }
        [DisplayFormat(ConvertEmptyStringToNull = false)]
        public string Country { get; set; }
        [DisplayFormat(ConvertEmptyStringToNull = false)]
        public string Region { get; set; }
        [Required, DataType(DataType.DateTime, ErrorMessage = "Must be a valid DateTime")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime DateFrom { get; set; }
        [Required, DataType(DataType.DateTime, ErrorMessage = "Must be a valid DateTime")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime DateTo { get; set; }
        [Required]
        public string Status { get; set; }
    }
}
