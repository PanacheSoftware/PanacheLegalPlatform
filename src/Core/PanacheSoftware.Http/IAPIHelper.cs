using Microsoft.AspNetCore.Http;
using PanacheSoftware.Core.Domain.API.Language;
using PanacheSoftware.Core.Domain.API.Settings;
using PanacheSoftware.Core.Domain.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace PanacheSoftware.Http
{
    public interface IAPIHelper
    {
        //HttpContent contentPost
        Task<HttpResponseMessage> MakeAPICallAsync(string accessToken, HttpMethod httpMethod, string apiType, string uriPart);
        Task<HttpResponseMessage> MakeAPICallAsync(string accessToken, HttpMethod httpMethod, string apiType, string uriPart, HttpContent contentPost);

        Task<LangQueryList> MakeLanguageQuery(string accessToken, string languageCode, long[] TextCodes);

        Task<bool> AuthCheck(string accessToken, string userId);

        Task<UsrSetting> GetUserLanguage(string accessToken);

        Task<SaveMessageModel> GenerateSaveMessageModel(string accessToken, string saveState = default(string), string errorString = default(string), int historyLength = -2);
        Task<SettingHead> GetSystemSetting(string accessToken, string settingName);
        Task<List<Guid>> GetTeamsForUserId(string accessToken, string userId);
        //string GetScope(string apiType);
    }
}
