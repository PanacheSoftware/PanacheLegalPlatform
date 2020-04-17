using System;
using System.Collections.Generic;
using System.Text;

namespace PanacheSoftware.Core.Domain.API.Task
{
    public class TaskSummary
    {
        public Guid Id { get; set; }
        public Guid TaskGroupHeaderId { get; set; }
        public DateTime DateFrom { get; set; }
        public DateTime DateTo { get; set; }
        public string Status { get; set; }

        public string Description { get; set; }
        public string Title { get; set; }

        public DateTime CompletionDate { get; set; }
        public DateTime OriginalCompletionDate { get; set; }
        public DateTime CompletedOnDate { get; set; }

        public string NodeType { get; set; }

        public bool Completed { get; set; }

        public int SequenceNumber { get; set; }
    }
}
