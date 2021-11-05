using Microsoft.EntityFrameworkCore;
using PanacheSoftware.Core.Domain.API.Join;
using PanacheSoftware.Core.Domain.API.Team;
using PanacheSoftware.Core.Domain.Task;
using PanacheSoftware.Core.Types;
using PanacheSoftware.Database.Repositories;
using PanacheSoftware.Http;
using PanacheSoftware.Service.Task.Core.Repositories;
using PanacheSoftware.Service.Task.Persistance.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace PanacheSoftware.Service.Task.Persistance.Repositories.Task
{
    public class TaskGroupHeaderRepository : PanacheSoftwareRepository<TaskGroupHeader>, ITaskGroupHeaderRepository
    {
        private readonly IUserProvider _userProvider;
        private readonly IAPIHelper _apiHelper;
        private List<Guid> _userTeams;

        public TaskGroupHeaderRepository(PanacheSoftwareServiceTaskContext context, IUserProvider userProvider, IAPIHelper apiHelper) : base(context)
        {
            _userProvider = userProvider;
            _apiHelper = apiHelper;
        }

        public PanacheSoftwareServiceTaskContext PanacheSoftwareServiceTaskContext
        {
            get { return Context as PanacheSoftwareServiceTaskContext; }
        }

        public async Task<List<TaskGroupHeader>> GetMainTaskGroupsAsync(bool includeChildren, string accessToken)
        {
            var userTeams = await GetUserTeamsAsync(accessToken);

            if (includeChildren)
                return PanacheSoftwareServiceTaskContext.TaskGroupHeaders
                .Include(t => t.ChildTaskGroups)
                .Include(t => t.ChildTasks)
                .AsEnumerable()
                .Where(t => t.ParentTaskGroupId == null)
                .Where(t => userTeams.Contains(t.TeamHeaderId)).ToList();

            return PanacheSoftwareServiceTaskContext.TaskGroupHeaders.AsEnumerable().Where(f => f.ParentTaskGroupId == null).Where(t => userTeams.Contains(t.TeamHeaderId)).ToList();
        }

        public TaskGroupHeader GetTaskGroupHeader(string taskGroupShortName, bool readOnly)
        {
            if (readOnly)
                return PanacheSoftwareServiceTaskContext.TaskGroupHeaders.AsNoTracking().SingleOrDefault(c => c.Id == TaskGroupNameToId(taskGroupShortName));

            return PanacheSoftwareServiceTaskContext.TaskGroupHeaders.Find(TaskGroupNameToId(taskGroupShortName));
        }

        public async Task<TaskGroupHeader> GetTaskGroupHeaderWithRelationsAsync(string taskGroupShortName, bool readOnly, string accessToken)
        {
            return await GetTaskGroupHeaderWithRelationsAsync(TaskGroupNameToId(taskGroupShortName), readOnly, accessToken);
        }

        public async Task<TaskGroupHeader> GetTaskGroupHeaderWithRelationsAsync(Guid taskGroupHeaderId, bool readOnly, string accessToken)
        {
            var userTeams = await GetUserTeamsAsync(accessToken);

            TaskGroupHeader taskGroupHeader; 

            if (readOnly)
                taskGroupHeader = PanacheSoftwareServiceTaskContext.TaskGroupHeaders
                .AsNoTracking()
                .Include(t => t.TaskGroupDetail)
                .AsNoTracking()
                .Include(t => t.ChildTasks.Where(ct => ct.Status != StatusTypes.Closed))
                .AsNoTracking()
                .Include(t => t.ChildTaskGroups.Where(ctg => ctg.Status != StatusTypes.Closed))
                .ThenInclude(ct => ct.ChildTasks.Where(ct => ct.Status != StatusTypes.Closed))
                .AsNoTracking()
                .SingleOrDefault(t => t.Id == taskGroupHeaderId && t.Status != StatusTypes.Closed);

            taskGroupHeader = PanacheSoftwareServiceTaskContext.TaskGroupHeaders
                .Include(t => t.TaskGroupDetail)
                .Include(t => t.ChildTasks.Where(ct => ct.Status != StatusTypes.Closed))
                .Include(t => t.ChildTaskGroups.Where(ctg => ctg.Status != StatusTypes.Closed))
                .ThenInclude(ct => ct.ChildTasks.Where(ct => ct.Status != StatusTypes.Closed))
                .SingleOrDefault(t => t.Id == taskGroupHeaderId && t.Status != StatusTypes.Closed);

            if(taskGroupHeader != null)
            {
                if (taskGroupHeader.ParentTaskGroupId != null)
                {
                    var parentTaskGroup = PanacheSoftwareServiceTaskContext.TaskGroupHeaders.Where(th => th.Id == taskGroupHeader.ParentTaskGroupId).FirstOrDefault();

                    if (parentTaskGroup != null)
                    {
                        if (userTeams.Contains(parentTaskGroup.TeamHeaderId))
                        {
                            return taskGroupHeader;
                        }
                    }
                }
                else
                {
                    if (userTeams.Contains(taskGroupHeader.TeamHeaderId))
                    {
                        return taskGroupHeader;
                    }
                }
            }

            return null;
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

        public async Task<List<TaskGroupHeader>> GetChildTaskGroupHeadersAsync(Guid taskGroupHeaderId, bool includeChildTasks, bool readOnly)
        {
            if(readOnly)
            {
                if (includeChildTasks)
                    return PanacheSoftwareServiceTaskContext.TaskGroupHeaders.Where(t => t.ParentTaskGroupId == taskGroupHeaderId).AsNoTracking().Include(t => t.ChildTasks).AsNoTracking().ToList();

                return PanacheSoftwareServiceTaskContext.TaskGroupHeaders.Where(t => t.ParentTaskGroupId == taskGroupHeaderId).AsNoTracking().ToList();
            }

            if(includeChildTasks)
                return PanacheSoftwareServiceTaskContext.TaskGroupHeaders.Where(t => t.ParentTaskGroupId == taskGroupHeaderId).Include(t => t.ChildTasks).ToList();

            return PanacheSoftwareServiceTaskContext.TaskGroupHeaders.Where(t => t.ParentTaskGroupId == taskGroupHeaderId).ToList();
        }

        public async Task<List<TaskHeader>> GetChildTaskHeadersAsync(Guid taskGroupHeaderId, bool readOnly)
        {
            if(readOnly)
                return PanacheSoftwareServiceTaskContext.TaskHeaders.Where(t => t.TaskGroupHeaderId == taskGroupHeaderId).AsNoTracking().ToList();

            return PanacheSoftwareServiceTaskContext.TaskHeaders.Where(t => t.TaskGroupHeaderId == taskGroupHeaderId).ToList();
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

        private async Task<List<Guid>> GetUserTeamsAsync(string accessToken)
        {
            if (_userTeams == null)
                _userTeams = await _apiHelper.GetTeamsForUserId(accessToken, _userProvider.GetUserId());

            return _userTeams;
        }
    }
}
