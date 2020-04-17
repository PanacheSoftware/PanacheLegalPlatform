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
    public class TaskGroupDetailRepository : PanacheSoftwareRepository<TaskGroupDetail>, ITaskGroupDetailRepository
    {
        private readonly ITaskGroupHeaderRepository _taskGroupHeaderRepository;

        public TaskGroupDetailRepository(PanacheSoftwareServiceTaskContext context, ITaskGroupHeaderRepository taskGroupHeaderRepository) : base(context)
        {
            _taskGroupHeaderRepository = taskGroupHeaderRepository;
        }

        public PanacheSoftwareServiceTaskContext PanacheSoftwareServiceTaskContext
        {
            get { return Context as PanacheSoftwareServiceTaskContext; }
        }

        public TaskGroupDetail GetDetail(Guid taskGroupDetailId, bool readOnly)
        {
            if (readOnly)
                return PanacheSoftwareServiceTaskContext.TaskGroupDetails.AsNoTracking().SingleOrDefault(a => a.Id == taskGroupDetailId);

            return PanacheSoftwareServiceTaskContext.TaskGroupDetails.SingleOrDefault(a => a.Id == taskGroupDetailId);
        }

        public TaskGroupDetail GetTaskGroupDetail(string taskGroupShortName, bool readOnly)
        {
            return GetTaskGroupDetail(_taskGroupHeaderRepository.TaskGroupNameToId(taskGroupShortName), readOnly);
        }

        public TaskGroupDetail GetTaskGroupDetail(Guid taskGroupHeaderId, bool readOnly)
        {
            if (readOnly)
                return PanacheSoftwareServiceTaskContext.TaskGroupDetails.AsNoTracking().FirstOrDefault(t => t.TaskGroupHeaderId == taskGroupHeaderId);

            return PanacheSoftwareServiceTaskContext.TaskGroupDetails.FirstOrDefault(t => t.TaskGroupHeaderId == taskGroupHeaderId);
        }
    }
}
