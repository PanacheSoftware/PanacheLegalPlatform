using PanacheSoftware.Core.Domain.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace PanacheSoftware.Core.Domain.Task
{
    public class TaskGroupHeader : PanacheSoftwareEntity
    {
        public TaskGroupHeader()
        {
            TaskGroupDetail = new TaskGroupDetail();
            ChildTaskGroups = new List<TaskGroupHeader>();
            ChildTasks = new List<TaskHeader>();
        }

        public string ShortName { get; set; }
        public string LongName { get; set; }
        public string Description { get; set; }

        public Guid? ParentTaskGroupId { get; set; }
        public Guid MainUserId { get; set; }
        public Guid TeamHeaderId { get; set; }
        public Guid ClientHeaderId { get; set; }
        public virtual TaskGroupHeader ParentTaskGroup { get; set; }

        public virtual TaskGroupDetail TaskGroupDetail { get; set; }
        public virtual IList<TaskGroupHeader> ChildTaskGroups { get; set; }
        public virtual IList<TaskHeader> ChildTasks { get; set; }
        public int SequenceNumber { get; set; }

        public DateTime CompletionDate { get; set; }
        public DateTime OriginalCompletionDate { get; set; }
        public DateTime CompletedOnDate { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime OriginalStartDate { get; set; }
        public bool Completed { get; set; }
    }
}
