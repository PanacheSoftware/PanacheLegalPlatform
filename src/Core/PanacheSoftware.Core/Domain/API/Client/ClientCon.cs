using PanacheSoftware.Core.Types;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace PanacheSoftware.Core.Domain.API.Client
{
    public class ClientCon
    {
        public ClientCon()
        {
            ClientAddresses = new List<ClientAddr>();
            DateFrom = DateTime.Today;
            DateTo = DateTime.Today.AddYears(1);
            Status = StatusTypes.Open;
            Title = ContactTitles.NA;
            MainContact = false;
        }

        public Guid Id { get; set; }
        public Guid ClientHeaderId { get; set; }
        [Required]
        public string Title { get; set; }
        [Required]
        [Display(Name = "First Name")]
        public string FirstName { get; set; }
        [DisplayFormat(ConvertEmptyStringToNull = false)]
        [Display(Name = "Middle Name")]
        public string MiddleName { get; set; }
        [Required]
        [Display(Name = "Last Name")]
        public string LastName { get; set; }
        [Required, DataType(DataType.EmailAddress, ErrorMessage = "Must be a valid Email address")]
        public string Email { get; set; }
        [DataType(DataType.PhoneNumber, ErrorMessage = "Must be a valid Phone number")]
        public string Phone { get; set; }
        [Required, DataType(DataType.DateTime, ErrorMessage = "Must be a valid DateTime")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime DateFrom { get; set; }
        [Required, DataType(DataType.DateTime, ErrorMessage = "Must be a valid DateTime")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime DateTo { get; set; }
        public List<ClientAddr> ClientAddresses { get; set; }
        public bool MainContact { get; set; }
        [Required]
        public string Status { get; set; }
    }
}
