using PanacheSoftware.Core.Domain.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace PanacheSoftware.Core.Domain.Task
{
    public class TaskHeader : PanacheSoftwareEntity
    {
        public TaskHeader()
        {
            TaskDetail = new TaskDetail();
        }

        public Guid TaskGroupHeaderId { get; set; }
        public virtual TaskGroupHeader TaskGroupHeader { get; set; }

        public string Description { get; set; }
        public string Title { get; set; }

        public DateTime CompletionDate { get; set; }
        public DateTime OriginalCompletionDate { get; set; }
        public DateTime CompletedOnDate { get; set; }

        public DateTime DateFrom { get; set; }
        public DateTime DateTo { get; set; }

        public string TaskType { get; set; }

        public bool Completed { get; set; }

        public int SequenceNumber { get; set; }

        public virtual TaskDetail TaskDetail { get; set; }
    }
}
