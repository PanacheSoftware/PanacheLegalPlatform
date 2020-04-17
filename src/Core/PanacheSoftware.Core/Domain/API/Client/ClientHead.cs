using PanacheSoftware.Core.Types;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace PanacheSoftware.Core.Domain.API.Client
{
    public class ClientHead
    {
        public ClientHead()
        {
            ClientContacts = new List<ClientCon>();
            ClientDetail = new ClientDet();
            DateFrom = DateTime.Today;
            DateTo = DateTime.Today.AddYears(1);
            Status = StatusTypes.Open;
        }

        public Guid Id { get; set; }
        [Required]
        [Display(Name = "Client ID")]
        public string ShortName { get; set; }
        [Required]
        [Display(Name = "Client Name")]
        public string LongName { get; set; }
        [DisplayFormat(ConvertEmptyStringToNull = false)]
        public string Description { get; set; }
        [Required, DataType(DataType.DateTime, ErrorMessage = "Must be a valid DateTime")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime DateFrom { get; set; }
        [Required, DataType(DataType.DateTime, ErrorMessage = "Must be a valid DateTime")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime DateTo { get; set; }    
        public ClientDet ClientDetail { get; set; }
        public List<ClientCon> ClientContacts { get; set; }
        [Required]
        public string Status { get; set; }
    }
}
