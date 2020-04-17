

using PanacheSoftware.Core.Domain.Task;
using PanacheSoftware.Database.Core.Repositories;
using System;

namespace PanacheSoftware.Service.Task.Core.Repositories
{
    public interface ITaskGroupDetailRepository : IPanacheSoftwareRepository<TaskGroupDetail>
    {
        TaskGroupDetail GetTaskGroupDetail(string taskGroupShortName, bool readOnly);
        TaskGroupDetail GetTaskGroupDetail(Guid taskGroupHeaderId, bool readOnly);
        TaskGroupDetail GetDetail(Guid taskGroupDetailId, bool readOnly);
    }
}
