using System;
using System.Collections.Generic;
using System.Text;

namespace PanacheSoftware.Core.Domain.API.Task
{
    public class TaskGroupSummaryList
    {
        public TaskGroupSummaryList()
        {
            TaskGroupSummarys = new List<TaskGroupSummary>();
        }

        public List<TaskGroupSummary> TaskGroupSummarys { get; set; }
    }
}
