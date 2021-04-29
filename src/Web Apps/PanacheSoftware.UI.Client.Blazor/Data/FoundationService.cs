﻿using PanacheSoftware.Core.Domain.API.Language;
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

        public async Task<LangQueryList> GetLanguageInfo(string accessToken, long[] textCodes)
        {
            var languageSetting = await apiHelper.GetUserLanguage(accessToken);

            var langQueryList = await apiHelper.MakeLanguageQuery(accessToken, languageSetting.Value, textCodes);

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

        public async Task<string> GetErrorFromResponse(HttpResponseMessage httpResponseMessage)
        {
            if (httpResponseMessage.IsSuccessStatusCode)
                return string.Empty;

            if (httpResponseMessage.StatusCode == System.Net.HttpStatusCode.NotFound)
                return "Not Found";

            return httpResponseMessage.ContentAsType<APIErrorResponse>().message;
        }
    }
}