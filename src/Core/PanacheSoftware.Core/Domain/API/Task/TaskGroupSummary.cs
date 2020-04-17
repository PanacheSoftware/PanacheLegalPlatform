using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace PanacheSoftware.Core.Domain.API.Task
{
    public class TaskGroupSummary
    {
        public TaskGroupSummary()
        {
            ChildTaskGroups = new List<TaskGroupSummary>();
            ChildTasks = new List<TaskSummary>();
            PercentageComplete = 0;
        }

        public Guid Id { get; set; }
        public string ShortName { get; set; }
        public string LongName { get; set; }
        public string Description { get; set; }
        public DateTime DateFrom { get; set; }
        public DateTime DateTo { get; set; }
        public Guid ClientHeaderId { get; set; }
        public string Status { get; set; }
        public int SequenceNumber { get; set; }
        public DateTime CompletionDate { get; set; }
        public DateTime OriginalCompletionDate { get; set; }
        public DateTime CompletedOnDate { get; set; }
        public bool Completed { get; set; }
        public List<TaskGroupSummary> ChildTaskGroups { get; set; }
        public List<TaskSummary> ChildTasks { get; set; }
        public double PercentageComplete { get; set; }

    }
}
