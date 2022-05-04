using PanacheSoftware.Core.Types;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace PanacheSoftware.Core.Domain.API.Task
{
    public class TaskGroupHead
    {
        public TaskGroupHead()
        {
            ChildTaskGroups = new List<TaskGroupHead>();
            ChildTasks = new List<TaskHead>();
            TaskGroupDetail = new TaskGroupDet();
            StartDate = DateTime.Today;
            OriginalStartDate = DateTime.Today;
            Status = StatusTypes.Open;
            CompletionDate = DateTime.Today;
            OriginalCompletionDate = DateTime.Today;
            CompletedOnDate = DateTime.Parse("01/01/1900");
            Completed = false;
        }

        public Guid Id { get; set; }
        [Required]
        [Display(Name = "Task Group ID")]
        [RegularExpression("^[A-Z0-9]*$", ErrorMessage = "Characters A-Z or 0-9 only")]
        [MaxLength(100, ErrorMessage = "Maximum Length 100 characters")]
        public string ShortName { get; set; }
        [Required]
        [Display(Name = "Task Group Name")]
        [MaxLength(255, ErrorMessage = "Maximum Length 255 characters")]
        public string LongName { get; set; }
        [DisplayFormat(ConvertEmptyStringToNull = false)]
        public string Description { get; set; }
        [Required, DataType(DataType.DateTime, ErrorMessage = "Must be a valid DateTime")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime StartDate { get; set; }
        [Required, DataType(DataType.DateTime, ErrorMessage = "Must be a valid DateTime")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime OriginalStartDate { get; set; }
        public Guid? ParentTaskGroupId { get; set; }
        public Guid MainUserId { get; set; }
        public Guid TeamHeaderId { get; set; }
        public Guid ClientHeaderId { get; set; }
        public TaskGroupDet TaskGroupDetail { get; set; }
        public List<TaskGroupHead> ChildTaskGroups { get; set; }
        public List<TaskHead> ChildTasks { get; set; }
        [Required]
        public string Status { get; set; }
        [Required]
        public int SequenceNumber { get; set; }

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
        public bool Completed { get; set; }
    }
}
