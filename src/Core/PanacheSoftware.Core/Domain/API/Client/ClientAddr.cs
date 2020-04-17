using PanacheSoftware.Core.Types;
using System;
using System.ComponentModel.DataAnnotations;

namespace PanacheSoftware.Core.Domain.API.Client
{
    public class ClientAddr
    {
        public ClientAddr()
        {
            DateFrom = DateTime.Today;
            DateTo = DateTime.Today.AddYears(1);
            Status = StatusTypes.Open;
            AddressType = AddressTypes.General;
        }

        public Guid Id { get; set; }
        public Guid ClientContactId { get; set; }
        [Required]
        [Display(Name = "Address Type")]
        public string AddressType { get; set; }
        [Required]
        [Display(Name = "Addr Ln 1")]
        public string AddressLine1 { get; set; }
        [DisplayFormat(ConvertEmptyStringToNull = false)]
        [Display(Name = "Addr Ln 2")]
        public string AddressLine2 { get; set; }
        [DisplayFormat(ConvertEmptyStringToNull = false)]
        [Display(Name = "Addr Ln 3")]
        public string AddressLine3 { get; set; }
        [DisplayFormat(ConvertEmptyStringToNull = false)]
        [Display(Name = "Addr Ln 4")]
        public string AddressLine4 { get; set; }
        [DisplayFormat(ConvertEmptyStringToNull = false)]
        [Display(Name = "Addr Ln 5")]
        public string AddressLine5 { get; set; }
        [Required]
        [Display(Name = "Postal code")]
        public string PostalCode { get; set; }
        [Required]
        [Display(Name = "Country")]
        public string Country { get; set; }
        [Required]
        [Display(Name = "Region")]
        public string Region { get; set; }
        [DataType(DataType.EmailAddress, ErrorMessage = "Must be a valid Email address")]
        [DisplayFormat(ConvertEmptyStringToNull = false)]
        public string Email1 { get; set; }
        [DataType(DataType.EmailAddress, ErrorMessage = "Must be a valid Email address")]
        [DisplayFormat(ConvertEmptyStringToNull = false)]
        public string Email2 { get; set; }
        [DataType(DataType.EmailAddress, ErrorMessage = "Must be a valid Email address")]
        [DisplayFormat(ConvertEmptyStringToNull = false)]
        public string Email3 { get; set; }
        [DataType(DataType.PhoneNumber, ErrorMessage = "Must be a valid Phone number")]
        [DisplayFormat(ConvertEmptyStringToNull = false)]
        public string Phone1 { get; set; }
        [DataType(DataType.PhoneNumber, ErrorMessage = "Must be a valid Phone number")]
        [DisplayFormat(ConvertEmptyStringToNull = false)]
        public string Phone2 { get; set; }
        [DataType(DataType.PhoneNumber, ErrorMessage = "Must be a valid Phone number")]
        [DisplayFormat(ConvertEmptyStringToNull = false)]
        public string Phone3 { get; set; }
        [Required, DataType(DataType.DateTime, ErrorMessage = "Must be a valid DateTime")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime DateFrom { get; set; }
        [Required, DataType(DataType.DateTime, ErrorMessage = "Must be a valid DateTime")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime DateTo { get; set; }
        [Required]
        [Display(Name = "Status")]
        public string Status { get; set; }
    }
}
