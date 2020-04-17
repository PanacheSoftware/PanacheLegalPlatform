using Microsoft.AspNetCore.Http;
using PanacheSoftware.Core.Domain.API.Language;
using PanacheSoftware.Core.Domain.API.Settings;
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
        //string GetScope(string apiType);
    }
}
