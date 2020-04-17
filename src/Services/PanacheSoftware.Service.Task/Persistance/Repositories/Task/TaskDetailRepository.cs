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
    public class TaskDetailRepository : PanacheSoftwareRepository<TaskDetail>, ITaskDetailRepository
    {
        public TaskDetailRepository(PanacheSoftwareServiceTaskContext context) : base(context)
        {
        }

        public PanacheSoftwareServiceTaskContext PanacheSoftwareServiceTaskContext
        {
            get { return Context as PanacheSoftwareServiceTaskContext; }
        }

        public TaskDetail GetDetail(Guid taskDetailId, bool readOnly)
        {
            if (readOnly)
                return PanacheSoftwareServiceTaskContext.TaskDetails.AsNoTracking().FirstOrDefault(fnd => fnd.Id == taskDetailId);

            return PanacheSoftwareServiceTaskContext.TaskDetails.FirstOrDefault(fnd => fnd.Id == taskDetailId);
        }

        public TaskDetail GetTaskDetail(Guid taskHeaderId, bool readOnly)
        {
            if (readOnly)
                return PanacheSoftwareServiceTaskContext.TaskDetails.AsNoTracking().FirstOrDefault(fnd => fnd.TaskHeaderId == taskHeaderId);

            return PanacheSoftwareServiceTaskContext.TaskDetails.FirstOrDefault(fnd => fnd.TaskHeaderId == taskHeaderId);
        }
    }
}
