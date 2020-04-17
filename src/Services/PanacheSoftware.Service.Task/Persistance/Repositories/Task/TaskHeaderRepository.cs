using Microsoft.EntityFrameworkCore;
using PanacheSoftware.Core.Domain.Task;
using PanacheSoftware.Database.Repositories;
using PanacheSoftware.Service.Task.Core.Repositories;
using PanacheSoftware.Service.Task.Persistance.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PanacheSoftware.Service.Task.Persistance.Repositories.Task
{
    public class TaskHeaderRepository : PanacheSoftwareRepository<TaskHeader>, ITaskHeaderRepository
    {
        private readonly ITaskGroupHeaderRepository _taskGroupHeaderRepository;

        public TaskHeaderRepository(PanacheSoftwareServiceTaskContext context, ITaskGroupHeaderRepository taskGroupHeaderRepository) : base(context)
        {
            _taskGroupHeaderRepository = taskGroupHeaderRepository;
        }

        public PanacheSoftwareServiceTaskContext PanacheSoftwareServiceTaskContext
        {
            get { return Context as PanacheSoftwareServiceTaskContext; }
        }

        public TaskHeader GetTaskHeader(Guid taskHeaderId, bool readOnly)
        {
            if (readOnly)
                return PanacheSoftwareServiceTaskContext.TaskHeaders.Include(n => n.TaskDetail).AsNoTracking().SingleOrDefault(a => a.Id == taskHeaderId);

            return PanacheSoftwareServiceTaskContext.TaskHeaders.Include(n => n.TaskDetail).SingleOrDefault(a => a.Id == taskHeaderId);
        }

        public IEnumerable<TaskHeader> GetTaskHeaders(string taskGroupHeaderShortName, bool readOnly)
        {
            return GetTaskHeaders(_taskGroupHeaderRepository.TaskGroupNameToId(taskGroupHeaderShortName), readOnly);
        }

        public IEnumerable<TaskHeader> GetTaskHeaders(Guid taskGroupHeaderId, bool readOnly)
        {
            if (readOnly)
                return PanacheSoftwareServiceTaskContext.TaskHeaders.Include(n => n.TaskDetail).AsNoTracking().Where(f => f.TaskGroupHeaderId == taskGroupHeaderId);

            return PanacheSoftwareServiceTaskContext.TaskHeaders.Include(n => n.TaskDetail).Where(f => f.TaskGroupHeaderId == taskGroupHeaderId);
        }
    }
}
