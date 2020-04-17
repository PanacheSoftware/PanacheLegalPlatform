using PanacheSoftware.Core.Types;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace PanacheSoftware.Core.Domain.API.Task
{
    public class TaskDet
    {
        public TaskDet()
        {
            DateFrom = DateTime.Today;
            DateTo = DateTime.Today.AddYears(1);
            Status = StatusTypes.Open;
        }

        public Guid Id { get; set; }
        public Guid TaskHeaderId { get; set; }
        [Required, DataType(DataType.DateTime, ErrorMessage = "Must be a valid DateTime")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime DateFrom { get; set; }
        [Required, DataType(DataType.DateTime, ErrorMessage = "Must be a valid DateTime")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime DateTo { get; set; }
        [Required]
        public string Status { get; set; }
        public string Data { get; set; }
    }
}
