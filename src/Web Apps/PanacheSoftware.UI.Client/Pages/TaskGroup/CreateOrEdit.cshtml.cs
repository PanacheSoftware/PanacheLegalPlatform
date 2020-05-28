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
        public TaskGroupHead taskGroupHead { get; set; }
        [BindProperty(SupportsGet = true)]
        public string Id { get; set; }
        [BindProperty(SupportsGet = true)]
        public string ParentId { get; set; }
        public SelectList StatusList { get; }
        public SelectList TeamSelectList { get; set; }
        public SelectList UserSelectList { get; set; }
        public SelectList ClientSelectList { get; set; }
        public SelectList TaskGroupSelectList { get; set; }
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
            await CreateTaskGroupSelectList(accessToken);
            await CreateUserSelectList(accessToken);
            await CreateClientSelectList(accessToken);

            langQueryList = await _apiHelper.MakeLanguageQuery(accessToken, "EN", new long[] { 10121, 10111, 10915, 10908, 10223, 10218, 10200, 10224, 10222, 10911, 10225, 10902, 10904, 10905, 10201, 10906, 10907, 10917, 10918 });

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

            taskGroupHead = new TaskGroupHead();
            taskGroupHead.MainUserId = Guid.Parse(User.FindFirst("sub").Value);

            if (!string.IsNullOrWhiteSpace(Id))
            {
                if(Guid.TryParse(Id, out Guid parsedId))
                {
                    if (parsedId != Guid.Empty)
                    {
                        var response = await _apiHelper.MakeAPICallAsync(accessToken, HttpMethod.Get, APITypes.TASK, $"TaskGroup/{Id}");

                        if (response.StatusCode == System.Net.HttpStatusCode.OK)
                        {
                            taskGroupHead = response.ContentAsType<TaskGroupHead>();
                            if(taskGroupHead != null)
                            {
                                ParentId = taskGroupHead.ParentTaskGroupId.ToString();
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
                                taskGroupHead = response.ContentAsType<TaskGroupHead>();
                                taskGroupHead.ShortName = string.Empty;
                                taskGroupHead.LongName = string.Empty;
                                taskGroupHead.Description = string.Empty;
                                taskGroupHead.ParentTaskGroupId = taskGroupHead.Id;
                                taskGroupHead.Id = Guid.Empty;
                            }
                        }
                    }
                }
            }
            else
            {
                Id = Guid.Empty.ToString();
            }

            SaveMessageModel = await _apiHelper.GenerateSaveMessageModel(accessToken);

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var accessToken = await HttpContext.GetTokenAsync("access_token");

            await PageConstructor(SaveStates.IGNORE, accessToken);

            if (ModelState.IsValid)
            {
                if (taskGroupHead != null)
                {
                    if(await CreateOrUpdateTaskGroupAsync(accessToken))
                    {
                        SaveState = SaveStates.SUCCESS;
                        SaveMessageModel = await _apiHelper.GenerateSaveMessageModel(accessToken, SaveState);
                        return Page();
                    }
                }
            }

            Id = taskGroupHead.Id.ToString();

            SaveState = SaveStates.FAILED;

            SaveMessageModel = await _apiHelper.GenerateSaveMessageModel(accessToken, SaveState);

            return Page();
        }

        private async Task<bool> CreateTeamSelectList(string accessToken)
        {
            var response = await _apiHelper.MakeAPICallAsync(accessToken, HttpMethod.Get, APITypes.TEAM, $"Team");

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

        private async Task<bool> CreateTaskGroupSelectList(string accessToken)
        {
            Dictionary<string, string> TaskGroupListDictionary = new Dictionary<string, string>();

            if (!string.IsNullOrWhiteSpace(ParentId))
            {
                var findTaskGroupResponse = await _apiHelper.MakeAPICallAsync(accessToken, HttpMethod.Get, APITypes.TASK, $"TaskGroup/{ParentId}");

                if (findTaskGroupResponse.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    return false;
                }
                else if (findTaskGroupResponse.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    TaskGroupHead taskGroupHead = findTaskGroupResponse.ContentAsType<TaskGroupHead>();

                    TaskGroupListDictionary.Add(taskGroupHead.Id.ToString(), taskGroupHead.ShortName);
                }
            }
            else
            {
                var response = await _apiHelper.MakeAPICallAsync(accessToken, HttpMethod.Get, APITypes.TASK, $"TaskGroup/GetValidParents/{Id}");

                TaskGroupListDictionary.Add("", "None");

                if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    return false;
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    TaskGroupList taskGroupList = response.ContentAsType<TaskGroupList>();

                    foreach (var taskGroupHeader in taskGroupList.TaskGroupHeaders.OrderBy(h => h.ShortName))
                    {
                        TaskGroupListDictionary.Add(taskGroupHeader.Id.ToString(), taskGroupHeader.ShortName);
                    }
                }
            }

            TaskGroupSelectList = new SelectList(TaskGroupListDictionary, "Key", "Value");

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

        private async Task<bool> CreateOrUpdateTaskGroupAsync(string apiAccessToken)
        {
            if (taskGroupHead != null)
            {
                if (taskGroupHead.Id != Guid.Empty)
                {
                    var response = await _apiHelper.MakeAPICallAsync(apiAccessToken, HttpMethod.Get, APITypes.TASK, $"TaskGroup/{taskGroupHead.Id}");

                    if (response.StatusCode == System.Net.HttpStatusCode.OK)
                    {
                        var foundTaskGroupHead = response.ContentAsType<TaskGroupHead>();

                        if (foundTaskGroupHead != null)
                        {
                            if (!await _modelHelper.ProcessPatch(foundTaskGroupHead, taskGroupHead, foundTaskGroupHead.Id, apiAccessToken, APITypes.TASK, "TaskGroup"))
                            {
                                return false;
                            }

                            response = await _apiHelper.MakeAPICallAsync(apiAccessToken, HttpMethod.Get, APITypes.TASK, $"TaskGroupDetail/{taskGroupHead.TaskGroupDetail.Id}");

                            if (response.StatusCode == System.Net.HttpStatusCode.OK)
                            {
                                var foundTaskGroupDet = response.ContentAsType<TaskGroupDet>();

                                if (foundTaskGroupDet != null)
                                {
                                    if (!await _modelHelper.ProcessPatch(foundTaskGroupDet, taskGroupHead.TaskGroupDetail, foundTaskGroupDet.Id, apiAccessToken, APITypes.TASK, "TaskGroupDetail"))
                                    {
                                        return false;
                                    }
                                }
                            }
                        }
                    }
                }
                else
                {
                    HttpContent contentPost = new StringContent(JsonConvert.SerializeObject(taskGroupHead), Encoding.UTF8, "application/json");

                    try
                    {
                        var response = await _apiHelper.MakeAPICallAsync(apiAccessToken, HttpMethod.Post, APITypes.TASK, $"TaskGroup", contentPost);

                        if (response.StatusCode != System.Net.HttpStatusCode.Created)
                        {
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
    }
}