﻿using Microsoft.EntityFrameworkCore;
using PanacheSoftware.Core.Domain.API.Join;
using PanacheSoftware.Core.Domain.API.Team;
using PanacheSoftware.Core.Domain.Task;
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
                .Include(t => t.TaskGroupDetail)
                .Include(t => t.ChildTasks)
                .Include(t => t.ChildTaskGroups)
                .ThenInclude(ct => ct.ChildTasks)
                .AsNoTracking()
                .SingleOrDefault(t => t.Id == taskGroupHeaderId);

            taskGroupHeader = PanacheSoftwareServiceTaskContext.TaskGroupHeaders
                .Include(t => t.TaskGroupDetail)
                .Include(t => t.ChildTasks)
                .Include(t => t.ChildTaskGroups)
                .ThenInclude(ct => ct.ChildTasks)
                .SingleOrDefault(t => t.Id == taskGroupHeaderId);

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
