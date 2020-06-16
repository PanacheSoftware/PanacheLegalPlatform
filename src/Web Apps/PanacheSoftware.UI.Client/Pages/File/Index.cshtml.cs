using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch.Internal;
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

namespace PanacheSoftware.UI.Client.Pages.File
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
        public string linkId { get; set; }
        [BindProperty(SupportsGet = true)]
        public string linkType { get; set; }

        [BindProperty]
        public FileUploadModel FileUploadModel { get; set; }

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

            FileUploadModel = new FileUploadModel();

            if (!await _apiHelper.AuthCheck(accessToken, User.FindFirst("sub").Value))
            {
                return RedirectToPage("/Logout");
            }

            if (!string.IsNullOrWhiteSpace(Id))
            {
                if(Guid.TryParse(Id, out Guid parsedId))
                {
                    if(parsedId != Guid.Empty)
                    {
                        var response = await _apiHelper.MakeAPICallAsync(accessToken, HttpMethod.Get, APITypes.FILE, $"File/{parsedId}");

                        if (response.StatusCode == System.Net.HttpStatusCode.OK)
                        {
                            FileUploadModel.FileHeader = response.ContentAsType<FileHead>();
                        }
                    }
                    else
                    {
                        if(!string.IsNullOrWhiteSpace(linkId) && !string.IsNullOrWhiteSpace(linkType))
                        {
                            FileUploadModel.linkId = Guid.Parse(linkId);
                            FileUploadModel.linkType = linkType;
                        }
                    }
                }
            }

            await PageConstructor(SaveStates.IGNORE, accessToken);

            SaveMessageModel = await _apiHelper.GenerateSaveMessageModel(accessToken);

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var accessToken = await HttpContext.GetTokenAsync("access_token");

            await PageConstructor(SaveStates.IGNORE, accessToken);

            if (ModelState.IsValid)
            {
                if(await UploadFile(accessToken))
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
            if (FileUploadModel.FileHeader != null)
            {
                if (FileUploadModel.FileHeader.Id == Guid.Empty)
                {
                    var fileVer = new FileVer()
                    {
                        FileHeaderId = FileUploadModel.FileHeader.Id
                    };

                    if (string.IsNullOrWhiteSpace(FileUploadModel.URI))
                    {
                        var fileSizeSetting = await _apiHelper.GetSystemSetting(accessToken, "FILE_UPLOAD_MAX");
                        var fileExtPermitted = await _apiHelper.GetSystemSetting(accessToken, "FILE_UPLOAD_EXT");

                        var formFileContent =
                            await FileHelpers.ProcessFormFile<FileUploadModel>(
                                FileUploadModel.FormFile, ModelState, GeneralHelpers.SeperatedListToEnumerable(fileExtPermitted.Value, prefix: ".").ToArray<string>(),
                                long.Parse(fileSizeSetting.Value));

                        if (!ModelState.IsValid)
                            return false;

                        FileUploadModel.FileHeader.FileDetail.FileExtension = Path.GetExtension(FileUploadModel.FormFile.FileName).ToLowerInvariant();
                        FileUploadModel.FileHeader.FileDetail.FileType = FileUploadModel.FormFile.ContentType;

                        fileVer.Size = FileUploadModel.FormFile.Length;
                        fileVer.UntrustedName = FileUploadModel.FormFile.FileName;
                        fileVer.TrustedName = WebUtility.HtmlEncode(FileUploadModel.FormFile.FileName);
                        fileVer.Content = formFileContent;
                    }
                    else
                    {
                        fileVer.URI = FileUploadModel.URI;
                        FileUploadModel.FileHeader.FileDetail.FileExtension = "URI";
                        FileUploadModel.FileHeader.FileDetail.FileType = "URI";
                    }

                    FileUploadModel.FileHeader.FileLinks.Add(new FileLnk()
                    {
                        LinkId = FileUploadModel.linkId,
                        LinkType = FileUploadModel.linkType,
                        FileHeaderId = FileUploadModel.FileHeader.Id
                    });

                    FileUploadModel.FileHeader.FileVersions.Add(fileVer);
                }
            }

            return await CreateOrUpdateFileAsync(accessToken);
        }

        private async Task<bool> CreateOrUpdateFileAsync(string apiAccessToken)
        {
            if (FileUploadModel.FileHeader != null)
            {
                if (FileUploadModel.FileHeader.Id != Guid.Empty)
                {
                    var response = await _apiHelper.MakeAPICallAsync(apiAccessToken, HttpMethod.Get, APITypes.FILE, $"File/{FileUploadModel.FileHeader.Id}");

                    if (response.StatusCode == HttpStatusCode.OK)
                    {
                        var foundFileHeader = response.ContentAsType<FileHead>();

                        if (foundFileHeader != null)
                        {
                            if (!await _modelHelper.ProcessPatch(foundFileHeader, FileUploadModel.FileHeader, foundFileHeader.Id, apiAccessToken, APITypes.FILE, "File"))
                            {
                                return false;
                            }

                            if (!await _modelHelper.ProcessPatch(foundFileHeader.FileDetail, FileUploadModel.FileHeader.FileDetail, foundFileHeader.FileDetail.Id, apiAccessToken, APITypes.FILE, "File/Detail"))
                            {
                                return false;
                            }
                        }
                    }
                }
                else
                {
                    HttpContent contentPost = new StringContent(JsonConvert.SerializeObject(FileUploadModel.FileHeader), Encoding.UTF8, "application/json");

                    try
                    {
                        var response = await _apiHelper.MakeAPICallAsync(apiAccessToken, HttpMethod.Post, APITypes.FILE, $"File", contentPost);

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