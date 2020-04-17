using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;
using AutoMapper;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
//using Newtonsoft.Json;
using PanacheSoftware.Core.Domain.API.Client;
using PanacheSoftware.Core.Domain.API.Folder;
using PanacheSoftware.Core.Domain.API.Team;
using PanacheSoftware.Core.Domain.Identity.API;
using PanacheSoftware.Core.Domain.UI;
using PanacheSoftware.Core.Types;
using PanacheSoftware.Http;
using PanacheSoftware.UI.Core.Helpers;
using PanacheSoftware.Core.Extensions;
using PanacheSoftware.UI.Core.PageModel;
using PanacheSoftware.Core.Domain.API.Language;

namespace PanacheSoftware.UI.Client.Pages.Folder
{
    public class IndexModel : PageModel, IPanacheSoftwarePageModel
    {
        private readonly IAPIHelper _apiHelper;
        private readonly IRazorPartialToStringRenderer _renderer;
        private readonly IModelHelper _modelHelper;
        private readonly IMapper _mapper;
        [BindProperty]
        public FolderHead folderHead { get; set; }
        [BindProperty(SupportsGet = true)]
        public string Id { get; set; }
        public SelectList StatusList { get; }
        public SelectList TeamSelectList { get; set; }
        public SelectList UserSelectList { get; set; }
        public SelectList ClientSelectList { get; set; }
        public SelectList FolderSelectList { get; set; }
        public string FolderDatasource { get; set; }
        public List<FolderStructureModel> completedNodes { get; set; }
        public List<FolderStructureModel> incompleteNodes { get; set; }
        public LangQueryList langQueryList { get; set; }
        public string SaveState { get; set; }
        public string ErrorString { get; set; }

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

            langQueryList = await _apiHelper.MakeLanguageQuery(accessToken, "EN", new long[] { 10121, 10111 });

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
    }
}