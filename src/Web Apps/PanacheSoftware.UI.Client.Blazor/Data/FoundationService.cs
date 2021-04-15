using PanacheSoftware.Core.Domain.API.Language;
using PanacheSoftware.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PanacheSoftware.UI.Client.Blazor.Data
{
    public class FoundationService
    {
        private readonly TokenProvider tokenProvider;
        private readonly IAPIHelper apiHelper;

        public FoundationService(TokenProvider tokenProvider, IAPIHelper apiHelper)
        {
            this.tokenProvider = tokenProvider;
            this.apiHelper = apiHelper;
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
    }
}
