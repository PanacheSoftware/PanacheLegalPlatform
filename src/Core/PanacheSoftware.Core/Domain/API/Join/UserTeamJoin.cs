using PanacheSoftware.Core.Domain.API.Team;
using PanacheSoftware.Core.Types;
using System;
using System.ComponentModel.DataAnnotations;

namespace PanacheSoftware.Core.Domain.API.Join
{
    public class UserTeamJoin
    {
        public UserTeamJoin()
        {
            DateFrom = DateTime.Today;
            DateTo = DateTime.Today.AddYears(1);
            Status = StatusTypes.Open;
        }

        [Required]
        public Guid Id { get; set; }
        [Required]
        public Guid UserId { get; set; }
        [Required]
        public Guid TeamHeaderId { get; set; }
        public TeamHead TeamHeader { get; set; }
        [Required, DataType(DataType.DateTime, ErrorMessage = "Must be a valid DateTime")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime DateFrom { get; set; }
        [Required, DataType(DataType.DateTime, ErrorMessage = "Must be a valid DateTime")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime DateTo { get; set; }
        [Required]
        public string Status { get; set; }

        public string TeamHeaderIdString { 
            get { return TeamHeaderId.ToString(); }
            set { TeamHeaderId = Guid.TryParse(value, out Guid result) ? result : Guid.Empty; }
        }
    }
}
