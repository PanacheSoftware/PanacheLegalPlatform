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
        TaskGroupHeader GetTaskGroupHeaderWithRelations(string taskGroupShortName, bool readOnly);
        TaskGroupHeader GetTaskGroupHeaderWithRelations(Guid taskGroupHeaderId, bool readOnly);
        Guid TaskGroupNameToId(string taskGroupShortName);
        List<TaskGroupHeader> GetTaskGroupTree(string taskGroupShortName);
        List<TaskGroupHeader> GetTaskGroupTree(Guid taskGroupHeaderId);
        List<TaskGroupHeader> GetMainTaskGroups(bool includeChildren);  
    }
}
