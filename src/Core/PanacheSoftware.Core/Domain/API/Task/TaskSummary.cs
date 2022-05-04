using PanacheSoftware.Core.Domain.API.File;
using PanacheSoftware.Core.Domain.API.Language;
using System;
using System.Collections.Generic;
using System.Text;

namespace PanacheSoftware.Core.Domain.API.Task
{
    public class TaskSummary
    {
        public TaskSummary()
        {
            FileList = new FileList();
            langQueryList = new LangQueryList();
            MainUserName = string.Empty;
        }

        public Guid Id { get; set; }
        public Guid TaskGroupHeaderId { get; set; }
        public Guid MainUserId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime OriginalStartDate { get; set; }
        public string Status { get; set; }

        public string Description { get; set; }
        public string Title { get; set; }

        public DateTime CompletionDate { get; set; }
        public DateTime OriginalCompletionDate { get; set; }
        public DateTime CompletedOnDate { get; set; }

        public string NodeType { get; set; }

        public bool Completed { get; set; }

        public int SequenceNumber { get; set; }

        public FileList FileList { get; set; }
        //Adding this is a temporary fix for the task group and task item summary pages
        public LangQueryList langQueryList { get; set; }
        public string MainUserName { get; set; }

        public string ShortName { get; set; }
    }
}
