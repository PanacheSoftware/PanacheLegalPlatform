using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using PanacheSoftware.Core.Domain.API.Folder;
using PanacheSoftware.Core.Types;
using PanacheSoftware.Http;
using PanacheSoftware.UI.Core.Helpers;

namespace PanacheSoftware.UI.Client.Pages.TaskGroup.Task
{
    public class IndexModel : PageModel
    {
        private readonly IAPIHelper _apiHelper;
        private readonly IRazorPartialToStringRenderer _renderer;
        private readonly IModelHelper _modelHelper;
        private readonly IMapper _mapper;
        [BindProperty]
        public FolderNod folderNod { get; set; }
        [BindProperty(SupportsGet = true)]
        public string Id { get; set; }
        [BindProperty(SupportsGet = true)]
        public string ParentId { get; set; }
        public SelectList StatusList { get; }
        public SelectList FolderSelectList { get; set; }
        public SelectList NodeTypeList { get; }

        public IndexModel(IAPIHelper apiHelper, IRazorPartialToStringRenderer renderer, IModelHelper modelHelper, IMapper mapper)
        {
            _apiHelper = apiHelper;
            _renderer = renderer;
            _modelHelper = modelHelper;
            _mapper = mapper;

            StatusTypes statusTypes = new StatusTypes();
            StatusList = new SelectList(statusTypes.GetStatusTypesDictionary(), "Key", "Value");
            NodeTypes nodeTypes = new NodeTypes();
            NodeTypeList = new SelectList(nodeTypes.GetNodeTypesDictionary(), "Key", "Value");
        }

        public async Task<IActionResult> OnGetAsync()
        {
            folderNod = new FolderNod();

            var accessToken = await HttpContext.GetTokenAsync("access_token");

            Guid.TryParse(Id, out Guid foundNodeId);

            if (foundNodeId != Guid.Empty)
            {
                var response = await _apiHelper.MakeAPICallAsync(accessToken, HttpMethod.Get, APITypes.FOLDER, $"FolderNode/{Id}");

                if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    return RedirectToPage("/Logout");
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    folderNod = response.ContentAsType<FolderNod>();
                }
            }
            else
            {
                if (Guid.TryParse(ParentId, out Guid foundFolderId))
                {
                    folderNod.FolderHeaderId = foundFolderId;
                }
            }

            //if (!await CreateFolderSelectList(accessToken))
            //{
            //    return RedirectToPage("/Logout");
            //}

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var accessToken = await HttpContext.GetTokenAsync("access_token");

            if (ModelState.IsValid)
            {
                if (folderNod != null)
                {
                    await CreateOrUpdateFolderNodeAsync(accessToken);
                }

                //return RedirectToPage("Folder", new { Id = folderNod.FolderHeaderId });
                return RedirectToPage("/Folder/Folders");
            }

            Id = folderNod.Id.ToString();

            if (!await CreateFolderSelectList(accessToken))
            {
                return RedirectToPage("/Logout");
            }

            return Page();
        }

        private async Task<bool> CreateFolderSelectList(string accessToken)
        {
            var response = await _apiHelper.MakeAPICallAsync(accessToken, HttpMethod.Get, APITypes.FOLDER, $"Folder/GetFolderStructure/{folderNod.FolderHeaderId}");

            Dictionary<string, string> FolderListDictionary = new Dictionary<string, string>();
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

            FolderSelectList = new SelectList(FolderListDictionary, "Key", "Value");

            return true;
        }

        private async Task<bool> CreateOrUpdateFolderNodeAsync(string apiAccessToken)
        {
            if (folderNod != null)
            {
                if (folderNod.Id != Guid.Empty)
                {
                    var response = await _apiHelper.MakeAPICallAsync(apiAccessToken, HttpMethod.Get, APITypes.FOLDER, $"FolderNode/{folderNod.Id}");

                    if (response.StatusCode == System.Net.HttpStatusCode.OK)
                    {
                        var foundFolderNod = response.ContentAsType<FolderNod>();

                        if (foundFolderNod != null)
                        {
                            if (!await _modelHelper.ProcessPatch(foundFolderNod, folderNod, foundFolderNod.Id, apiAccessToken, APITypes.FOLDER, "FolderNode"))
                            {
                                return false;
                            }

                            response = await _apiHelper.MakeAPICallAsync(apiAccessToken, HttpMethod.Get, APITypes.FOLDER, $"FolderNodeDetail/{foundFolderNod.FolderNodeDetail.Id}");

                            if (response.StatusCode == System.Net.HttpStatusCode.OK)
                            {
                                var foundFolderNodeDet = response.ContentAsType<FolderNodDet>();

                                if (foundFolderNodeDet != null)
                                {
                                    if (!await _modelHelper.ProcessPatch(foundFolderNodeDet, folderNod.FolderNodeDetail, foundFolderNodeDet.Id, apiAccessToken, APITypes.FOLDER, "FolderNodeDetail"))
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
                    HttpContent contentPost = new StringContent(JsonSerializer.Serialize(folderNod), Encoding.UTF8, "application/json");

                    try
                    {
                        var response = await _apiHelper.MakeAPICallAsync(apiAccessToken, HttpMethod.Post, APITypes.FOLDER, $"FolderNode", contentPost);

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