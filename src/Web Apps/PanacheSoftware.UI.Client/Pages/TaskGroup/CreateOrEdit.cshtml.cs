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
using PanacheSoftware.Core.Domain.API.Client;
using PanacheSoftware.Core.Domain.API.CustomField;
using PanacheSoftware.Core.Domain.API.Language;
using PanacheSoftware.Core.Domain.API.Task;
using PanacheSoftware.Core.Domain.API.Team;
using PanacheSoftware.Core.Domain.Identity.API;
using PanacheSoftware.Core.Domain.UI;
using PanacheSoftware.Core.Types;
using PanacheSoftware.Http;
using PanacheSoftware.UI.Core.Helpers;
using PanacheSoftware.UI.Core.PageModel;

namespace PanacheSoftware.UI.Client.Pages.TaskGroup
{
    public class CreateOrEditModel : PageModel, IPanacheSoftwarePageModel
    {
        private readonly IAPIHelper _apiHelper;
        private readonly IRazorPartialToStringRenderer _renderer;
        private readonly IModelHelper _modelHelper;
        private readonly IMapper _mapper;
        [BindProperty]
        public TaskGroupModel taskGroupModel { get; set; }
        //public TaskGroupHead taskGroupHead { get; set; }
        [BindProperty(SupportsGet = true)]
        public string Id { get; set; }
        [BindProperty(SupportsGet = true)]
        public string ParentId { get; set; }
        public CustomFieldGroupLinkTaskGroupTableModel customFieldGroupLinkTaskGroupTableModel { get; set; }
        public SelectList StatusList { get; }
        public SelectList TeamSelectList { get; set; }
        public SelectList UserSelectList { get; set; }
        public SelectList ClientSelectList { get; set; }
        public SelectList CustomFieldGroupList { get; set; }
        public LangQueryList langQueryList { get; set; }
        public string SaveState { get; set; }
        public string ErrorString { get; set; }
        public SaveMessageModel SaveMessageModel { get; set; }

        //public string ChartDatasource { get; set; }

        public CreateOrEditModel(IAPIHelper apiHelper, IRazorPartialToStringRenderer renderer, IModelHelper modelHelper, IMapper mapper)
        {
            _apiHelper = apiHelper;
            _renderer = renderer;
            _modelHelper = modelHelper;
            _mapper = mapper;

            StatusTypes statusTypes = new StatusTypes();
            StatusList = new SelectList(statusTypes.GetStatusTypesDictionary(), "Key", "Value");
            //ChartDatasource = "{}";
        }

        public async Task<bool> PageConstructor(string saveState, string accessToken)
        {
            SaveState = saveState;

            await CreateTeamSelectList(accessToken);
            await CreateUserSelectList(accessToken);
            await CreateClientSelectList(accessToken);
            await CreateCustomFieldGroupSelectList(accessToken);

            langQueryList = await _apiHelper.MakeLanguageQuery(accessToken, "EN", new long[] { 10121, 10111, 10915, 10908, 10223, 10218, 10200, 10224, 10222, 10911, 10225, 10902, 10904, 10905, 10201, 10906, 10907, 10917, 10918, 11001, 11006, 11010 });

            await GeneratePageConstructionModel();

            customFieldGroupLinkTaskGroupTableModel.langQueryList = langQueryList;
            customFieldGroupLinkTaskGroupTableModel.StatusList = StatusList;
            customFieldGroupLinkTaskGroupTableModel.CustomFieldGroupSelectList = CustomFieldGroupList;

            return true;
        }

        public async Task<IActionResult> OnGetAsync()
        {
            var accessToken = await HttpContext.GetTokenAsync("access_token");

            if (!await _apiHelper.AuthCheck(accessToken, User.FindFirst("sub").Value))
            {
                return RedirectToPage("/Logout");
            }

            taskGroupModel = new TaskGroupModel();
            taskGroupModel.taskGroupHead = new TaskGroupHead();
            customFieldGroupLinkTaskGroupTableModel = new CustomFieldGroupLinkTaskGroupTableModel();
            taskGroupModel.taskGroupHead.MainUserId = Guid.Parse(User.FindFirst("sub").Value);

            await PageConstructor(SaveStates.IGNORE, accessToken);

            if (!string.IsNullOrWhiteSpace(Id))
            {
                if(Guid.TryParse(Id, out Guid parsedId))
                {
                    if (parsedId != Guid.Empty)
                    {
                        var response = await _apiHelper.MakeAPICallAsync(accessToken, HttpMethod.Get, APITypes.TASK, $"TaskGroup/{Id}");

                        if (response.StatusCode == System.Net.HttpStatusCode.OK)
                        {
                            taskGroupModel.taskGroupHead = response.ContentAsType<TaskGroupHead>();
                            
                            if(taskGroupModel.taskGroupHead != null)
                            {
                                ParentId = taskGroupModel.taskGroupHead.ParentTaskGroupId.ToString();

                                response = await _apiHelper.MakeAPICallAsync(accessToken, HttpMethod.Get, APITypes.CUSTOMFIELD, $"CustomFieldGroupLink/GetLinks/{LinkTypes.TaskGroup}/{taskGroupModel.taskGroupHead.Id}");

                                if(response.StatusCode == System.Net.HttpStatusCode.OK)
                                {
                                    var customFieldGroupLnkList = response.ContentAsType<CustomFieldGroupLnkList>();

                                    foreach (var customFieldGroupLnk in customFieldGroupLnkList.CustomFieldGroupLinks)
                                    {
                                        taskGroupModel.customFieldGroupLinks.Add(customFieldGroupLnk);
                                    }
                                }

                            }
                        }
                    }
                    else
                    {
                        if(!string.IsNullOrWhiteSpace(ParentId))
                        {
                            var response = await _apiHelper.MakeAPICallAsync(accessToken, HttpMethod.Get, APITypes.TASK, $"TaskGroup/{ParentId}");

                            if (response.StatusCode == System.Net.HttpStatusCode.OK)
                            {
                                taskGroupModel.taskGroupHead = response.ContentAsType<TaskGroupHead>();
                                taskGroupModel.taskGroupHead.ShortName = string.Empty;
                                taskGroupModel.taskGroupHead.LongName = string.Empty;
                                taskGroupModel.taskGroupHead.Description = string.Empty;
                                taskGroupModel.taskGroupHead.ParentTaskGroupId = taskGroupModel.taskGroupHead.Id;
                                taskGroupModel.taskGroupHead.Id = Guid.Empty;
                            }
                        }
                    }
                }
            }
            else
            {
                Id = Guid.Empty.ToString();
            }

            customFieldGroupLinkTaskGroupTableModel.taskGroupModel = taskGroupModel;
            SaveMessageModel = await _apiHelper.GenerateSaveMessageModel(accessToken);

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var accessToken = await HttpContext.GetTokenAsync("access_token");
            customFieldGroupLinkTaskGroupTableModel = new CustomFieldGroupLinkTaskGroupTableModel();

            await PageConstructor(SaveStates.IGNORE, accessToken);

            customFieldGroupLinkTaskGroupTableModel.taskGroupModel = taskGroupModel;

            if (ModelState.IsValid)
            {
                if (taskGroupModel.taskGroupHead != null)
                {
                    if(await CreateOrUpdateTaskGroupAsync(accessToken))
                    {
                        SaveState = SaveStates.SUCCESS;
                        SaveMessageModel = await _apiHelper.GenerateSaveMessageModel(accessToken, SaveState);
                        return Page();
                    }
                }
            }

            Id = taskGroupModel.taskGroupHead.Id.ToString();

            SaveState = SaveStates.FAILED;

            SaveMessageModel = await _apiHelper.GenerateSaveMessageModel(accessToken, SaveState);

            return Page();
        }

        private async Task<bool> CreateTeamSelectList(string accessToken)
        {
            var response = await _apiHelper.MakeAPICallAsync(accessToken, HttpMethod.Get, APITypes.TEAM, $"UserTeam/GetTeamsForUser/{User.FindFirst("sub").Value}");

            Dictionary<string, string> TeamListDictionary = new Dictionary<string, string>();
            TeamListDictionary.Add(Guid.Empty.ToString(), "None");

            if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            {
                return false;
            }
            else if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                TeamList teamList = response.ContentAsType<TeamList>();

                foreach (var teamHeader in teamList.TeamHeaders.OrderBy(h => h.ShortName))
                {
                    TeamListDictionary.Add(teamHeader.Id.ToString(), teamHeader.ShortName);
                }
            }

            TeamSelectList = new SelectList(TeamListDictionary, "Key", "Value");

            return true;
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

        private async Task<bool> CreateClientSelectList(string accessToken)
        {
            var response = await _apiHelper.MakeAPICallAsync(accessToken, HttpMethod.Get, APITypes.CLIENT, $"Client");

            Dictionary<string, string> ClientListDictionary = new Dictionary<string, string>();
            ClientListDictionary.Add(Guid.Empty.ToString(), "None");

            if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            {
                return false;
            }
            else if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                var clientListModel = response.ContentAsType<ClientList>();

                foreach (var clientDetail in clientListModel.ClientHeaders.OrderBy(c => c.LongName))
                {
                    ClientListDictionary.Add(clientDetail.Id.ToString(), clientDetail.LongName);
                }
            }

            ClientSelectList = new SelectList(ClientListDictionary, "Key", "Value");

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

        private async Task<bool> CreateOrUpdateTaskGroupAsync(string apiAccessToken)
        {
            if (taskGroupModel.taskGroupHead != null)
            {
                if (taskGroupModel.taskGroupHead.Id != Guid.Empty)
                {
                    var response = await _apiHelper.MakeAPICallAsync(apiAccessToken, HttpMethod.Get, APITypes.TASK, $"TaskGroup/{taskGroupModel.taskGroupHead.Id}");

                    if (response.StatusCode == System.Net.HttpStatusCode.OK)
                    {
                        var foundTaskGroupHead = response.ContentAsType<TaskGroupHead>();

                        if (foundTaskGroupHead != null)
                        {
                            if (!await _modelHelper.ProcessPatch(foundTaskGroupHead, taskGroupModel.taskGroupHead, foundTaskGroupHead.Id, apiAccessToken, APITypes.TASK, "TaskGroup"))
                            {
                                return false;
                            }

                            response = await _apiHelper.MakeAPICallAsync(apiAccessToken, HttpMethod.Get, APITypes.TASK, $"TaskGroup/Detail/{taskGroupModel.taskGroupHead.TaskGroupDetail.Id}");

                            if (response.StatusCode == System.Net.HttpStatusCode.OK)
                            {
                                var foundTaskGroupDet = response.ContentAsType<TaskGroupDet>();

                                if (foundTaskGroupDet != null)
                                {
                                    if (!await _modelHelper.ProcessPatch(foundTaskGroupDet, taskGroupModel.taskGroupHead.TaskGroupDetail, foundTaskGroupDet.Id, apiAccessToken, APITypes.TASK, "TaskGroup/Detail"))
                                    {
                                        return false;
                                    }
                                }
                            }

                            if (!await CreateOrUpdateCustomFieldLinksAsync(apiAccessToken, foundTaskGroupHead.Id))
                                return false;
                        }
                    }
                }
                else
                {
                    HttpContent contentPost = new StringContent(JsonConvert.SerializeObject(taskGroupModel.taskGroupHead), Encoding.UTF8, "application/json");

                    try
                    {
                        var response = await _apiHelper.MakeAPICallAsync(apiAccessToken, HttpMethod.Post, APITypes.TASK, $"TaskGroup", contentPost);

                        if (response.StatusCode != System.Net.HttpStatusCode.Created)
                        {
                            return false;
                        }

                        var createdTaskGroup = response.ContentAsType<TaskGroupHead>();

                        if(createdTaskGroup != null)
                        {
                            if (!await CreateOrUpdateCustomFieldLinksAsync(apiAccessToken, createdTaskGroup.Id))
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

        private async Task<bool> CreateOrUpdateCustomFieldLinksAsync(string apiAccessToken, Guid taskGroupHeadId)
        {
            foreach (var customFieldGroupLink in taskGroupModel.customFieldGroupLinks)
            {
                if (customFieldGroupLink.Id != Guid.Empty && customFieldGroupLink.LinkId == taskGroupHeadId)
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
                    customFieldGroupLink.LinkId = taskGroupHeadId;
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
            var dummyCustomFieldGroupLinkTaskGroupTableModel = new CustomFieldGroupLinkTaskGroupTableModel()
            {
                taskGroupModel = GenerateDummyTaskGroupModel(),
                CustomFieldGroupSelectList = CustomFieldGroupList,
                StatusList = StatusList,
                langQueryList = langQueryList
            };

            var customFieldGroupLinkListHTML = await _renderer.RenderPartialToStringAsync("_CustomFieldGroupLinkTaskGroup", dummyCustomFieldGroupLinkTaskGroupTableModel);

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

                customFieldGroupLinkListRows[i] = customFieldGroupLinkListRows[i].Replace("taskGroupModel_customFieldGroupLinks_0__", "taskGroupModel_customFieldGroupLinks_' + rowCount + '__");
                customFieldGroupLinkListRows[i] = customFieldGroupLinkListRows[i].Replace("taskGroupModel.customFieldGroupLinks[0]", "taskGroupModel.customFieldGroupLinks[' + rowCount + ']");
                customFieldGroupLinkListRows[i] = customFieldGroupLinkListRows[i].Replace("11111111-1111-1111-1111-111111111111", "' + customFieldGroupHeadId + '");
                customFieldGroupLinkListRows[i] = customFieldGroupLinkListRows[i].Replace("22222222-2222-2222-2222-222222222222", "' + taskGroupHeadId + '");
                customFieldGroupLinkListRows[i] = customFieldGroupLinkListRows[i].Replace("readonly", string.Empty);
                customFieldGroupLinkListRows[i] = customFieldGroupLinkListRows[i].Replace("disabled", string.Empty);

                customFieldGroupLinkListRows[i] = $"'{customFieldGroupLinkListRows[i]}'{eol}";
            }


            customFieldGroupLinkTaskGroupTableModel.customFieldGroupRows = customFieldGroupLinkListRows.ToArray();

            return true;
        }

        private TaskGroupModel GenerateDummyTaskGroupModel()
        {
            var dummyTaskGroupModel = new TaskGroupModel();

            dummyTaskGroupModel.taskGroupHead = new TaskGroupHead()
            {
                Id = Guid.Empty
            };

            dummyTaskGroupModel.customFieldGroupLinks.Add(GenerateDummyCustomFieldGroupLink());

            return dummyTaskGroupModel;
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