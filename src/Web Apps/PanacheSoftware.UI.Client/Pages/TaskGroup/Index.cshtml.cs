using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Text.Json;
using AutoMapper;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
//using Newtonsoft.Json;
using PanacheSoftware.Core.Domain.API.Client;
using PanacheSoftware.Core.Domain.Identity.API;
using PanacheSoftware.Core.Domain.UI;
using PanacheSoftware.Core.Types;
using PanacheSoftware.Http;
using PanacheSoftware.UI.Core.Helpers;
using PanacheSoftware.UI.Core.PageModel;
using PanacheSoftware.Core.Domain.API.Language;
using PanacheSoftware.Core.Domain.API.Task;
using PanacheSoftware.Core.Domain.API.File;
using PanacheSoftware.Core.Domain.API.CustomField;
using System.Collections.Generic;

namespace PanacheSoftware.UI.Client.Pages.TaskGroup
{
    public class IndexModel : PageModel, IPanacheSoftwarePageModel
    {
        private readonly IAPIHelper _apiHelper;
        private readonly IRazorPartialToStringRenderer _renderer;
        private readonly IModelHelper _modelHelper;
        private readonly IMapper _mapper;
        [BindProperty(SupportsGet = true)]
        public string Id { get; set; }
        [BindProperty(SupportsGet = true)]
        public string Message { get; set; }
        [BindProperty(SupportsGet = true)]
        public string MessageType { get; set; }
        [BindProperty]
        public string taskGroupId { get; set; }
        [BindProperty]
        public string taskId { get; set; }
        public TaskGroupSummary taskGroupSummary { get; set; }
        public ClientSummary clientSummary { get; set; }
        public LangQueryList langQueryList { get; set; }
        public FileLnkList fileLnkList { get; set; }
        public string SaveState { get; set; }
        public string ErrorString { get; set; }
        public SaveMessageModel SaveMessageModel { get; set; }
        public GanttDataModel GanttDataModel { get; set; }
        public string GanttJSON { get; set; }
        public LangQueryList taskGroupSummarylangQueryList { get; set; }
        public LangQueryList taskSummarylangQueryList { get; set; }
        public CustomFieldGroupValuesModelList customFieldGroupValuesModelList { get; set; }

        public IndexModel(IAPIHelper apiHelper, IRazorPartialToStringRenderer renderer, IModelHelper modelHelper, IMapper mapper)
        {
            _apiHelper = apiHelper;
            _renderer = renderer;
            _modelHelper = modelHelper;
            _mapper = mapper;
        }

        public async Task<bool> PageConstructor(string saveState, string accessToken)
        {
            SaveState = saveState;

            langQueryList = await _apiHelper.MakeLanguageQuery(accessToken, "EN", new long[] { 10121, 10112, 10909, 10921, 10922, 10911, 10912, 10222, 10302, 10306, 10923, 10924, 10925, 10926, 10901, 10927, 10943 });

            taskGroupSummarylangQueryList = await _apiHelper.MakeLanguageQuery(accessToken, "EN", new long[] { 10921, 10928, 10911, 10912, 10929, 10930, 10931, 10936, 10937, 10938, 10939, 10940, 10941, 10942, 10943 });

            taskSummarylangQueryList = await _apiHelper.MakeLanguageQuery(accessToken, "EN", new long[] { 10921, 10928, 10911, 10912, 10932, 10933, 10934, 10935, 10943 });

            if (fileLnkList == null)
                fileLnkList = new FileLnkList();

            return true;
        }

        public async Task<IActionResult> OnGetAsync()
        {
            var accessToken = await HttpContext.GetTokenAsync("access_token");

            await PageConstructor(SaveStates.IGNORE, accessToken);

            if (!await _apiHelper.AuthCheck(accessToken, User.FindFirst("sub").Value))
            {
                return RedirectToPage("/Logout");
            }

            taskGroupSummary = new TaskGroupSummary();
            clientSummary = new ClientSummary();

            if (!string.IsNullOrWhiteSpace(Id))
            {
                var response = await _apiHelper.MakeAPICallAsync(accessToken, HttpMethod.Get, APITypes.TASK, $"TaskGroup/GetTaskGroupSummary/{Id}");

                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    taskGroupSummary = response.ContentAsType<TaskGroupSummary>();
                }
                else
                {
                    return RedirectToPage("/TaskGroup/TaskGroups");
                }
            }
            else
            {
                return RedirectToPage("/TaskGroup/TaskGroups");
            }

            if(taskGroupSummary != null)
            {
                await GetFileLinks(accessToken);

                var response = await _apiHelper.MakeAPICallAsync(accessToken, HttpMethod.Get, APITypes.CLIENT, $"Client/GetClientSummary/{taskGroupSummary.ClientHeaderId}");

                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    clientSummary = response.ContentAsType<ClientSummary>();
                }

                GanttDataModel = GanttHelper.GenerateGanttDataModel(taskGroupSummary); 
            }
            else
            {
                GanttDataModel = new GanttDataModel()
                {
                    Data = new GanttData[0],
                    Links = new GanttLink[0]
                };
            }

            var userList = await GetUsernames(accessToken);

            //Temporary fix for adding language code data to partial pages.
            taskGroupSummary.langQueryList = taskGroupSummarylangQueryList;
            taskGroupSummary.MainUserName = userList.Users.Where(u => u.Id == taskGroupSummary.MainUserId).FirstOrDefault().FullName;

            foreach (var childTaskGroupSummary in taskGroupSummary.ChildTaskGroups)
            {
                childTaskGroupSummary.langQueryList = taskGroupSummarylangQueryList;
                childTaskGroupSummary.MainUserName = userList.Users.Where(u => u.Id == childTaskGroupSummary.MainUserId).FirstOrDefault().FullName;

                foreach (var childTaskSummary in childTaskGroupSummary.ChildTasks)
                {
                    childTaskSummary.langQueryList = taskSummarylangQueryList;
                    childTaskSummary.MainUserName = userList.Users.Where(u => u.Id == childTaskSummary.MainUserId).FirstOrDefault().FullName;
                }
            }

            foreach (var childTaskSummary in taskGroupSummary.ChildTasks)
            {
                childTaskSummary.langQueryList = taskGroupSummarylangQueryList;
                childTaskSummary.MainUserName = userList.Users.Where(u => u.Id == childTaskSummary.MainUserId).FirstOrDefault().FullName;
            }

            GanttJSON = JsonSerializer.Serialize(GanttDataModel);

            customFieldGroupValuesModelList = await GetCustomFieldGroupModelList(accessToken);

            SaveMessageModel = await _apiHelper.GenerateSaveMessageModel(accessToken);

            return Page();
        }

        private async Task<CustomFieldGroupValuesModelList> GetCustomFieldGroupModelList(string accessToken)
        {
            var fieldGroupValuesList = new CustomFieldGroupValuesModelList();

            if(!Guid.TryParse(Id, out Guid parsedId))
                return fieldGroupValuesList;

            var mainTaskGroupList = await GetCustomFieldGroupModels(accessToken, parsedId, LinkTypes.TaskGroup);

            if (mainTaskGroupList.Any())
                fieldGroupValuesList.CustomFieldGroupValuesModels.AddRange(mainTaskGroupList);

            foreach (var taskGroup in taskGroupSummary.ChildTaskGroups)
            {
                var taskGroupList = await GetCustomFieldGroupModels(accessToken, taskGroup.Id, LinkTypes.TaskGroup);

                if (taskGroupList.Any())
                    fieldGroupValuesList.CustomFieldGroupValuesModels.AddRange(taskGroupList);

                foreach (var taskItem in taskGroup.ChildTasks)
                {
                    var taskList = await GetCustomFieldGroupModels(accessToken, taskItem.Id, LinkTypes.Task);

                    if (taskList.Any())
                        fieldGroupValuesList.CustomFieldGroupValuesModels.AddRange(taskList);
                }
            }

            return fieldGroupValuesList;
        }

        private async Task<List<CustomFieldGroupValuesModel>> GetCustomFieldGroupModels(string accessToken, Guid linkId, string linkType)
        {
            var fieldGroupValueModels = new List<CustomFieldGroupValuesModel>();

            var response = await _apiHelper.MakeAPICallAsync(accessToken, HttpMethod.Get, APITypes.CUSTOMFIELD, $"CustomFieldGroupLink/GetLinks/{linkType}/{linkId}");

            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                var customFieldGroupLnkList = response.ContentAsType<CustomFieldGroupLnkList>();

                foreach (var customFieldGroupLnk in customFieldGroupLnkList.CustomFieldGroupLinks)
                {
                    var customFieldGroupValuesModel = await GetCustomFieldGroupModel(accessToken, customFieldGroupLnk);

                    if (customFieldGroupValuesModel != null)
                        fieldGroupValueModels.Add(customFieldGroupValuesModel);
                }
            }

            return fieldGroupValueModels;
        }

        private async Task<CustomFieldGroupValuesModel> GetCustomFieldGroupModel(string accessToken, CustomFieldGroupLnk customFieldGroupLnk)
        {
            var fieldGroupValuesModel = new CustomFieldGroupValuesModel();

            fieldGroupValuesModel.LinkId = customFieldGroupLnk.LinkId;
            fieldGroupValuesModel.LinkType = customFieldGroupLnk.LinkType;
            fieldGroupValuesModel.customFieldGroupHeader = customFieldGroupLnk.CustomFieldGroupHeader;

            var response = await _apiHelper.MakeAPICallAsync(accessToken, HttpMethod.Get, APITypes.CUSTOMFIELD, $"CustomFieldValue/GetValuesForLink/{fieldGroupValuesModel.LinkType}/{fieldGroupValuesModel.LinkId}");

            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                var customFieldValueList = response.ContentAsType<CustomFieldValList>();

                foreach (var customFieldVal in customFieldValueList.CustomFieldValues)
                {
                    var foundOpenCustomField = customFieldGroupLnk.CustomFieldGroupHeader.CustomFieldHeaders.Where(h => h.Id == customFieldVal.CustomFieldHeaderId && h.Status == StatusTypes.Open).FirstOrDefault();

                    if(foundOpenCustomField != default)
                    {
                        fieldGroupValuesModel.CustomFieldValues.Add(customFieldVal);
                    }
                }
            }

            return fieldGroupValuesModel;
        }

        private async Task<UserListModel> GetUsernames(string accessToken)
        {
            var userListModel = new UserListModel();

            var response = await _apiHelper.MakeAPICallAsync(accessToken, HttpMethod.Get, APITypes.IDENTITY, $"User");

            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                userListModel = response.ContentAsType<UserListModel>();
            }

            return userListModel;
        }

        public async Task<IActionResult> OnPostCompleteAsync()
        {
            if (ModelState.IsValid)
            {
                var accessToken = await HttpContext.GetTokenAsync("access_token");

                await PageConstructor(SaveStates.IGNORE, accessToken);

                if (!await _apiHelper.AuthCheck(accessToken, User.FindFirst("sub").Value))
                {
                    return RedirectToPage("/Logout");
                }

                if (!string.IsNullOrWhiteSpace(taskGroupId))
                {
                    if (Guid.TryParse(taskGroupId, out Guid parsedId))
                    {
                        if (parsedId != Guid.Empty)
                        {
                            var response = await _apiHelper.MakeAPICallAsync(accessToken, HttpMethod.Get, APITypes.TASK, $"TaskGroup/{parsedId}");

                            if (response.StatusCode == System.Net.HttpStatusCode.OK)
                            {
                                var taskGroupHead = response.ContentAsType<TaskGroupHead>();

                                if (!string.IsNullOrWhiteSpace(taskId))
                                {
                                    if (Guid.TryParse(taskId, out Guid parsedTaskId))
                                    {
                                        if (parsedTaskId != Guid.Empty)
                                        {
                                            var childTask = taskGroupHead.ChildTasks.Where(c => c.Id == parsedTaskId).FirstOrDefault();

                                            if (childTask != null)
                                            {
                                                response = await _apiHelper.MakeAPICallAsync(accessToken, HttpMethod.Post, APITypes.TASK, $"Task/Complete/{parsedTaskId}");

                                                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                                                {
                                                    return RedirectToPage($"/TaskGroup/Index", new { Id = taskGroupHead.ParentTaskGroupId, Message = $"Completed: {childTask.Title}", Messagetype = SaveStates.SUCCESS });
                                                }
                                                else
                                                {
                                                    return RedirectToPage($"/TaskGroup/Index", new { Id = taskGroupHead.ParentTaskGroupId, Message = $"Unable to complete: {childTask.Title}", Messagetype = SaveStates.FAILED });
                                                }
                                            }
                                        }
                                    }
                                }
                                else
                                {
                                    response = await _apiHelper.MakeAPICallAsync(accessToken, HttpMethod.Post, APITypes.TASK, $"TaskGroup/Complete/{parsedId}");

                                    if (response.StatusCode == System.Net.HttpStatusCode.OK)
                                    {
                                        if (taskGroupHead.ParentTaskGroupId != null)
                                        {
                                            return RedirectToPage($"/TaskGroup/Index", new { Id = taskGroupHead.ParentTaskGroupId, Message = $"Completed: {taskGroupHead.LongName}", Messagetype = SaveStates.SUCCESS });
                                        }
                                        else
                                        {
                                            return RedirectToPage($"/TaskGroup/Index", new { Id = taskGroupHead.Id, Message = $"Completed: {taskGroupHead.LongName}", Messagetype = SaveStates.SUCCESS });
                                        }
                                    }
                                    else
                                    {
                                        if (taskGroupHead.ParentTaskGroupId != null)
                                        {
                                            return RedirectToPage($"/TaskGroup/Index", new { Id = taskGroupHead.ParentTaskGroupId, Message = $"Unable to complete: {taskGroupHead.LongName}", Messagetype = SaveStates.FAILED });
                                        }
                                        else
                                        {
                                            return RedirectToPage($"/TaskGroup/Index", new { Id = taskGroupHead.Id, Message = $"Unable to complete: {taskGroupHead.LongName}", Messagetype = SaveStates.FAILED });
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                else
                {
                    return RedirectToPage("/TaskGroup/TaskGroups");
                }

                return RedirectToPage("/TaskGroup/TaskGroups");
            }

            return RedirectToPage("/TaskGroup/TaskGroups");
        }

        private async Task<bool> GetFileLinks(string accessToken)
        {
            foreach (var childTaskGroup in taskGroupSummary.ChildTaskGroups)
            {
                childTaskGroup.FileList = await GetLinksForTask(accessToken, LinkTypes.TaskGroup, childTaskGroup.Id);

                foreach (var childTask in childTaskGroup.ChildTasks)
                {
                    childTask.FileList = await GetLinksForTask(accessToken, LinkTypes.Task, childTask.Id);
                }
            }

            return true;
        }

        private async Task<FileList> GetLinksForTask(string accessToken, string linkType, Guid linkId)
        {
            var response = await _apiHelper.MakeAPICallAsync(accessToken, HttpMethod.Get, APITypes.FILE, $"File/Link/GetFilesForLink/{linkType}/{linkId}");

            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                FileList fileList = response.ContentAsType<FileList>();

                return fileList;
            }

            return new FileList();
        }
    }
}