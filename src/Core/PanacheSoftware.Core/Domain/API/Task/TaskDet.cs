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
            Status = StatusTypes.Open;
        }

        public Guid Id { get; set; }
        public Guid TaskHeaderId { get; set; }
        [Required]
        public string Status { get; set; }
    }
}
