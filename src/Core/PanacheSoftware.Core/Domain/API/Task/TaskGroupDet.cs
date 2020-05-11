using PanacheSoftware.Core.Types;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace PanacheSoftware.Core.Domain.API.Task
{
    public class TaskGroupDet
    {
        public TaskGroupDet()
        {
            Status = StatusTypes.Open;
        }

        public Guid Id { get; set; }
        public Guid TaskGroupHeaderId { get; set; }
        [Required]
        public string Status { get; set; }
    }
}
