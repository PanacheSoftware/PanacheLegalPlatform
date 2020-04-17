using PanacheSoftware.Core.Domain.Task;
using PanacheSoftware.Database.Core.Repositories;
using System;
using System.Collections.Generic;

namespace PanacheSoftware.Service.Task.Core.Repositories
{
    public interface ITaskHeaderRepository : IPanacheSoftwareRepository<TaskHeader>
    {
        IEnumerable<TaskHeader> GetTaskHeaders(string taskGroupHeaderShortName, bool readOnly);
        IEnumerable<TaskHeader> GetTaskHeaders(Guid taskGroupHeaderId, bool readOnly);
        TaskHeader GetTaskHeader(Guid taskHeaderId, bool readOnly);
    }
}
