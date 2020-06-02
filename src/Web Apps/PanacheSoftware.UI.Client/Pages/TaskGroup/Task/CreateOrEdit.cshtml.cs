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
        public TaskHead taskHead { get; set; }
        [BindProperty(SupportsGet = true)]
        public string Id { get; set; }
        [BindProperty(SupportsGet = true)]
        public string TaskGroupId { get; set; }
        public SelectList StatusList { get; }
        public SelectList UserSelectList { get; set; }
        public LangQueryList langQueryList { get; set; }
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

        public async Task<IActionResult> OnGetAsync()
        {
            var accessToken = await HttpContext.GetTokenAsync("access_token");

            await PageConstructor(SaveStates.IGNORE, accessToken);

            if (!await _apiHelper.AuthCheck(accessToken, User.FindFirst("sub").Value))
            {
                return RedirectToPage("/Logout");
            }

            taskHead = new TaskHead();

            if (!string.IsNullOrWhiteSpace(Id))
            {
                if (Guid.TryParse(Id, out Guid parsedId))
                {
                    if (parsedId != Guid.Empty)
                    {
                        var response = await _apiHelper.MakeAPICallAsync(accessToken, HttpMethod.Get, APITypes.GATEWAY, $"Task/{Id}");

                        if (response.StatusCode == System.Net.HttpStatusCode.OK)
                        {
                            taskHead = response.ContentAsType<TaskHead>();
                        }
                    }
                    else
                    {
                        if (!string.IsNullOrWhiteSpace(TaskGroupId))
                        {
                            var response = await _apiHelper.MakeAPICallAsync(accessToken, HttpMethod.Get, APITypes.GATEWAY, $"TaskGroup/{TaskGroupId}");

                            if (response.StatusCode == System.Net.HttpStatusCode.OK)
                            {
                                var taskGroupHead = response.ContentAsType<TaskGroupHead>();
                                taskHead.TaskGroupHeaderId = taskGroupHead.Id;
                                taskHead.StartDate = taskGroupHead.StartDate;
                                taskHead.CompletionDate = taskGroupHead.CompletionDate;
                                taskHead.MainUserId = taskGroupHead.MainUserId;
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

            SaveMessageModel = await _apiHelper.GenerateSaveMessageModel(accessToken);

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var accessToken = await HttpContext.GetTokenAsync("access_token");

            await PageConstructor(SaveStates.IGNORE, accessToken);

            if (ModelState.IsValid)
            {
                if (taskHead != null)
                {
                    if (await CreateOrUpdateTaskAsync(accessToken))
                    {
                        SaveState = SaveStates.SUCCESS;
                        SaveMessageModel = await _apiHelper.GenerateSaveMessageModel(accessToken, SaveState);
                        return Page();
                    }
                }
            }

            Id = taskHead.Id.ToString();
            TaskGroupId = taskHead.TaskGroupHeaderId.ToString();

            SaveState = SaveStates.FAILED;

            SaveMessageModel = await _apiHelper.GenerateSaveMessageModel(accessToken, SaveState);

            return Page();
        }

        public async Task<bool> PageConstructor(string saveState, string accessToken)
        {
            SaveState = saveState;

            await CreateUserSelectList(accessToken);

            langQueryList = await _apiHelper.MakeLanguageQuery(accessToken, "EN", new long[] { 10121, 10111, 10500, 10501, 10900, 10901, 10200, 10222, 10203, 10902, 10903, 10904, 10905, 10201, 10906, 10907, 10908, 10909, 10910, 10911, 10918, 10917 });

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

        private async Task<bool> CreateOrUpdateTaskAsync(string apiAccessToken)
        {
            if (taskHead != null)
            {
                if (taskHead.Id != Guid.Empty)
                {
                    var response = await _apiHelper.MakeAPICallAsync(apiAccessToken, HttpMethod.Get, APITypes.GATEWAY, $"Task/{taskHead.Id}");

                    if (response.StatusCode == System.Net.HttpStatusCode.OK)
                    {
                        var foundTaskHead = response.ContentAsType<TaskHead>();

                        if (foundTaskHead != null)
                        {
                            if (!await _modelHelper.ProcessPatch(foundTaskHead, taskHead, foundTaskHead.Id, apiAccessToken, APITypes.GATEWAY, "Task"))
                            {
                                return false;
                            }

                            response = await _apiHelper.MakeAPICallAsync(apiAccessToken, HttpMethod.Get, APITypes.GATEWAY, $"Task/Detail/{taskHead.TaskDetail.Id}");

                            if (response.StatusCode == System.Net.HttpStatusCode.OK)
                            {
                                var foundTaskDet = response.ContentAsType<TaskDet>();

                                if (foundTaskDet != null)
                                {
                                    if (!await _modelHelper.ProcessPatch(foundTaskDet, taskHead.TaskDetail, foundTaskDet.Id, apiAccessToken, APITypes.GATEWAY, "Task/Detail"))
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
                    HttpContent contentPost = new StringContent(JsonConvert.SerializeObject(taskHead), Encoding.UTF8, "application/json");

                    try
                    {
                        var response = await _apiHelper.MakeAPICallAsync(apiAccessToken, HttpMethod.Post, APITypes.GATEWAY, $"Task", contentPost);

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