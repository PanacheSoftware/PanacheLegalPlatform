using PanacheSoftware.Core.Domain.Task;
using PanacheSoftware.Database.Core.Repositories;
using System;

namespace PanacheSoftware.Service.Task.Core.Repositories
{
    public interface ITaskDetailRepository : IPanacheSoftwareRepository<TaskDetail>
    {
        TaskDetail GetTaskDetail(Guid taskHeaderId, bool readOnly);
        TaskDetail GetDetail(Guid taskDetailId, bool readOnly);
    }
}
