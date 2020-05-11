using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;
using PanacheSoftware.Core.Domain.API.File;
using PanacheSoftware.Core.Domain.API.Language;
using PanacheSoftware.Core.Domain.API.Settings;
using PanacheSoftware.Core.Domain.UI;
using PanacheSoftware.Core.Types;
using PanacheSoftware.Http;
using PanacheSoftware.UI.Core.Helpers;
using PanacheSoftware.UI.Core.PageModel;

namespace PanacheSoftware.UI.Client.Pages.File.Version
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
        public string fileHeaderId { get; set; }
        [BindProperty]
        public FileVersionUploadModel FileVersionUploadModel { get; set; }
        public LangQueryList langQueryList { get; set; }
        public string SaveState { get; set; }
        public string ErrorString { get; set; }
        public SaveMessageModel SaveMessageModel { get; set; }
        public UsrSetting languageCodeSetting { get; set; }

        public IndexModel(IAPIHelper apiHelper, IRazorPartialToStringRenderer renderer, IModelHelper modelHelper, IMapper mapper)
        {
            _apiHelper = apiHelper;
            _renderer = renderer;
            _modelHelper = modelHelper;
            _mapper = mapper;
        }

        public async Task<IActionResult> OnGetAsync()
        {
            var accessToken = await HttpContext.GetTokenAsync("access_token");

            await PageConstructor(SaveStates.IGNORE, accessToken);

            FileVersionUploadModel = new FileVersionUploadModel();
            FileHead fileHead = new FileHead();

            if (!await _apiHelper.AuthCheck(accessToken, User.FindFirst("sub").Value))
            {
                return RedirectToPage("/Logout");
            }

            if (!string.IsNullOrWhiteSpace(fileHeaderId))
            {
                if (Guid.TryParse(fileHeaderId, out Guid parsedId))
                {
                    if (parsedId != Guid.Empty)
                    {
                        var response = await _apiHelper.MakeAPICallAsync(accessToken, HttpMethod.Get, APITypes.FILE, $"File/{parsedId}");

                        if (response.StatusCode == HttpStatusCode.OK)
                        {
                            fileHead = response.ContentAsType<FileHead>();
                        }
                    }
                }
            }

            if (fileHead != null)
            {
                if (!string.IsNullOrWhiteSpace(Id))
                {
                    if (Guid.TryParse(Id, out Guid parsedId))
                    {
                        if (parsedId != Guid.Empty)
                        {
                            FileVersionUploadModel.FileVersion = fileHead.FileVersions.Where(v => v.Id == parsedId).FirstOrDefault();
                        }
                    }
                }

                FileVersionUploadModel.FileTitle = fileHead.FileDetail.FileTitle;
                FileVersionUploadModel.Description = fileHead.FileDetail.FileTitle;

                if (FileVersionUploadModel.FileVersion == null)
                {
                    FileVersionUploadModel.FileVersion = new FileVer()
                    {
                        FileHeaderId = fileHead.Id
                    };
                }
                else
                {
                    FileVersionUploadModel.FileVersion.FileHeaderId = fileHead.Id;
                }
            }

            SaveMessageModel = await _apiHelper.GenerateSaveMessageModel(accessToken, SaveState);

            return Page();
        }
        public async Task<IActionResult> OnPostAsync()
        {
            var accessToken = await HttpContext.GetTokenAsync("access_token");

            await PageConstructor(SaveStates.IGNORE, accessToken);

            if (ModelState.IsValid)
            {
                if (await UploadFile(accessToken))
                {
                    SaveState = SaveStates.SUCCESS;

                    SaveMessageModel = await _apiHelper.GenerateSaveMessageModel(accessToken, SaveState);

                    return Page();
                }
            }

            SaveState = SaveStates.FAILED;

            SaveMessageModel = await _apiHelper.GenerateSaveMessageModel(accessToken, SaveState);

            return Page();
        }


        public async Task<bool> PageConstructor(string saveState, string accessToken)
        {
            SaveState = saveState;

            languageCodeSetting = await _apiHelper.GetUserLanguage(accessToken);

            langQueryList = await _apiHelper.MakeLanguageQuery(accessToken, languageCodeSetting.Value, new long[] { 10121 });

            return true;
        }

        private async Task<bool> UploadFile(string accessToken)
        {
            if (FileVersionUploadModel.FileVersion != null)
            {
                if (FileVersionUploadModel.FileVersion.Id == Guid.Empty)
                {
                    if (string.IsNullOrWhiteSpace(FileVersionUploadModel.URI))
                    {
                        var fileSizeSetting = await _apiHelper.GetSystemSetting(accessToken, "FILE_UPLOAD_MAX");
                        var fileExtPermitted = await _apiHelper.GetSystemSetting(accessToken, "FILE_UPLOAD_EXT");

                        var formFileContent =
                            await FileHelpers.ProcessFormFile<FileUploadModel>(
                                FileVersionUploadModel.FormFile, ModelState, GeneralHelpers.SeperatedListToEnumerable(fileExtPermitted.Value, prefix: ".").ToArray<string>(),
                                long.Parse(fileSizeSetting.Value));

                        if (!ModelState.IsValid)
                            return false;

                        FileVersionUploadModel.FileVersion.Size = FileVersionUploadModel.FormFile.Length;
                        FileVersionUploadModel.FileVersion.UntrustedName = FileVersionUploadModel.FormFile.FileName;
                        FileVersionUploadModel.FileVersion.TrustedName = WebUtility.HtmlEncode(FileVersionUploadModel.FormFile.FileName);
                        FileVersionUploadModel.FileVersion.Content = formFileContent;
                    }
                    else
                    {
                        FileVersionUploadModel.FileVersion.URI = FileVersionUploadModel.URI;
                    }
                }
            }

            return await CreateFileVersionAsync(accessToken);
        }

        private async Task<bool> CreateFileVersionAsync(string apiAccessToken)
        {
            if (FileVersionUploadModel.FileVersion != null)
            {
                if (FileVersionUploadModel.FileVersion.Id == Guid.Empty)
                {
                    HttpContent contentPost = new StringContent(JsonConvert.SerializeObject(FileVersionUploadModel.FileVersion), Encoding.UTF8, "application/json");

                    try
                    {
                        var response = await _apiHelper.MakeAPICallAsync(apiAccessToken, HttpMethod.Post, APITypes.FILE, $"FileVersion", contentPost);

                        if (response.StatusCode != HttpStatusCode.Created)
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