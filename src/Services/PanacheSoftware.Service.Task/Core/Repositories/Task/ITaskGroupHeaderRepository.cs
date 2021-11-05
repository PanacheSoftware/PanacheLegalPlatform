using PanacheSoftware.Core.Domain.Task;
using PanacheSoftware.Database.Core.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PanacheSoftware.Service.Task.Core.Repositories
{
    public interface ITaskGroupHeaderRepository : IPanacheSoftwareRepository<TaskGroupHeader>
    {
        TaskGroupHeader GetTaskGroupHeader(string taskGroupShortName, bool readOnly);
        Task<TaskGroupHeader> GetTaskGroupHeaderWithRelationsAsync(string taskGroupShortName, bool readOnly, string accessToken);
        Task<TaskGroupHeader> GetTaskGroupHeaderWithRelationsAsync(Guid taskGroupHeaderId, bool readOnly, string accessToken);
        Guid TaskGroupNameToId(string taskGroupShortName);
        List<TaskGroupHeader> GetTaskGroupTree(string taskGroupShortName);
        List<TaskGroupHeader> GetTaskGroupTree(Guid taskGroupHeaderId);
        Task<List<TaskGroupHeader>> GetMainTaskGroupsAsync(bool includeChildren, string accessToken);
        Task<List<TaskGroupHeader>> GetChildTaskGroupHeadersAsync(Guid taskGroupHeaderId, bool includeChildTasks, bool readOnly);
        Task<List<TaskHeader>> GetChildTaskHeadersAsync(Guid taskGroupHeaderId, bool readOnly);
    }
}
