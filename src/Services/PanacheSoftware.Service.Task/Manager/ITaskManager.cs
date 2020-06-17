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
        Task<TaskGroupList> GetTaskGroupListAsync(string accessToken);
        Task<TaskGroupList> GetMainTaskGroupsAsync(string accessToken);
        //List<Guid> GetChildTaskGroupIds(Guid taskGroupHeaderId);
        Task<bool> TaskGroupParentOkayAsync(TaskGroupHeader taskGroupHeader, string accessToken);
        Task<bool> SetNewTaskGroupSequenceNoAsync(TaskGroupHeader taskGroupHeader, string accessToken);
        Task<bool> SetNewTaskSequenceNoAsync(TaskHeader taskHeader, string accessToken);
        Task<TaskGroupSummaryList> GetMainTaskGroupSummarysAsync(string accessToken);
        Task<TaskGroupSummary> GetTaskGroupSummaryAsync(Guid taskGroupHeaderId, string accessToken);
        Task<bool> CanCompleteTaskGroupAsync(Guid taskGroupHeaderId, string accessToken);
        Task<bool> CanAccessTaskGroupHeaderAsync(Guid taskGroupHeaderId, string accessToken);
        Task<bool> TaskGroupTeamOkayAsync(TaskGroupHeader taskGroupHeader, string accessToken);
    }
}
