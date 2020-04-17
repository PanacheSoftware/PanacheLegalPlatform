using PanacheSoftware.Core.Types;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace PanacheSoftware.Core.Domain.API.Team
{
    public class TeamHead
    {
        public TeamHead()
        {
            ChildTeams = new List<TeamHead>();
            TeamDetail = new TeamDet();
            DateFrom = DateTime.Today;
            DateTo = DateTime.Today.AddYears(1);
            Status = StatusTypes.Open;
        }

        public Guid Id { get; set; }
        [Required]
        [Display(Name = "Team ID")]
        public string ShortName { get; set; }
        [Required]
        [Display(Name = "Team Name")]
        public string LongName { get; set; }
        [DisplayFormat(ConvertEmptyStringToNull = false)]
        public string Description { get; set; }
        [Required, DataType(DataType.DateTime, ErrorMessage = "Must be a valid DateTime")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime DateFrom { get; set; }
        [Required, DataType(DataType.DateTime, ErrorMessage = "Must be a valid DateTime")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime DateTo { get; set; }
        public Guid? ParentTeamId { get; set; }
        public TeamDet TeamDetail { get; set; }
        public List<TeamHead> ChildTeams { get; set; }
        [Required]
        public string Status { get; set; }
    }
}
