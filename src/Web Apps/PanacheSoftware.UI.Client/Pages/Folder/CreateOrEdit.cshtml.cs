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
using PanacheSoftware.Core.Domain.API.Folder;
using PanacheSoftware.Core.Domain.API.Language;
using PanacheSoftware.Core.Domain.API.Team;
using PanacheSoftware.Core.Domain.Identity.API;
using PanacheSoftware.Core.Types;
using PanacheSoftware.Http;
using PanacheSoftware.UI.Core.Helpers;
using PanacheSoftware.UI.Core.PageModel;

namespace PanacheSoftware.UI.Client.Pages.Folder
{
    public class CreateOrEditModel : PageModel, IPanacheSoftwarePageModel
    {
        private readonly IAPIHelper _apiHelper;
        private readonly IRazorPartialToStringRenderer _renderer;
        private readonly IModelHelper _modelHelper;
        private readonly IMapper _mapper;
        [BindProperty]
        public FolderHead folderHead { get; set; }
        [BindProperty(SupportsGet = true)]
        public string Id { get; set; }
        [BindProperty(SupportsGet = true)]
        public string ParentId { get; set; }
        public SelectList StatusList { get; }
        public SelectList TeamSelectList { get; set; }
        public SelectList UserSelectList { get; set; }
        public SelectList ClientSelectList { get; set; }
        public SelectList FolderSelectList { get; set; }
        public LangQueryList langQueryList { get; set; }
        public string SaveState { get; set; }
        public string ErrorString { get; set; }

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
            await CreateFolderSelectList(accessToken);
            await CreateUserSelectList(accessToken);
            await CreateClientSelectList(accessToken);

            langQueryList = await _apiHelper.MakeLanguageQuery(accessToken, "EN", new long[] { 10121, 10111, 10500, 10501, 10900, 10901, 10200, 10202, 10203, 10902, 10903, 10904, 10905, 10201, 10906, 10907, 10908, 10909, 10910, 10911 });

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

            folderHead = new FolderHead();
            folderHead.MainUserId = Guid.Parse(User.FindFirst("sub").Value);

            if (!string.IsNullOrWhiteSpace(Id))
            {
                var response = await _apiHelper.MakeAPICallAsync(accessToken, HttpMethod.Get, APITypes.FOLDER, $"Folder/{Id}");

                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    folderHead = response.ContentAsType<FolderHead>();
                }
            }
            else
            {
                Id = Guid.Empty.ToString();
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var accessToken = await HttpContext.GetTokenAsync("access_token");

            await PageConstructor(SaveStates.IGNORE, accessToken);

            if (ModelState.IsValid)
            {
                if (folderHead != null)
                {
                    if(await CreateOrUpdateFolderAsync(accessToken))
                    {
                        SaveState = SaveStates.SUCCESS;
                        return Page();
                    }
                }
            }

            Id = folderHead.Id.ToString();

            SaveState = SaveStates.FAILED;

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

        private async Task<bool> CreateFolderSelectList(string accessToken)
        {
            Dictionary<string, string> FolderListDictionary = new Dictionary<string, string>();

            if (!string.IsNullOrWhiteSpace(ParentId))
            {
                var findFolderResponse = await _apiHelper.MakeAPICallAsync(accessToken, HttpMethod.Get, APITypes.FOLDER, $"Folder/{ParentId}");

                if (findFolderResponse.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    return false;
                }
                else if (findFolderResponse.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    FolderHead folderHead = findFolderResponse.ContentAsType<FolderHead>();

                    FolderListDictionary.Add(folderHead.Id.ToString(), folderHead.ShortName);
                }
            }
            else
            {
                var response = await _apiHelper.MakeAPICallAsync(accessToken, HttpMethod.Get, APITypes.FOLDER, $"Folder/GetValidParents/{Id}");

                FolderListDictionary.Add("", "None");

                if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    return false;
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    FolderList folderList = response.ContentAsType<FolderList>();

                    foreach (var folderHeader in folderList.FolderHeaders.OrderBy(h => h.ShortName))
                    {
                        FolderListDictionary.Add(folderHeader.Id.ToString(), folderHeader.ShortName);
                    }
                }
            }

            FolderSelectList = new SelectList(FolderListDictionary, "Key", "Value");

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

        private async Task<bool> CreateOrUpdateFolderAsync(string apiAccessToken)
        {
            if (folderHead != null)
            {
                if (folderHead.Id != Guid.Empty)
                {
                    var response = await _apiHelper.MakeAPICallAsync(apiAccessToken, HttpMethod.Get, APITypes.FOLDER, $"Folder/{folderHead.Id}");

                    if (response.StatusCode == System.Net.HttpStatusCode.OK)
                    {
                        var foundFolderHead = response.ContentAsType<FolderHead>();

                        if (foundFolderHead != null)
                        {
                            if (!await _modelHelper.ProcessPatch(foundFolderHead, folderHead, foundFolderHead.Id, apiAccessToken, APITypes.FOLDER, "Folder"))
                            {
                                return false;
                            }

                            response = await _apiHelper.MakeAPICallAsync(apiAccessToken, HttpMethod.Get, APITypes.FOLDER, $"FolderDetail/{folderHead.FolderDetail.Id}");

                            if (response.StatusCode == System.Net.HttpStatusCode.OK)
                            {
                                var foundFolderDet = response.ContentAsType<FolderDet>();

                                if (foundFolderDet != null)
                                {
                                    if (!await _modelHelper.ProcessPatch(foundFolderDet, folderHead.FolderDetail, foundFolderDet.Id, apiAccessToken, APITypes.FOLDER, "FolderDetail"))
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
                    HttpContent contentPost = new StringContent(JsonConvert.SerializeObject(folderHead), Encoding.UTF8, "application/json");

                    try
                    {
                        var response = await _apiHelper.MakeAPICallAsync(apiAccessToken, HttpMethod.Post, APITypes.FOLDER, $"Folder", contentPost);

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