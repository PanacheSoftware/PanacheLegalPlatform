using PanacheSoftware.Core.Types;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace PanacheSoftware.Core.Domain.API.Task
{
    public class TaskHead
    {
        public TaskHead()
        {
            DateFrom = DateTime.Today;
            DateTo = DateTime.Today.AddYears(1);
            CompletionDate = DateTime.Now;
            OriginalCompletionDate = DateTime.Now;
            CompletedOnDate = DateTime.Parse("01/01/1900");
            Status = StatusTypes.Open;
            TaskDetail = new TaskDet();
            TaskType = NodeTypes.Task;
            Completed = false;
        }

        public Guid Id { get; set; }
        public Guid TaskGroupHeaderId { get; set; }
        [Required, DataType(DataType.DateTime, ErrorMessage = "Must be a valid DateTime")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime DateFrom { get; set; }
        [Required, DataType(DataType.DateTime, ErrorMessage = "Must be a valid DateTime")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime DateTo { get; set; }
        [Required]
        public string Status { get; set; }

        public string Description { get; set; }
        [Required]
        public string Title { get; set; }

        [Required, DataType(DataType.DateTime, ErrorMessage = "Must be a valid DateTime")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime CompletionDate { get; set; }
        [Required, DataType(DataType.DateTime, ErrorMessage = "Must be a valid DateTime")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime OriginalCompletionDate { get; set; }
        [Required, DataType(DataType.DateTime, ErrorMessage = "Must be a valid DateTime")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime CompletedOnDate { get; set; }

        [Required]
        public string TaskType { get; set; }

        [Required]
        public bool Completed { get; set; }

        [Required]
        public int SequenceNumber { get; set; }

        public TaskDet TaskDetail { get; set; }
    }
}
