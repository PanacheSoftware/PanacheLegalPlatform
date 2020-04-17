using Microsoft.EntityFrameworkCore;
using PanacheSoftware.Core.Domain.Task;
using PanacheSoftware.Database.Repositories;
using PanacheSoftware.Service.Task.Core.Repositories;
using PanacheSoftware.Service.Task.Persistance.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace PanacheSoftware.Service.Task.Persistance.Repositories.Task
{
    public class TaskGroupHeaderRepository : PanacheSoftwareRepository<TaskGroupHeader>, ITaskGroupHeaderRepository
    {
        public TaskGroupHeaderRepository(PanacheSoftwareServiceTaskContext context) : base(context)
        {

        }

        public PanacheSoftwareServiceTaskContext PanacheSoftwareServiceTaskContext
        {
            get { return Context as PanacheSoftwareServiceTaskContext; }
        }

        public List<TaskGroupHeader> GetMainTaskGroups(bool includeChildren)
        {
            if(includeChildren)
                return PanacheSoftwareServiceTaskContext.TaskGroupHeaders
                .Include(t => t.ChildTaskGroups)
                .Include(t => t.ChildTasks)
                .AsEnumerable()
                .Where(t => t.ParentTaskGroupId == null).ToList();

            return PanacheSoftwareServiceTaskContext.TaskGroupHeaders.AsEnumerable().Where(f => f.ParentTaskGroupId == null).ToList();
        }

        public TaskGroupHeader GetTaskGroupHeader(string taskGroupShortName, bool readOnly)
        {
            if (readOnly)
                return PanacheSoftwareServiceTaskContext.TaskGroupHeaders.AsNoTracking().SingleOrDefault(c => c.Id == TaskGroupNameToId(taskGroupShortName));

            return PanacheSoftwareServiceTaskContext.TaskGroupHeaders.Find(TaskGroupNameToId(taskGroupShortName));
        }

        public TaskGroupHeader GetTaskGroupHeaderWithRelations(string taskGroupShortName, bool readOnly)
        {
            return GetTaskGroupHeaderWithRelations(TaskGroupNameToId(taskGroupShortName), readOnly);
        }

        public TaskGroupHeader GetTaskGroupHeaderWithRelations(Guid taskGroupHeaderId, bool readOnly)
        {
            if (readOnly)
                return PanacheSoftwareServiceTaskContext.TaskGroupHeaders
                .Include(t => t.TaskGroupDetail)
                .Include(t => t.ChildTasks)
                .Include(t => t.ChildTaskGroups)
                .ThenInclude(ct => ct.ChildTasks)
                .AsNoTracking()
                .SingleOrDefault(t => t.Id == taskGroupHeaderId);

            return PanacheSoftwareServiceTaskContext.TaskGroupHeaders
                .Include(t => t.TaskGroupDetail)
                .Include(t => t.ChildTasks)
                .Include(t => t.ChildTaskGroups)
                .ThenInclude(ct => ct.ChildTasks)
                .SingleOrDefault(t => t.Id == taskGroupHeaderId);

            //List<TaskGroupHeader> taskGroupHeader = null;

            //if (readOnly)
            //{
            //    taskGroupHeader = PanacheSoftwareServiceTaskContext.TaskGroupHeaders
            //        .Include(t => t.TaskGroupDetail)
            //        .Include(t => t.ChildTasks)
            //        .Include(t => t.ChildTaskGroups)
            //        .ThenInclude(ct => ct.ChildTasks)
            //        //.Include(t => t.ParentTaskGroup)

            //        .AsNoTracking()
            //        .AsEnumerable()
            //        .Where(t => t.Id == taskGroupHeaderId).ToList();
            //}
            //else
            //{
            //    taskGroupHeader = PanacheSoftwareServiceTaskContext.TaskGroupHeaders
            //        .Include(t => t.TaskGroupDetail)
            //        .Include(t => t.ChildTasks)
            //        .Include(t => t.ChildTaskGroups)
            //        .ThenInclude(ct => ct.ChildTasks)
            //        //.Include(t => t.ParentTaskGroup)

            //        .AsEnumerable()
            //        .Where(t => t.Id == taskGroupHeaderId).ToList();
            //}

            //if(taskGroupHeader != null)
            //{
            //    if(taskGroupHeader.Any())
            //    {
            //        return taskGroupHeader.FirstOrDefault();
            //    }
            //}

            //return null;
        }

        public List<TaskGroupHeader> GetTaskGroupTree(string taskGroupShortName)
        {
            return GetTaskGroupTree(TaskGroupNameToId(taskGroupShortName));
        }

        public List<TaskGroupHeader> GetTaskGroupTree(Guid taskGroupHeaderId)
        {
            return PanacheSoftwareServiceTaskContext.TaskGroupHeaders
                .Include(t => t.TaskGroupDetail)
                .Include(t => t.ChildTaskGroups)
                .Include(t => t.ChildTasks)
                .AsEnumerable()
                .Where(t => t.Id == taskGroupHeaderId).ToList();
        }

        public Guid TaskGroupNameToId(string taskGroupShortName)
        {
            Guid foundGuid = Guid.Empty;

            TaskGroupHeader foundTaskGroupHeader =
                PanacheSoftwareServiceTaskContext.TaskGroupHeaders.AsNoTracking().SingleOrDefault(t => t.ShortName == taskGroupShortName);

            if (foundTaskGroupHeader != null)
                foundGuid = foundTaskGroupHeader.Id;

            return foundGuid;
        }
    }
}
