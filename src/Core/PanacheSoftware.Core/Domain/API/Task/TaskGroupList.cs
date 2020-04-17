using System;
using System.Collections.Generic;
using System.Text;

namespace PanacheSoftware.Core.Domain.API.Task
{
    public class TaskGroupList
    {
        public TaskGroupList()
        {
            TaskGroupHeaders = new List<TaskGroupHead>();
        }

        public List<TaskGroupHead> TaskGroupHeaders { get; set; }
    }
}
