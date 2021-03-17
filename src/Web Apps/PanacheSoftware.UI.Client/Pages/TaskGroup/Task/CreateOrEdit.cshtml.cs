using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using PanacheSoftware.Core.Domain.API.CustomField;
using PanacheSoftware.Core.Domain.API.Language;
using PanacheSoftware.Core.Domain.API.Task;
using PanacheSoftware.Core.Domain.Identity.API;
using PanacheSoftware.Core.Domain.UI;
using PanacheSoftware.Core.Types;
using PanacheSoftware.Http;
using PanacheSoftware.UI.Core.Helpers;
using PanacheSoftware.UI.Core.PageModel;

namespace PanacheSoftware.UI.Client.Pages.TaskGroup.Task
{
    public class CreateOrEditModel : PageModel, IPanacheSoftwarePageModel
    {
        private readonly IAPIHelper _apiHelper;
        private readonly IRazorPartialToStringRenderer _renderer;
        private readonly IModelHelper _modelHelper;
        private readonly IMapper _mapper;
        [BindProperty]
        public TaskItemModel taskItemModel { get; set; }
        [BindProperty(SupportsGet = true)]
        public string Id { get; set; }
        [BindProperty(SupportsGet = true)]
        public string TaskGroupId { get; set; }
        public CustomFieldGroupLinkTaskItemTableModel customFieldGroupLinkTaskItemTableModel { get; set; }
        public SelectList StatusList { get; }
        public SelectList UserSelectList { get; set; }
        public LangQueryList langQueryList { get; set; }
        public SelectList CustomFieldGroupList { get; set; }
        public string SaveState { get; set; }
        public string ErrorString { get; set; }
        public SaveMessageModel SaveMessageModel { get; set; }

        public CreateOrEditModel(IAPIHelper apiHelper, IRazorPartialToStringRenderer renderer, IModelHelper modelHelper, IMapper mapper)
        {
            _apiHelper = apiHelper;
            _renderer = renderer;
            _modelHelper = modelHelper;
            _mapper = mapper;

            StatusTypes statusTypes = new StatusTypes();
            StatusList = new SelectList(statusTypes.GetStatusTypesDictionary(), "Key", "Value");
        }

        public async Task<bool> PageConstructor(string saveState, string accessToken)
        {
            SaveState = saveState;

            await CreateUserSelectList(accessToken);
            await CreateCustomFieldGroupSelectList(accessToken);

            langQueryList = await _apiHelper.MakeLanguageQuery(accessToken, "EN", new long[] { 10121, 10111, 10500, 10501, 10900, 10901, 10200, 10222, 10203, 10902, 10903, 10904, 10905, 10201, 10906, 10907, 10908, 10909, 10910, 10911, 10918, 10917, 11001, 11006, 11010 });

            await GeneratePageConstructionModel();

            customFieldGroupLinkTaskItemTableModel.langQueryList = langQueryList;
            customFieldGroupLinkTaskItemTableModel.StatusList = StatusList;
            customFieldGroupLinkTaskItemTableModel.CustomFieldGroupSelectList = CustomFieldGroupList;

            return true;
        }

        public async Task<IActionResult> OnGetAsync()
        {
            var accessToken = await HttpContext.GetTokenAsync("access_token");

            if (!await _apiHelper.AuthCheck(accessToken, User.FindFirst("sub").Value))
            {
                return RedirectToPage("/Logout");
            }

            taskItemModel = new TaskItemModel();
            taskItemModel.taskHead = new TaskHead();
            customFieldGroupLinkTaskItemTableModel = new CustomFieldGroupLinkTaskItemTableModel();

            await PageConstructor(SaveStates.IGNORE, accessToken);

            if (!string.IsNullOrWhiteSpace(Id))
            {
                if (Guid.TryParse(Id, out Guid parsedId))
                {
                    if (parsedId != Guid.Empty)
                    {
                        var response = await _apiHelper.MakeAPICallAsync(accessToken, HttpMethod.Get, APITypes.TASK, $"Task/{Id}");

                        if (response.StatusCode == System.Net.HttpStatusCode.OK)
                        {
                            taskItemModel.taskHead = response.ContentAsType<TaskHead>();

                            response = await _apiHelper.MakeAPICallAsync(accessToken, HttpMethod.Get, APITypes.CUSTOMFIELD, $"CustomFieldGroupLink/GetLinks/{LinkTypes.TaskGroup}/{taskItemModel.taskHead.Id}");

                            if (response.StatusCode == System.Net.HttpStatusCode.OK)
                            {
                                var customFieldGroupLnkList = response.ContentAsType<CustomFieldGroupLnkList>();

                                foreach (var customFieldGroupLnk in customFieldGroupLnkList.CustomFieldGroupLinks)
                                {
                                    taskItemModel.customFieldGroupLinks.Add(customFieldGroupLnk);
                                }
                            }
                        }
                    }
                    else
                    {
                        if (!string.IsNullOrWhiteSpace(TaskGroupId))
                        {
                            var response = await _apiHelper.MakeAPICallAsync(accessToken, HttpMethod.Get, APITypes.TASK, $"TaskGroup/{TaskGroupId}");

                            if (response.StatusCode == System.Net.HttpStatusCode.OK)
                            {
                                var taskGroupHead = response.ContentAsType<TaskGroupHead>();
                                taskItemModel.taskHead.TaskGroupHeaderId = taskGroupHead.Id;
                                taskItemModel.taskHead.StartDate = taskGroupHead.StartDate;
                                taskItemModel.taskHead.CompletionDate = taskGroupHead.CompletionDate;
                                taskItemModel.taskHead.MainUserId = taskGroupHead.MainUserId;
                            }
                        }
                        else
                        {
                            return RedirectToPage("/TaskGroup/TaskGroups");
                        }
                    }
                }
            }
            else
            {
                return RedirectToPage("/TaskGroup/TaskGroups");
            }

            customFieldGroupLinkTaskItemTableModel.taskItemModel = taskItemModel;
            SaveMessageModel = await _apiHelper.GenerateSaveMessageModel(accessToken);

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var accessToken = await HttpContext.GetTokenAsync("access_token");
            customFieldGroupLinkTaskItemTableModel = new CustomFieldGroupLinkTaskItemTableModel();

            await PageConstructor(SaveStates.IGNORE, accessToken);

            customFieldGroupLinkTaskItemTableModel.taskItemModel = taskItemModel;

            if (ModelState.IsValid)
            {
                if (taskItemModel.taskHead != null)
                {
                    if (await CreateOrUpdateTaskAsync(accessToken))
                    {
                        SaveState = SaveStates.SUCCESS;
                        SaveMessageModel = await _apiHelper.GenerateSaveMessageModel(accessToken, SaveState);
                        return Page();
                    }
                }
            }

            Id = taskItemModel.taskHead.Id.ToString();
            TaskGroupId = taskItemModel.taskHead.TaskGroupHeaderId.ToString();

            SaveState = SaveStates.FAILED;

            SaveMessageModel = await _apiHelper.GenerateSaveMessageModel(accessToken, SaveState);

            return Page();
        }

        private async Task<bool> CreateUserSelectList(string accessToken)
        {
            var response = await _apiHelper.MakeAPICallAsync(accessToken, HttpMethod.Get, APITypes.IDENTITY, $"User");

            Dictionary<string, string> UserListDictionary = new Dictionary<string, string>();
            UserListDictionary.Add(Guid.Empty.ToString(), "None");

            if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            {
                return false;
            }
            else if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                var userListModel = response.ContentAsType<UserListModel>();

                foreach (var userDetail in userListModel.Users.OrderBy(u => u.FullName))
                {
                    UserListDictionary.Add(userDetail.Id.ToString(), userDetail.FullName);
                }
            }

            UserSelectList = new SelectList(UserListDictionary, "Key", "Value");

            return true;
        }

        private async Task<bool> CreateCustomFieldGroupSelectList(string accessToken)
        {
            var response = await _apiHelper.MakeAPICallAsync(accessToken, HttpMethod.Get, APITypes.CUSTOMFIELD, $"CustomFieldGroup");

            Dictionary<string, string> CustomFieldGroupListDictionary = new Dictionary<string, string>();

            if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            {
                return false;
            }
            else if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                var customFieldGroupList = response.ContentAsType<CustomFieldGroupList>();

                foreach (var customFieldGroupHeader in customFieldGroupList.CustomFieldGroupHeaders.OrderBy(h => h.ShortName))
                {
                    CustomFieldGroupListDictionary.Add(customFieldGroupHeader.Id.ToString(), customFieldGroupHeader.ShortName);
                }
            }

            CustomFieldGroupList = new SelectList(CustomFieldGroupListDictionary, "Key", "Value");

            return true;
        }

        private async Task<bool> CreateOrUpdateTaskAsync(string apiAccessToken)
        {
            if (taskItemModel.taskHead != null)
            {
                if (taskItemModel.taskHead.Id != Guid.Empty)
                {
                    var response = await _apiHelper.MakeAPICallAsync(apiAccessToken, HttpMethod.Get, APITypes.TASK, $"Task/{taskItemModel.taskHead.Id}");

                    if (response.StatusCode == System.Net.HttpStatusCode.OK)
                    {
                        var foundTaskHead = response.ContentAsType<TaskHead>();

                        if (foundTaskHead != null)
                        {
                            if (!await _modelHelper.ProcessPatch(foundTaskHead, taskItemModel.taskHead, foundTaskHead.Id, apiAccessToken, APITypes.TASK, "Task"))
                            {
                                return false;
                            }

                            response = await _apiHelper.MakeAPICallAsync(apiAccessToken, HttpMethod.Get, APITypes.TASK, $"Task/Detail/{taskItemModel.taskHead.TaskDetail.Id}");

                            if (response.StatusCode == System.Net.HttpStatusCode.OK)
                            {
                                var foundTaskDet = response.ContentAsType<TaskDet>();

                                if (foundTaskDet != null)
                                {
                                    if (!await _modelHelper.ProcessPatch(foundTaskDet, taskItemModel.taskHead.TaskDetail, foundTaskDet.Id, apiAccessToken, APITypes.TASK, "Task/Detail"))
                                    {
                                        return false;
                                    }
                                }
                            }

                            if (!await CreateOrUpdateCustomFieldLinksAsync(apiAccessToken, foundTaskHead.Id))
                                return false;
                        }
                    }
                }
                else
                {
                    HttpContent contentPost = new StringContent(JsonConvert.SerializeObject(taskItemModel.taskHead), Encoding.UTF8, "application/json");

                    try
                    {
                        var response = await _apiHelper.MakeAPICallAsync(apiAccessToken, HttpMethod.Post, APITypes.TASK, $"Task", contentPost);

                        if (response.StatusCode != System.Net.HttpStatusCode.Created)
                        {
                            return false;
                        }

                        var createdTaskHead = response.ContentAsType<TaskHead>();

                        if (createdTaskHead != null)
                        {
                            if (!await CreateOrUpdateCustomFieldLinksAsync(apiAccessToken, createdTaskHead.Id))
                                return false;
                        }
                    }
                    catch (Exception ex)
                    {
                        var responseString = $"Error calling API: {ex.Message}";
                        return false;
                    }
                }
            }

            return true;
        }

        private async Task<bool> CreateOrUpdateCustomFieldLinksAsync(string apiAccessToken, Guid taskHeadId)
        {
            foreach (var customFieldGroupLink in taskItemModel.customFieldGroupLinks)
            {
                if (customFieldGroupLink.Id != Guid.Empty && customFieldGroupLink.LinkId == taskHeadId)
                {
                    customFieldGroupLink.LinkType = LinkTypes.TaskGroup;

                    var response = await _apiHelper.MakeAPICallAsync(apiAccessToken, HttpMethod.Get, APITypes.CUSTOMFIELD, $"CustomFieldGroupLink/{customFieldGroupLink.Id}");

                    if (response.StatusCode == System.Net.HttpStatusCode.OK)
                    {
                        var foundCustomFieldGroupLink = response.ContentAsType<CustomFieldGroupLnk>();

                        if (foundCustomFieldGroupLink != null)
                        {
                            if (!await _modelHelper.ProcessPatch(foundCustomFieldGroupLink, customFieldGroupLink, foundCustomFieldGroupLink.Id, apiAccessToken, APITypes.CUSTOMFIELD, "CustomFieldGroupLink"))
                            {
                                return false;
                            }
                        }
                    }
                }
                else
                {
                    customFieldGroupLink.LinkId = taskHeadId;
                    customFieldGroupLink.LinkType = LinkTypes.TaskGroup;

                    HttpContent contentPost = new StringContent(JsonConvert.SerializeObject(customFieldGroupLink), Encoding.UTF8, "application/json");

                    try
                    {
                        var response = await _apiHelper.MakeAPICallAsync(apiAccessToken, HttpMethod.Post, APITypes.CUSTOMFIELD, $"CustomFieldGroupLink", contentPost);

                        if (response.StatusCode != System.Net.HttpStatusCode.Created)
                        {
                            return false;
                        }
                    }
                    catch (Exception ex)
                    {
                        ErrorString = $"Error calling API: {ex.Message}";
                        return false;
                    }
                }
            }

            return true;
        }

        private async Task<bool> GeneratePageConstructionModel()
        {
            var dummyCustomFieldGroupLinkTaskItemTableModel = new CustomFieldGroupLinkTaskItemTableModel()
            {
                taskItemModel = GenerateDummyTaskItemModel(),
                CustomFieldGroupSelectList = CustomFieldGroupList,
                StatusList = StatusList,
                langQueryList = langQueryList
            };

            var customFieldGroupLinkListHTML = await _renderer.RenderPartialToStringAsync("_CustomFieldGroupLinkTaskItem", dummyCustomFieldGroupLinkTaskItemTableModel);

            customFieldGroupLinkListHTML = customFieldGroupLinkListHTML.Replace("\n", "").Replace("\r", "").Replace("\t", "").ToString();

            var customFieldGroupLinkTableHTML = _renderer.GetStringBetween(customFieldGroupLinkListHTML, "<!--CustomFieldGroupLinkTableRowsStart-->", "<!--CustomFieldGroupLinkTableRowsEnd-->");

            List<string> customFieldGroupLinkListRows = customFieldGroupLinkTableHTML.TrimStart().TrimEnd().Replace("<td>", "").Split("</td>").ToList();

            for (int i = customFieldGroupLinkListRows.Count - 1; i >= 0; i--)
            {
                var eol = ",";

                customFieldGroupLinkListRows[i] = customFieldGroupLinkListRows[i].TrimStart().TrimEnd();
                if (string.IsNullOrWhiteSpace(customFieldGroupLinkListRows[i]))
                {
                    customFieldGroupLinkListRows.RemoveAt(i);
                    continue;
                }

                if (i == customFieldGroupLinkListRows.Count - 1)
                {
                    eol = "";
                }

                customFieldGroupLinkListRows[i] = customFieldGroupLinkListRows[i].Replace("taskItemModel_customFieldGroupLinks_0__", "taskItemModel_customFieldGroupLinks_' + rowCount + '__");
                customFieldGroupLinkListRows[i] = customFieldGroupLinkListRows[i].Replace("taskItemModel.customFieldGroupLinks[0]", "taskItemModel.customFieldGroupLinks[' + rowCount + ']");
                customFieldGroupLinkListRows[i] = customFieldGroupLinkListRows[i].Replace("11111111-1111-1111-1111-111111111111", "' + customFieldGroupHeadId + '");
                customFieldGroupLinkListRows[i] = customFieldGroupLinkListRows[i].Replace("22222222-2222-2222-2222-222222222222", "' + taskHeadId + '");
                customFieldGroupLinkListRows[i] = customFieldGroupLinkListRows[i].Replace("readonly", string.Empty);
                customFieldGroupLinkListRows[i] = customFieldGroupLinkListRows[i].Replace("disabled", string.Empty);

                customFieldGroupLinkListRows[i] = $"'{customFieldGroupLinkListRows[i]}'{eol}";
            }


            customFieldGroupLinkTaskItemTableModel.customFieldGroupRows = customFieldGroupLinkListRows.ToArray();

            return true;
        }

        private TaskItemModel GenerateDummyTaskItemModel()
        {
            var dummyTaskItemModel = new TaskItemModel();

            dummyTaskItemModel.taskHead = new TaskHead()
            {
                Id = Guid.Empty
            };

            dummyTaskItemModel.customFieldGroupLinks.Add(GenerateDummyCustomFieldGroupLink());

            return dummyTaskItemModel;
        }

        private CustomFieldGroupLnk GenerateDummyCustomFieldGroupLink()
        {
            var dummyCustomFieldGroupLnk = new CustomFieldGroupLnk()
            {
                CustomFieldGroupHeaderId = Guid.Parse("11111111-1111-1111-1111-111111111111"),
                Id = Guid.Empty,
                LinkId = Guid.Parse("22222222-2222-2222-2222-222222222222"),
                Status = StatusTypes.Open,
                LinkType = LinkTypes.TaskGroup
            };

            return dummyCustomFieldGroupLnk;
        }
    }
}