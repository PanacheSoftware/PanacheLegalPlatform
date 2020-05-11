using PanacheSoftware.Core.Domain.API.File;
using PanacheSoftware.Core.Domain.API.Language;
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
            FileList = new FileList();
            PercentageComplete = 0;
            langQueryList = new LangQueryList();
            MainUserName = string.Empty;
        }

        public Guid Id { get; set; }
        public string ShortName { get; set; }
        public string LongName { get; set; }
        public string Description { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime OriginalStartDate { get; set; }
        public Guid ClientHeaderId { get; set; }
        public Guid MainUserId { get; set; }
        public string Status { get; set; }
        public int SequenceNumber { get; set; }
        public DateTime CompletionDate { get; set; }
        public DateTime OriginalCompletionDate { get; set; }
        public DateTime CompletedOnDate { get; set; }
        public bool Completed { get; set; }
        public List<TaskGroupSummary> ChildTaskGroups { get; set; }
        public List<TaskSummary> ChildTasks { get; set; }
        public FileList FileList { get; set; }
        public double PercentageComplete { get; set; }
        //Adding this is a temporary fix for the task group and task item summary pages
        public LangQueryList langQueryList { get; set; }
        public string MainUserName { get; set; }

    }
}
