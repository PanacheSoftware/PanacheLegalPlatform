using AutoMapper;
using Microsoft.EntityFrameworkCore.Internal;
using PanacheSoftware.Core.Domain.API.Task;
using PanacheSoftware.Core.Domain.API.Team;
using PanacheSoftware.Core.Domain.Task;
using PanacheSoftware.Http;
using PanacheSoftware.Service.Task.Core;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace PanacheSoftware.Service.Task.Manager
{
    public class TaskManager : ITaskManager
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IUserProvider _userProvider;
        private readonly IAPIHelper _apiHelper;
        private List<Guid> _userTeams;

        public TaskManager(IUnitOfWork unitOfWork, IMapper mapper, IUserProvider userProvider, IAPIHelper apiHelper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _userProvider = userProvider;
            _apiHelper = apiHelper;
        }

        //public List<Guid> GetChildTaskGroupIds(Guid taskGroupHeaderId)
        //{
        //    List<Guid> childTaskGroupHeaders = new List<Guid>();

        //    var taskGroupTree = _unitOfWork.TaskGroupHeaders.GetTaskGroupTree(taskGroupHeaderId);

        //    if (taskGroupTree.Any())
        //    {
        //        var queue = new Queue<TaskGroupHeader>();
        //        queue.Enqueue(taskGroupTree[0]);

        //        while (queue.Count > 0)
        //        {
        //            var node = queue.Dequeue();

        //            foreach (var childTaskGroupHeader in node.ChildTaskGroups)
        //            {
        //                if (!childTaskGroupHeaders.Contains(childTaskGroupHeader.Id))
        //                    childTaskGroupHeaders.Add(childTaskGroupHeader.Id);

        //                queue.Enqueue(childTaskGroupHeader);
        //            }
        //        }
        //    }

        //    return childTaskGroupHeaders;
        //}

        public async Task<TaskGroupList> GetMainTaskGroupsAsync(string accessToken)
        {
            TaskGroupList taskGroupList = new TaskGroupList();

            foreach (var currentTaskGroupHeader in await _unitOfWork.TaskGroupHeaders.GetMainTaskGroupsAsync(false, accessToken))
            {
                taskGroupList.TaskGroupHeaders.Add(_mapper.Map<TaskGroupHead>(currentTaskGroupHeader));
            }

            return taskGroupList;
        }

        public async Task<TaskGroupSummaryList> GetMainTaskGroupSummarysAsync(string accessToken)
        {
            TaskGroupSummaryList taskGroupSummaryList = new TaskGroupSummaryList();

            foreach (var currentTaskGroupHeader in await _unitOfWork.TaskGroupHeaders.GetMainTaskGroupsAsync(true, accessToken))
            {
                var taskSummary = _mapper.Map<TaskGroupSummary>(currentTaskGroupHeader);
                UpdatePercentages(taskSummary);
                taskGroupSummaryList.TaskGroupSummarys.Add(taskSummary);
            }

            return taskGroupSummaryList;
        }

        public async Task<TaskGroupSummary> GetTaskGroupSummaryAsync(Guid taskGroupHeaderId, string accessToken)
        {
            var taskGroupHeader = await _unitOfWork.TaskGroupHeaders.GetTaskGroupHeaderWithRelationsAsync(taskGroupHeaderId, true, accessToken);

            if (taskGroupHeader != null)
            {
                if (taskGroupHeader.ParentTaskGroupId == null)
                {
                    var taskSummary = _mapper.Map<TaskGroupSummary>(taskGroupHeader);
                    UpdatePercentages(taskSummary);
                    return taskSummary;
                }
            }

            return null;
        }

        //private void UpdatePercentages(TaskGroupSummary taskSummary)
        //{
        //    double childTaskCompletion = 0;
        //    double childTaskGroupCompletion = 0;

        //    if(taskSummary.ChildTasks.Any())
        //    {
        //        childTaskCompletion = Convert.ToDouble(taskSummary.ChildTasks.Where(c => c.Completed).Count()) / Convert.ToDouble(taskSummary.ChildTasks.Count());
        //    }

        //    if (taskSummary.ChildTaskGroups.Any())
        //    {
        //        for (int iCount = 0; iCount < taskSummary.ChildTaskGroups.Count; iCount++)
        //        {
        //            UpdatePercentages(taskSummary.ChildTaskGroups[iCount]);

        //            childTaskGroupCompletion += taskSummary.ChildTaskGroups[iCount].PercentageComplete;
        //        }
        //    }

        //    if(taskSummary.ChildTasks.Any() && taskSummary.ChildTaskGroups.Any())
        //    {
        //        taskSummary.PercentageComplete = (childTaskCompletion + childTaskGroupCompletion) / (taskSummary.ChildTaskGroups.Count + 1);
        //    }
        //    else if (taskSummary.ChildTasks.Any())
        //    {
        //        taskSummary.PercentageComplete = childTaskCompletion;
        //    }
        //    else
        //    {
        //        taskSummary.PercentageComplete = childTaskGroupCompletion / taskSummary.ChildTaskGroups.Count;
        //    }

        //    if (Double.IsNaN(taskSummary.PercentageComplete))
        //        taskSummary.PercentageComplete = 0.0;
        //}

        private void UpdatePercentages(TaskGroupSummary taskSummary)
        {
            var parentRunningTotals = new RunningTotals();

            UpdateRunningTotals(taskSummary, parentRunningTotals);

            taskSummary.PercentageComplete = parentRunningTotals.TotalComplete / (parentRunningTotals.TotalComplete + parentRunningTotals.TotalInComplete);

            if (Double.IsNaN(taskSummary.PercentageComplete))
                taskSummary.PercentageComplete = 0.0;

            //foreach (var chidTaskGroup in taskSummary.ChildTaskGroups)
            //{
            //    var childRunningTotals = new RunningTotals();

            //    UpdateRunningTotals(chidTaskGroup, parentRunningTotals);

            //    chidTaskGroup.PercentageComplete = childRunningTotals.TotalComplete / (childRunningTotals.TotalComplete + childRunningTotals.TotalInComplete);

            //    if (Double.IsNaN(chidTaskGroup.PercentageComplete))
            //        chidTaskGroup.PercentageComplete = 0.0;
            //}
        }

        private void UpdateRunningTotals(TaskGroupSummary taskSummary, RunningTotals runningTotal)
        {
            runningTotal.TotalComplete += taskSummary.ChildTasks.Where(ct => ct.Completed).Count();
            runningTotal.TotalInComplete += taskSummary.ChildTasks.Where(ct => !ct.Completed).Count();

            if (taskSummary.Completed)
                runningTotal.TotalComplete += 1;

            if (!taskSummary.Completed)
                runningTotal.TotalInComplete += 1;

            foreach (var childTaskGroup in taskSummary.ChildTaskGroups)
            {
                UpdateRunningTotals(childTaskGroup, runningTotal);
            }
        }


        public async Task<TaskGroupList> GetTaskGroupListAsync(string accessToken)
        {
            var userTeams = await GetUserTeamsAsync(accessToken);

            TaskGroupList taskGroupList = new TaskGroupList();

            foreach (var currentTaskGroupHeader in _unitOfWork.TaskGroupHeaders.GetAll(true))
            {
                if (currentTaskGroupHeader.ParentTaskGroupId != null)
                {
                    var parentTaskGroup = _unitOfWork.TaskGroupHeaders.Get(currentTaskGroupHeader.ParentTaskGroupId ?? Guid.Empty);

                    if (parentTaskGroup != null)
                    {
                        if (userTeams.Contains(parentTaskGroup.TeamHeaderId))
                        {
                            taskGroupList.TaskGroupHeaders.Add(_mapper.Map<TaskGroupHead>(currentTaskGroupHeader));
                        }
                    }
                }
                else
                {
                    if (userTeams.Contains(currentTaskGroupHeader.TeamHeaderId))
                    {
                        taskGroupList.TaskGroupHeaders.Add(_mapper.Map<TaskGroupHead>(currentTaskGroupHeader));
                    }
                }
            }

            return taskGroupList;
        }

        public async Task<bool> SetNewTaskGroupSequenceNoAsync(TaskGroupHeader taskGroupHeader, string accessToken)
        {
            taskGroupHeader.SequenceNumber = 0;

            Guid parentTaskGroupId = taskGroupHeader.ParentTaskGroupId ?? Guid.Empty;
            if (parentTaskGroupId != Guid.Empty)
            {
                TaskGroupHeader parentTaskGroupHeader = await _unitOfWork.TaskGroupHeaders.GetTaskGroupHeaderWithRelationsAsync(parentTaskGroupId, true, accessToken);

                if (parentTaskGroupHeader != null)
                {
                    var maxChildTaskGroup = parentTaskGroupHeader.ChildTaskGroups.OrderByDescending(f => f.SequenceNumber).FirstOrDefault();

                    if (maxChildTaskGroup != null)
                    {
                        taskGroupHeader.SequenceNumber = maxChildTaskGroup.SequenceNumber + 1;
                    }
                }
            }

            return true;
        }

        public async Task<bool> SetNewTaskSequenceNoAsync(TaskHeader taskHeader, string accessToken)
        {
            taskHeader.SequenceNumber = 0;

            Guid parentTaskGroupHeaderId = taskHeader.TaskGroupHeaderId;
            if (parentTaskGroupHeaderId != Guid.Empty)
            {
                TaskGroupHeader parentTaskGroupHeader = await _unitOfWork.TaskGroupHeaders.GetTaskGroupHeaderWithRelationsAsync(parentTaskGroupHeaderId, true, accessToken);

                if (parentTaskGroupHeader != null)
                {
                    var maxChildTask = parentTaskGroupHeader.ChildTasks.OrderByDescending(f => f.SequenceNumber).FirstOrDefault();

                    if (maxChildTask != null)
                    {
                        taskHeader.SequenceNumber = maxChildTask.SequenceNumber + 1;
                    }
                }
            }

            return true;
        }

        public async Task<bool> TaskGroupParentOkayAsync(TaskGroupHeader taskGroupHeader, string accessToken)
        {
            Guid parentTaskGroupId = taskGroupHeader.ParentTaskGroupId ?? Guid.Empty;
            if (parentTaskGroupId != Guid.Empty)
            {
                TaskGroupHeader parentTaskGroupHeader = _unitOfWork.TaskGroupHeaders.Get(parentTaskGroupId);

                var userTeams = await GetUserTeamsAsync(accessToken);

                if (!userTeams.Contains(parentTaskGroupHeader.TeamHeaderId))
                    parentTaskGroupHeader = null;

                if (parentTaskGroupHeader == null)
                {
                    return false;
                }

                taskGroupHeader.ClientHeaderId = Guid.Empty;
                taskGroupHeader.TeamHeaderId = Guid.Empty;
                //taskGroupHeader.MainUserId = Guid.Empty;

                //Make sure the start and completion dates don't fall outside of the group headers dates
                //taskGroupHeader.StartDate = (taskGroupHeader.StartDate < parentTaskGroupHeader.StartDate) ? parentTaskGroupHeader.StartDate : taskGroupHeader.StartDate;
                //taskGroupHeader.CompletionDate = (taskGroupHeader.CompletionDate > parentTaskGroupHeader.CompletionDate) ? parentTaskGroupHeader.CompletionDate : taskGroupHeader.CompletionDate;
            }
            else
            {
                taskGroupHeader.ParentTaskGroupId = null;
            }

            return true;
        }

        public async Task<bool> TaskGroupTeamOkayAsync(TaskGroupHeader taskGroupHeader, string accessToken)
        {
            var userTeams = await GetUserTeamsAsync(accessToken);

            if (userTeams.Contains(taskGroupHeader.TeamHeaderId))
                return true;

            return false;
        }

        public async Task<bool> CanCompleteTaskGroupAsync(Guid taskGroupHeaderId, string accessToken)
        {
            var taskGroupHeader = await _unitOfWork.TaskGroupHeaders.GetTaskGroupHeaderWithRelationsAsync(taskGroupHeaderId, true, accessToken);

            if (taskGroupHeader != null)
            {
                if (taskGroupHeader.ChildTaskGroups.Any())
                {
                    foreach (var childTaskGroupHeader in taskGroupHeader.ChildTaskGroups)
                    {
                        if (!childTaskGroupHeader.Completed)
                        {
                            if (!await CanCompleteTaskGroupAsync(childTaskGroupHeader.Id, accessToken))
                                return false;
                        }
                    }
                }

                if (taskGroupHeader.ChildTasks.Any())
                {
                    if (taskGroupHeader.ChildTasks.Where(c => c.Completed == false).Any())
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        public async Task<bool> CanAccessTaskGroupHeaderAsync(Guid taskGroupHeaderId, string accessToken)
        {
            var userTeams = await GetUserTeamsAsync(accessToken);

            var taskGroupHeader = _unitOfWork.TaskGroupHeaders.Get(taskGroupHeaderId);

            if (taskGroupHeader != null)
            {
                if (taskGroupHeader.ParentTaskGroupId != null)
                {
                    return await CanAccessTaskGroupHeaderAsync(taskGroupHeader.ParentTaskGroupId ?? Guid.Empty, accessToken);
                }
                else
                {
                    if (userTeams.Contains(taskGroupHeader.TeamHeaderId))
                        return true;
                }
            }

            return false;
        }

        public async Task<Tuple<bool, string>> TaskGroupDatesOkayAsync(TaskGroupHeader taskGroupHeader, string accessToken)
        {
            var returnMessage = string.Empty;

            if (taskGroupHeader.ParentTaskGroupId.HasValue)
            {
                var parentTaskGroupHeader = _unitOfWork.TaskGroupHeaders.Get(taskGroupHeader.ParentTaskGroupId.GetValueOrDefault());

                if(parentTaskGroupHeader != default)
                {
                    if (taskGroupHeader.StartDate < parentTaskGroupHeader.StartDate)
                        returnMessage = ReturnMessageBuilder(returnMessage, $"{taskGroupHeader.ShortName}: Start Date is earlier than parent task group Start Date");

                    if (taskGroupHeader.CompletionDate > parentTaskGroupHeader.CompletionDate)
                        returnMessage = ReturnMessageBuilder(returnMessage, $"{taskGroupHeader.ShortName}: Completion Date is later than parent task group Completion Date");
                }
            }

            var dateTimeFormatString = "yyyyMMddHHmmss";
            DateTime.TryParseExact("19000101000000", dateTimeFormatString, null, DateTimeStyles.None, out DateTime convertedDateTime);

            if (taskGroupHeader.CompletedOnDate > convertedDateTime)
            {
                if (taskGroupHeader.CompletedOnDate < taskGroupHeader.StartDate)
                    returnMessage = ReturnMessageBuilder(returnMessage, $"{taskGroupHeader.ShortName}: Completed On Date is earlier than Start Date");


                if (taskGroupHeader.CompletedOnDate > taskGroupHeader.CompletionDate)
                    returnMessage = ReturnMessageBuilder(returnMessage, $"{taskGroupHeader.ShortName}: Completed On Date is later than Completion Date");
            }

            if (taskGroupHeader.Id != Guid.Empty)
            {
                var childTaskHeaders = await _unitOfWork.TaskGroupHeaders.GetChildTaskHeadersAsync(taskGroupHeader.Id, true);
                var childTaskGroupHeaders = await _unitOfWork.TaskGroupHeaders.GetChildTaskGroupHeadersAsync(taskGroupHeader.Id, false, true);

                foreach (var childTask in childTaskHeaders)
                {
                    if (childTask.StartDate < taskGroupHeader.StartDate)
                        returnMessage = ReturnMessageBuilder(returnMessage, $"Child Task - {childTask.Title}: Child Task Start Date is earlier than Start Date");

                    if (childTask.CompletionDate > taskGroupHeader.CompletionDate)
                        returnMessage = ReturnMessageBuilder(returnMessage, $"Child Task - {childTask.Title}: Child Task Completion Date is later than Completion Date");

                    if (taskGroupHeader.CompletedOnDate > convertedDateTime)
                    {
                        if (childTask.Completed)
                        {
                            if (childTask.CompletedOnDate > taskGroupHeader.CompletedOnDate)
                                returnMessage = ReturnMessageBuilder(returnMessage, $"Child Task - {childTask.Title}: Child Task Completion On Date is later than Completion On Date");
                        }
                        else
                        {
                            if (childTask.CompletionDate > taskGroupHeader.CompletedOnDate)
                                returnMessage = ReturnMessageBuilder(returnMessage, $"Child Task - {childTask.Title}: Child Task Completion Date is later than Completion On Date");
                        }
                    }
                }

                foreach (var childTaskGroup in childTaskGroupHeaders)
                {
                    if (childTaskGroup.StartDate < taskGroupHeader.StartDate)
                        returnMessage = ReturnMessageBuilder(returnMessage, $"Child Task Group - {childTaskGroup.ShortName}: Child Task Group Start Date is earlier than Start Date");

                    if (childTaskGroup.CompletionDate > taskGroupHeader.CompletionDate)
                        returnMessage = ReturnMessageBuilder(returnMessage, $"Child Task Group - {childTaskGroup.ShortName}: Child Task Group Completion Date is later than Completion Date");

                    if (taskGroupHeader.CompletedOnDate > convertedDateTime)
                    {
                        if (childTaskGroup.Completed)
                        {
                            if (childTaskGroup.CompletedOnDate > taskGroupHeader.CompletedOnDate)
                                returnMessage = ReturnMessageBuilder(returnMessage, $"Child Task Group - {childTaskGroup.ShortName}: Child Task Group Completion On Date is later than Completion On Date");
                        }
                        else
                        {
                            if (childTaskGroup.CompletionDate > taskGroupHeader.CompletedOnDate)
                                returnMessage = ReturnMessageBuilder(returnMessage, $"Child Task Group - {childTaskGroup.ShortName}: Child Task Group Completion Date is later than Completion On Date");
                        }
                    }
                }
            }

            return new Tuple<bool, string>(string.IsNullOrWhiteSpace(returnMessage), returnMessage);
        }

        public async Task<Tuple<bool, string>> TaskDatesOkayAsync(TaskHeader taskHeader)
        {
            var returnMessage = string.Empty;

            var taskGroupHeader = _unitOfWork.TaskGroupHeaders.Get(taskHeader.TaskGroupHeaderId);

            if (taskGroupHeader != default)
            {
                if (taskHeader.StartDate < taskGroupHeader.StartDate)
                    returnMessage = ReturnMessageBuilder(returnMessage, $"{taskHeader.Title}: Start Date is earlier than parent task group Start Date");

                if (taskHeader.CompletionDate > taskGroupHeader.CompletionDate)
                    returnMessage = ReturnMessageBuilder(returnMessage, $"{taskHeader.Title}: Completion Date is later than parent task group Completion Date");
            }

            var dateTimeFormatString = "yyyyMMddHHmmss";
            DateTime.TryParseExact("19000101000000", dateTimeFormatString, null, DateTimeStyles.None, out DateTime convertedDateTime);

            if (taskHeader.CompletedOnDate > convertedDateTime)
            {
                if (taskHeader.CompletedOnDate < taskHeader.StartDate)
                    returnMessage = ReturnMessageBuilder(returnMessage, $"{taskGroupHeader.ShortName}: Completed On Date is earlier than Start Date");


                if (taskHeader.CompletedOnDate > taskGroupHeader.CompletionDate)
                    returnMessage = ReturnMessageBuilder(returnMessage, $"{taskGroupHeader.ShortName}: Completed On Date is later than task group Completion Date");
            }

            return new Tuple<bool, string>(string.IsNullOrWhiteSpace(returnMessage), returnMessage);
        }

        private async Task<List<Guid>> GetUserTeamsAsync(string accessToken)
        {
            if(_userTeams == null)
                _userTeams = await _apiHelper.GetTeamsForUserId(accessToken, _userProvider.GetUserId());

            return _userTeams;
        }

        private string ReturnMessageBuilder(string existingMessage, string message)
        {
            if (string.IsNullOrWhiteSpace(existingMessage))
                return message;

            return $"{existingMessage}, {message}";
        }

        //private async Task<List<Guid>> GetTeamsForUser(string accessToken)
        //{
        //    var userTeams = new List<Guid>();

        //    var response = await _apiHelper.MakeAPICallAsync(accessToken, HttpMethod.Get, APITypes.GATEWAY, $"UserTeam/GetTeamsForUser/{_userProvider.GetUserId()}");

        //    if (response.StatusCode == System.Net.HttpStatusCode.OK)
        //    {
        //        TeamList teamList = response.ContentAsType<TeamList>();

        //        foreach (var teamHead in teamList.TeamHeaders)
        //        {
        //            userTeams.Add(teamHead.Id);
        //        }
        //    }

        //    return userTeams;
        //}
    }

    public class RunningTotals
    {
        public double TotalComplete { get; set; }
        public double TotalInComplete { get; set; }
    }
}
