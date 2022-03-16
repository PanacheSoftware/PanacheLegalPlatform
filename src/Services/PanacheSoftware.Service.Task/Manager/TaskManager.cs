using AutoMapper;
using Microsoft.EntityFrameworkCore.Internal;
using Newtonsoft.Json;
using PanacheSoftware.Core.Domain.API.CustomField;
using PanacheSoftware.Core.Domain.API.File;
using PanacheSoftware.Core.Domain.API.Task;
using PanacheSoftware.Core.Domain.API.Team;
using PanacheSoftware.Core.Domain.Task;
using PanacheSoftware.Core.Helper;
using PanacheSoftware.Core.Types;
using PanacheSoftware.Http;
using PanacheSoftware.Service.Task.Core;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Text;
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

        public Tuple<bool, string> TaskGroupDatesOkay(TaskGroupHeader taskGroupHeader, string accessToken)
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
                var childTaskHeaders = _unitOfWork.TaskGroupHeaders.GetChildTaskHeaders(taskGroupHeader.Id, true);
                var childTaskGroupHeaders = _unitOfWork.TaskGroupHeaders.GetChildTaskGroupHeaders(taskGroupHeader.Id, false, true);

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

        public Tuple<bool, string> TaskDatesOkay(TaskHeader taskHeader)
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

        public async Task<Tuple<Guid, string>> CreateTaskFromTemplate(TaskGroupHead taskHeader, Guid TemplateId, string accessToken)
        {
            var returnResult = new Tuple<Guid, string>(Guid.Empty, string.Empty);

            var templateHeader = _unitOfWork.TemplateHeaders.GetTemplateHeaderWithRelations(TemplateId, true, accessToken);

            if (templateHeader == null)
                return new Tuple<Guid, string>(Guid.Empty, $"Id: {TemplateId}, Is not a template");

            var taskGroupHeader = _mapper.Map<TaskGroupHeader>(taskHeader);

            taskGroupHeader.CompletionDate = taskGroupHeader.StartDate.AddDays(templateHeader.TemplateDetail.TotalDays);

            var taskHeaderCreation = await CreateTaskGroupHeader(taskGroupHeader, accessToken);

            if (!taskHeaderCreation.Item1)
                return new Tuple<Guid, string>(Guid.Empty, taskHeaderCreation.Item2);

            var taskFileLinks = new List<FileHead>();
            var customFieldLinks = new List<CustomFieldGroupLnk>();

            await GetCustomFieldLinks(customFieldLinks, LinkTypes.Template, TemplateId, LinkTypes.TaskGroup, taskGroupHeader.Id, accessToken);

            foreach (var templateGroupHeader in templateHeader.TemplateGroupHeaders.OrderBy(s => s.SequenceNumber))
            {
                var childTaskGroupHead = new TaskGroupHead();
                childTaskGroupHead.Description = templateGroupHeader.Description;
                childTaskGroupHead.LongName = templateGroupHeader.LongName;
                childTaskGroupHead.MainUserId = taskGroupHeader.MainUserId;
                childTaskGroupHead.SequenceNumber = templateGroupHeader.SequenceNumber;
                childTaskGroupHead.ShortName = templateGroupHeader.ShortName;
                childTaskGroupHead.StartDate = taskGroupHeader.StartDate.AddDays(templateGroupHeader.TemplateGroupDetail.DaysOffset);
                childTaskGroupHead.CompletionDate = childTaskGroupHead.StartDate.AddDays(templateGroupHeader.TemplateGroupDetail.TotalDays);
                childTaskGroupHead.ParentTaskGroupId = taskGroupHeader.Id;
                childTaskGroupHead.TeamHeaderId = taskGroupHeader.TeamHeaderId;

                var childTaskGroupHeader = _mapper.Map<TaskGroupHeader>(childTaskGroupHead);
                
                var childTaskGroupCreation = await CreateTaskGroupHeader(childTaskGroupHeader, accessToken);

                if(!childTaskGroupCreation.Item1)
                    return new Tuple<Guid, string>(Guid.Empty, childTaskGroupCreation.Item2);

                await GetCustomFieldLinks(customFieldLinks, LinkTypes.Template, templateGroupHeader.Id, LinkTypes.TaskGroup, childTaskGroupHeader.Id, accessToken);

                foreach (var templateItemHeader in templateGroupHeader.TemplateItemHeaders)
                {
                    var childTaskItemHead = new TaskHead();
                    childTaskItemHead.Title = templateItemHeader.Title;
                    childTaskItemHead.TaskType = templateItemHeader.TaskType;
                    childTaskItemHead.Description = templateItemHeader.Description;
                    childTaskItemHead.StartDate = childTaskGroupHeader.StartDate.AddDays(templateItemHeader.TemplateItemDetail.DaysOffset);
                    childTaskItemHead.CompletionDate = childTaskItemHead.StartDate.AddDays(templateItemHeader.TemplateItemDetail.TotalDays);
                    childTaskItemHead.TaskGroupHeaderId = childTaskGroupHeader.Id;
                    childTaskItemHead.MainUserId = childTaskGroupHeader.MainUserId;

                    var childTaskItemHeader = _mapper.Map<TaskHeader>(childTaskItemHead);

                    var childTaskItemCreation = await CreateTaskHeader(childTaskItemHeader, accessToken);

                    if(!childTaskItemCreation.Item1)
                        return new Tuple<Guid, string>(Guid.Empty, childTaskItemCreation.Item2);

                    await GetCustomFieldLinks(customFieldLinks, LinkTypes.Template, templateItemHeader.Id, LinkTypes.Task, childTaskItemHeader.Id, accessToken);

                    var response = await _apiHelper.MakeAPICallAsync(accessToken, HttpMethod.Get, APITypes.FILE, $"File/Link/GetFilesForLink/{NodeTypes.Template}/{templateItemHeader.Id}");

                    if (response.IsSuccessStatusCode)
                    {
                        var fileList = response.ContentAsType<FileList>();

                        foreach (var fileHead in fileList.FileHeaders)
                        {
                            var fileHeadToCreate = new FileHead();
                            var fileVer = new FileVer();
                            var latestFileVerion = fileHead.FileVersions.OrderByDescending(v => v.VersionNumber).FirstOrDefault();

                            if (latestFileVerion != null)
                            {
                                fileHeadToCreate.FileDetail.FileTitle = fileHead.FileDetail.FileTitle;
                                fileHeadToCreate.FileDetail.Description = fileHead.FileDetail.Description;

                                if (!string.IsNullOrWhiteSpace(latestFileVerion.URI))
                                {
                                    fileHeadToCreate.FileDetail.FileExtension = "URI";
                                    fileHeadToCreate.FileDetail.FileType = "URI";
                                    fileVer.URI = latestFileVerion.URI;
                                    fileVer.VersionNumber = 0;
                                }
                                else
                                {
                                    fileHeadToCreate.FileDetail.FileExtension = fileHead.FileDetail.FileExtension;
                                    fileHeadToCreate.FileDetail.FileType = fileHead.FileDetail.FileType;
                                    fileVer.Content = latestFileVerion.Content;
                                    fileVer.UntrustedName = latestFileVerion.UntrustedName;
                                    fileVer.UploadDate = DateTime.Today;
                                    fileVer.Size = latestFileVerion.Size;
                                    fileVer.VersionNumber = 0;
                                }

                                fileHeadToCreate.FileVersions.Add(fileVer);

                                fileHeadToCreate.FileLinks.Add(new FileLnk()
                                {
                                    LinkId = childTaskItemHeader.Id,
                                    LinkType = LinkTypes.Task,
                                    FileHeaderId = fileHeadToCreate.Id
                                });
                            }

                            taskFileLinks.Add(fileHeadToCreate);
                        }
                    }
                }
            }

            foreach (var fileToCreate in taskFileLinks)
            {
                HttpContent contentPost = new StringContent(JsonConvert.SerializeObject(fileToCreate), Encoding.UTF8, "application/json");

                try
                {
                    var response = await _apiHelper.MakeAPICallAsync(accessToken, HttpMethod.Post, APITypes.FILE, $"File", contentPost);

                    if (response.StatusCode != System.Net.HttpStatusCode.Created)
                    {
                        return new Tuple<Guid, string>(Guid.Empty, $"Error creating file for Task Item: {fileToCreate.FileLinks.FirstOrDefault().LinkId}");
                    }
                }
                catch
                {
                    return new Tuple<Guid, string>(Guid.Empty, $"Error creating file for Task Item: {fileToCreate.FileLinks.FirstOrDefault().LinkId}");
                }
            }

            foreach (var customFieldGroupLink in customFieldLinks)
            {
                HttpContent contentPost = new StringContent(JsonConvert.SerializeObject(customFieldGroupLink), Encoding.UTF8, "application/json");

                try
                {
                    var response = await _apiHelper.MakeAPICallAsync(accessToken, HttpMethod.Post, APITypes.CUSTOMFIELD, $"CustomFieldGroupLink", contentPost);

                    if (response.StatusCode != System.Net.HttpStatusCode.Created)
                    {
                        return new Tuple<Guid, string>(Guid.Empty, $"Error creating custom filed group link for template: {customFieldGroupLink.LinkId}");
                    }
                }
                catch
                {
                    return new Tuple<Guid, string>(Guid.Empty, $"Error creating custom filed group link for template: {customFieldGroupLink.LinkId}");
                }
            }

            return new Tuple<Guid, string>(taskGroupHeader.Id, $"Created okay");
        }

        public async Task<Tuple<bool, string>> CreateTaskGroupHeader(TaskGroupHeader taskGroupHeader, string accessToken)
        {
            if(taskGroupHeader.Id != Guid.Empty)
                return new Tuple<bool, string>(false, $"TaskGroupHead.Id: '{taskGroupHeader.Id}' is not an empty guid.");

            if (taskGroupHeader.ChildTaskGroups.Any())
                return new Tuple<bool, string>(false, "Cannot create child task groups when creating parent");

            if (taskGroupHeader.ChildTasks.Any())
                return new Tuple<bool, string>(false, "Cannot create child tasks when creating task group");

            if (!await TaskGroupTeamOkayAsync(taskGroupHeader, accessToken))
                return new Tuple<bool, string>(false, $"TaskGroupHead.TeamHeaderId: Can't access '{taskGroupHeader.TeamHeaderId}'.");

            if (!await TaskGroupParentOkayAsync(taskGroupHeader, accessToken))
                return new Tuple<bool, string>(false, $"TaskGroupHead.ParentTaskGroupId: '{taskGroupHeader.ParentTaskGroupId}' is invalid.");

            var dateCheck = TaskGroupDatesOkay(taskGroupHeader, accessToken);

            if (!dateCheck.Item1)
                return new Tuple<bool, string>(false, dateCheck.Item2);

            if (!await SetNewTaskGroupSequenceNoAsync(taskGroupHeader, accessToken))
                return new Tuple<bool, string>(false, $"Unable to set task sequence number.");

            List<string> taskGroupHeadDuplicates = new List<string>();
            List<string> taskHeadDuplicates = new List<string>();

            SetTaskHeaderShortNames(taskGroupHeader);

            foreach (var childTaskGroupHeader in taskGroupHeader.ChildTaskGroups)
            {
                SetTaskHeaderShortNames(childTaskGroupHeader);
            }

            TaskGroupHead parentTaskGroupHead = null;

            if (taskGroupHeader.ParentTaskGroupId.HasValue)
            {
                var parentTaskGroupHeader = await _unitOfWork.TaskGroupHeaders.GetTaskGroupHeaderWithRelationsAsync(taskGroupHeader.ParentTaskGroupId.Value, true, accessToken);

                if(parentTaskGroupHeader != null)
                    parentTaskGroupHead = _mapper.Map<TaskGroupHead>(parentTaskGroupHeader);
            }

            if (parentTaskGroupHead != null)
            {
                parentTaskGroupHead.ChildTaskGroups.Add(_mapper.Map<TaskGroupHead>(taskGroupHeader));
                ValidateTaskShortNames(parentTaskGroupHead, taskGroupHeadDuplicates, taskHeadDuplicates);
            }
            else
            {
                ValidateTaskShortNames(_mapper.Map<TaskGroupHead>(taskGroupHeader), taskGroupHeadDuplicates, taskHeadDuplicates);
            }

            if(taskGroupHeadDuplicates.Any() || taskHeadDuplicates.Any())
                return new Tuple<bool, string>(false, $"Duplicate ShortNames found. Task Groups: '{string.Join(",", taskGroupHeadDuplicates)}', Tasks: '{string.Join(",", taskHeadDuplicates)}'");

            taskGroupHeader.OriginalCompletionDate = taskGroupHeader.CompletionDate;
            taskGroupHeader.CompletedOnDate = DateTime.Parse("01/01/1900");
            taskGroupHeader.Completed = false;

            if(taskGroupHeader.TaskGroupDetail == null)
                taskGroupHeader.TaskGroupDetail = new TaskGroupDetail();

            _unitOfWork.TaskGroupHeaders.Add(taskGroupHeader);

            _unitOfWork.Complete();

            return new Tuple<bool, string>(true, $"Created okay");
        }

        public void ValidateTaskShortNames(TaskGroupHead taskGroupHead, List<string> taskGroupHeadDuplicates, List<string> taskHeadDuplicates)
        {
            taskGroupHeadDuplicates.AddRange(DuplicateTaskGroupHeadShortNames(taskGroupHead));
            taskHeadDuplicates.AddRange(DuplicateTaskHeadShortNames(taskGroupHead));

            foreach (var childTaskGroupHead in taskGroupHead.ChildTaskGroups)
            {
                ValidateTaskShortNames(childTaskGroupHead, taskGroupHeadDuplicates, taskHeadDuplicates);
            }
        }

        public async Task<Tuple<bool, string>> CreateTaskHeader(TaskHeader taskHeader, string accessToken)
        {
            if (taskHeader.Id != Guid.Empty)
                return new Tuple<bool, string>(false, $"TaskHead.Id: '{taskHeader.Id}' is not an empty guid.");

            TaskGroupHeader taskGroupHeader =
                    await _unitOfWork.TaskGroupHeaders.GetTaskGroupHeaderWithRelationsAsync(taskHeader.TaskGroupHeaderId, true, accessToken);

            if(taskGroupHeader == null)
                return new Tuple<bool, string>(false, $"TaskHead.TaskGroupHeaderId: '{taskHeader.TaskGroupHeaderId}' is not valid.");

            if(!await CanAccessTaskGroupHeaderAsync(taskGroupHeader.Id, accessToken))
                return new Tuple<bool, string>(false, $"TaskHead.TaskGroupHeaderId: Can't access '{taskHeader.TaskGroupHeaderId}'.");

            var dateCheck = TaskDatesOkay(taskHeader);

            if (!dateCheck.Item1)
                return new Tuple<bool, string>(false, dateCheck.Item2);

            if (!await SetNewTaskSequenceNoAsync(taskHeader, accessToken))
                return new Tuple<bool, string>(false, $"Unable to set task sequence number.");

            taskHeader.ShortName = SetTaskHeaderShortName(taskHeader, taskGroupHeader);

            if(DuplicateTaskHeadShortName((List<TaskHeader>)taskGroupHeader.ChildTasks, taskHeader.ShortName))
                return new Tuple<bool, string>(false, $"Duplicate ShortName.");

            _unitOfWork.TaskHeaders.Add(taskHeader);

            _unitOfWork.Complete();

            return new Tuple<bool, string>(true, $"Created okay");
        }

        public async Task<bool> GetCustomFieldLinks(IList<CustomFieldGroupLnk> CustomFieldGroupLinks, string origlinkType, Guid origlinkId, string linkType, Guid linkId, string accessToken)
        {
            var response = await _apiHelper.MakeAPICallAsync(accessToken, HttpMethod.Get, APITypes.CUSTOMFIELD, $"CustomFieldGroupLink/GetLinks/{origlinkType}/{origlinkId}");

            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                var customFieldGroupLnkList = response.ContentAsType<CustomFieldGroupLnkList>();

                foreach (var customLnk in customFieldGroupLnkList.CustomFieldGroupLinks)
                {
                    var customFieldGroupLnk = new CustomFieldGroupLnk();

                    customFieldGroupLnk.CustomFieldGroupHeaderId = customLnk.CustomFieldGroupHeaderId;
                    customFieldGroupLnk.LinkId = linkId;
                    customFieldGroupLnk.LinkType = linkType;
                    
                    CustomFieldGroupLinks.Add(customFieldGroupLnk);
                }
            }

            return true;
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

        public void SetTaskHeaderShortNames(TaskGroupHeader taskGroupHeader)
        {
            foreach (var childTask in taskGroupHeader.ChildTasks)
            {
                if (string.IsNullOrWhiteSpace(childTask.ShortName))
                {
                    childTask.ShortName = SetTaskHeaderShortName(childTask, taskGroupHeader);
                }
            }
        }

        public string SetTaskHeaderShortName(TaskHeader taskHeader, TaskGroupHeader taskGroupHeader)
        {
            var newShortName = taskHeader.ShortName;

            if (string.IsNullOrWhiteSpace(taskHeader.ShortName))
            {
                for (int i = 0; i <= 999; i++)
                {
                    newShortName = Naming.CreateFieldShortName(taskHeader.Title, i);

                    if (!DuplicateTaskHeadShortName((List<TaskHeader>)taskGroupHeader.ChildTasks, newShortName))
                        break;

                    if (i == 999)
                        newShortName = string.Empty;
                }
            }

            return newShortName;
        }

        public bool DuplicateTaskHeadShortName(IList<TaskHeader> taskHeaders, string shortname)
        {
            var foundShortName = taskHeaders.FirstOrDefault(c => c.ShortName == shortname);

            return foundShortName != null;
        }

        public async Task<bool> TaskGroupHeadShortNameExists(Guid parentTaskGroupHeaderId, string shortName, string accessToken)
        {
            var parentTaskGroupHead = await _unitOfWork.TaskGroupHeaders.GetTaskGroupHeaderWithRelationsAsync(parentTaskGroupHeaderId, true, accessToken);

            if (parentTaskGroupHead == null)
                return false;

            var foundShortName = parentTaskGroupHead.ChildTaskGroups.FirstOrDefault(c => c.ShortName == shortName);

            return foundShortName != default;
        }

        public bool BlankShortNames(IList<TaskHead> taskHeads)
        {
            var blankShortNames = taskHeads.FirstOrDefault(c => c.ShortName == string.Empty);

            return blankShortNames != default;
        }

        public IList<string> DuplicateTaskHeadShortNames(TaskGroupHead taskGroupHead)
        {
            var duplicateShortNames = new List<string>();
            var groupedByShortName = taskGroupHead.ChildTasks.GroupBy(x => x.ShortName);
            var duplicates = groupedByShortName.Where(item => item.Count() > 1);

            foreach (var duplicate in duplicates)
            {
                if (duplicate.Count() > 0)
                    duplicateShortNames.Add(duplicate.Key);
            }

            return duplicateShortNames;
        }

        public IList<string> DuplicateTaskGroupHeadShortNames(TaskGroupHead taskGroupHead)
        {
            var duplicateShortNames = new List<string>();
            var groupedByShortName = taskGroupHead.ChildTaskGroups.GroupBy(x => x.ShortName);
            var duplicates = groupedByShortName.Where(item => item.Count() > 1);

            foreach (var duplicate in duplicates)
            {
                if (duplicate.Count() > 0)
                    duplicateShortNames.Add(duplicate.Key);
            }

            return duplicateShortNames;
        }
    }

    public class RunningTotals
    {
        public double TotalComplete { get; set; }
        public double TotalInComplete { get; set; }
    }
}
