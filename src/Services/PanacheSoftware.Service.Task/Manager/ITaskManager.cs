using PanacheSoftware.Core.Domain.API.Task;
using PanacheSoftware.Core.Domain.Task;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PanacheSoftware.Service.Task.Manager
{
    public interface ITaskManager
    {
        TaskGroupList GetTaskGroupList(Guid taskGroupHeaderId = new Guid(), bool validParents = false);
        TaskGroupList GetMainTaskGroups();
        List<Guid> GetChildTaskGroupIds(Guid taskGroupHeaderId);
        bool TaskGroupParentOkay(TaskGroupHeader taskGroupHeader);
        bool SetNewTaskGroupSequenceNo(TaskGroupHeader taskGroupHeader);
        bool SetNewTaskSequenceNo(TaskHeader taskHeader);
        TaskGroupSummaryList GetMainTaskGroupSummarys();
        TaskGroupSummary GetTaskGroupSummary(Guid taskGroupHeaderId);
    }
}
