using Newtonsoft.Json;
using PanacheSoftware.Core.Domain.API.Language;
using PanacheSoftware.Core.Domain.API.Settings;
using PanacheSoftware.Core.Domain.UI;
using PanacheSoftware.Core.Types;
using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace PanacheSoftware.Http
{
    public class APIHelper : IAPIHelper
    {
        public async Task<HttpResponseMessage> MakeAPICallAsync(string accessToken, HttpMethod httpMethod, string apiType, string uriPart)
        {
            return await MakeAPICallAsync(accessToken, httpMethod, apiType, uriPart, null);
        }

        public async Task<HttpResponseMessage> MakeAPICallAsync(string accessToken, HttpMethod httpMethod, string apiType, string uriPart, HttpContent contentPost)
        {
            var addAPIString = string.Empty;

            if (apiType != APITypes.GATEWAY)
                addAPIString = "api/";


            var requestUri = $"{GetBaseURL(apiType)}/{addAPIString}{uriPart}";

            var httpRequestBuilder = new HttpRequestBuilder();
            httpRequestBuilder.AddBearerToken(accessToken);
            httpRequestBuilder.AddMethod(httpMethod);
            httpRequestBuilder.AddRequestUri(requestUri);
            httpRequestBuilder.AddTimeout(TimeSpan.FromSeconds(30));
            if (contentPost != null)
            {
                httpRequestBuilder.AddContent(contentPost);
            }

            return await httpRequestBuilder.SendAsync();
        }

        public async Task<LangQueryList> MakeLanguageQuery(string accessToken, string languageCode, long[] TextCodes)
        {
            var langQueryList = GetLangQueryList(languageCode, TextCodes);

            HttpContent contentPost = new StringContent(JsonConvert.SerializeObject(langQueryList), Encoding.UTF8, "application/json");

            var response = await MakeAPICallAsync(accessToken, HttpMethod.Get, APITypes.GATEWAY, $"Language/LanguageQuery", contentPost);

            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                langQueryList = response.ContentAsType<LangQueryList>();
            }

            return langQueryList;
        }

        public async Task<UsrSetting> GetUserLanguage(string accessToken)
        {
            var response = await MakeAPICallAsync(accessToken, HttpMethod.Get, APITypes.GATEWAY, $"Setting/UserSetting/USER_LANGUAGE");

            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                return response.ContentAsType<UsrSetting>();
            }

            return new UsrSetting() { Value = "EN" };
        }

        public async Task<SettingHead> GetSystemSetting(string accessToken, string settingName)
        {
            var response = await MakeAPICallAsync(accessToken, HttpMethod.Get, APITypes.GATEWAY, $"Setting/{settingName}");

            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                return response.ContentAsType<SettingHead>();
            }

            return null;
        }

        public async Task<SaveMessageModel> GenerateSaveMessageModel(string accessToken, string saveState = default(string), string errorString = default(string), int historyLength = -2)
        {
            var usrSetting = await GetUserLanguage(accessToken);

            return new SaveMessageModel()
            {
                ErrorString = string.IsNullOrWhiteSpace(errorString) ? string.Empty : errorString,
                SaveState = string.IsNullOrWhiteSpace(saveState) ? SaveStates.IGNORE : saveState,
                HistoryLength = historyLength,
                SaveLangQueryList = await MakeLanguageQuery(accessToken, usrSetting.Value, new long[] { 10500, 10501 })
            };
        }

        private LangQueryList GetLangQueryList(string languageCode, long[] TextCodes)
        {
            var langQueryList = new LangQueryList();

            foreach (var textCode in TextCodes)
            {
                var langQuery = new LangQuery()
                {
                    LanguageCode = languageCode,
                    TextCode = textCode
                };
                langQueryList.LangQuerys.Add(langQuery);
            }

            return langQueryList;
        }

        private string GetBaseURL(string apiType)
        {
            switch (apiType)
            {
                case APITypes.IDENTITY:
                    return "https://localhost:44397";
                case APITypes.CLIENT:
                    return "https://localhost:44359";
                case APITypes.TEAM:
                    return "https://localhost:44357";
                case APITypes.FOLDER:
                    return "https://localhost:44337";
                case APITypes.FOUNDATION:
                    return "https://localhost:44316";
                case APITypes.TASK:
                    return "https://localhost:44377";
                case APITypes.FILE:
                    return "https://localhost:44324";
                case APITypes.GATEWAY:
                    return "https://localhost:44346";
                default:
                    break;
            }
            return string.Empty;
        }

        public async Task<bool> AuthCheck(string accessToken, string userId)
        {
            var response = await MakeAPICallAsync(accessToken, HttpMethod.Get, APITypes.IDENTITY, $"User/{userId}");

            if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            {
                return false;
            }

            return true;
        }

        //public string GetScope(string apiType)
        //{
        //    switch (apiType)
        //    {
        //        case APITypes.USER:
        //            return AzureAdOptions.Settings.PanacheLegalUserScope;
        //        case APITypes.CLIENT:
        //            return AzureAdOptions.Settings.PanacheLegalClientScope;
        //        default:
        //            break;
        //    }

        //    return string.Empty;
        //}
    }
}
