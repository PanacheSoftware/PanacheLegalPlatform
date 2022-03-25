using PanacheSoftware.Core.Domain.API.Language;
using PanacheSoftware.Core.Domain.API.Settings;
using PanacheSoftware.Core.Domain.Ocelot;
using PanacheSoftware.Core.Domain.UI;
using PanacheSoftware.Http;
using PanacheSoftware.UI.Core.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace PanacheSoftware.UI.Client.Blazor.Data
{
    public class FoundationService
    {
        private readonly TokenProvider tokenProvider;
        private readonly IAPIHelper apiHelper;
        private readonly IModelHelper modelHelper;

        public FoundationService(TokenProvider tokenProvider, IAPIHelper apiHelper, IModelHelper modelHelper)
        {
            this.tokenProvider = tokenProvider;
            this.apiHelper = apiHelper;
            this.modelHelper = modelHelper;
        }

        public async Task<string> GetLanguageCode(string accessToken)
        {
            var languageSetting = await apiHelper.GetUserLanguage(accessToken);

            return languageSetting.Value;
        }

        public async Task<SettingHead> GetSystemSetting(string accessToken, string settingName)
        {
            return await apiHelper.GetSystemSetting(accessToken, settingName);

            //var response = await MakeAPICallAsync(accessToken, HttpMethod.Get, APITypes.FOUNDATION, $"Setting/{settingName}");

            //if (response.StatusCode == System.Net.HttpStatusCode.OK)
            //{
            //    return response.ContentAsType<SettingHead>();
            //}

            //return null;
        }

        public async Task<UsrSetting> GetUsrSetting(string accessToken, string settingName)
        {
            return await apiHelper.GetUserSetting(accessToken, settingName);
        }

        public async Task<LangQueryList> GetLanguageInfo(string accessToken, string languageCode, long[] textCodes)
        {
            //var languageSetting = await apiHelper.GetUserLanguage(accessToken);

            var langQueryList = await apiHelper.MakeLanguageQuery(accessToken, languageCode, textCodes);

            return langQueryList;
        }

        public async Task<bool> PerformAuthCheck(string accessToken, string userId)
        {
            return await apiHelper.AuthCheck(accessToken, userId);
        }

        public async Task<HttpResponseMessage> MakeAPICallAsync(string accessToken, HttpMethod httpMethod, string apiType, string uriPart)
        {
            return await apiHelper.MakeAPICallAsync(accessToken, httpMethod, apiType, uriPart);
        }

        public async Task<HttpResponseMessage> MakeAPICallAsync(string accessToken, HttpMethod httpMethod, string apiType, string uriPart, HttpContent contentPost)
        {
            return await apiHelper.MakeAPICallAsync(accessToken, httpMethod, apiType, uriPart, contentPost);
        }

        public async Task<Tuple<bool, string>> ProcessPatch(object existingObject, object updatedObject, Guid objectId, string accessToken, string apiType, string urlPrefix)
        {
            return await modelHelper.ProcessPatchWithMessage(existingObject, updatedObject, objectId, accessToken, apiType, urlPrefix);
        }

        public string GetErrorFromResponse(HttpResponseMessage httpResponseMessage)
        {
            if (httpResponseMessage.IsSuccessStatusCode)
                return string.Empty;

            if (httpResponseMessage.StatusCode == System.Net.HttpStatusCode.NotFound)
                return "Not Found";

            return httpResponseMessage.ContentAsType<APIErrorResponse>().message;
        }

        public async Task<bool> ProcessAPIConfig(string accessToken, APIList apiList, string gatewayURI)
        {
            return await apiHelper.ProcessAPIConfig(accessToken, apiList, gatewayURI);
        }

        public async Task<bool> CheckBaseGatewayConfig(string accessToken)
        {
            if (!await apiHelper.CheckGatewayConfig(accessToken))
                return await apiHelper.CreateBaseGatewayConfig(accessToken);

            return false;
        }

        public APIList GetAPIList()
        {
            return apiHelper.GetAPIList();
        }
    }
}
