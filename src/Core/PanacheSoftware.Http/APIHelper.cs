using Newtonsoft.Json;
using PanacheSoftware.Core.Domain.API.Language;
using PanacheSoftware.Core.Domain.API.Settings;
using PanacheSoftware.Core.Domain.API.Team;
using PanacheSoftware.Core.Domain.UI;
using PanacheSoftware.Core.Types;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using PanacheSoftware.Core.Domain.Configuration;
using Microsoft.OpenApi.Readers;
using Microsoft.OpenApi.Models;
using PanacheSoftware.Core.Domain.Ocelot;

namespace PanacheSoftware.Http
{
    public class APIHelper : IAPIHelper
    {
        private readonly IConfiguration _configuration;

        public APIHelper(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task<HttpResponseMessage> MakeAPICallAsync(string accessToken, HttpMethod httpMethod, string apiType, string uriPart)
        {
            return await MakeAPICallAsync(accessToken, httpMethod, apiType, uriPart, null);
        }

        public async Task<HttpResponseMessage> MakeAPICallAsync(string accessToken, HttpMethod httpMethod, string apiType, string uriPart, HttpContent contentPost)
        {
            var addAPIString = string.Empty;

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

            var response = await MakeAPICallAsync(accessToken, HttpMethod.Get, APITypes.FOUNDATION, $"Language/LanguageQuery", contentPost);

            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                langQueryList = response.ContentAsType<LangQueryList>();
            }

            return langQueryList;
        }

        public async Task<UsrSetting> GetUserLanguage(string accessToken)
        {
            var response = await MakeAPICallAsync(accessToken, HttpMethod.Get, APITypes.FOUNDATION, $"Setting/UserSetting/USER_LANGUAGE");

            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                return response.ContentAsType<UsrSetting>();
            }

            return new UsrSetting() { Value = "EN" };
        }

        public async Task<SettingHead> GetSystemSetting(string accessToken, string settingName)
        {
            var response = await MakeAPICallAsync(accessToken, HttpMethod.Get, APITypes.FOUNDATION, $"Setting/{settingName}");

            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                return response.ContentAsType<SettingHead>();
            }

            return null;
        }

        public async Task<List<Guid>> GetTeamsForUserId(string accessToken, string userId)
        {
            var userTeams = new List<Guid>();

            var response = await MakeAPICallAsync(accessToken, HttpMethod.Get, APITypes.TEAM, $"UserTeam/GetTeamsForUser/{userId}");

            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                TeamList teamList = response.ContentAsType<TeamList>();

                foreach (var teamHead in teamList.TeamHeaders)
                {
                    userTeams.Add(teamHead.Id);
                }
            }

            return userTeams;
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

        public string GetBaseURL(string apiType, bool ignoreGateway = false)
        {
            var panacheSoftwareConfiguration = new PanacheSoftwareConfiguration();
            _configuration.Bind("PanacheSoftware", panacheSoftwareConfiguration);

            if (!ignoreGateway)
            {
                //If appsttings specifies we should use the API gateway force all API requests through their
                if (bool.Parse(panacheSoftwareConfiguration.CallMethod.UseAPIGateway))
                    return bool.Parse(panacheSoftwareConfiguration.CallMethod.APICallsSecure)
                        ? panacheSoftwareConfiguration.Url.APIGatewayURLSecure
                        : panacheSoftwareConfiguration.Url.APIGatewayURL;
            }

            //Otherwise return the URL specific to the APIType passed in
            switch (apiType)
            {
                case APITypes.CLIENT:
                    return bool.Parse(panacheSoftwareConfiguration.CallMethod.APICallsSecure)
                        ? panacheSoftwareConfiguration.Url.ClientServiceURLSecure
                        : panacheSoftwareConfiguration.Url.ClientServiceURL;
                case APITypes.FILE:
                    return bool.Parse(panacheSoftwareConfiguration.CallMethod.APICallsSecure)
                        ? panacheSoftwareConfiguration.Url.FileServiceURLSecure
                        : panacheSoftwareConfiguration.Url.FileServiceURL;
                case APITypes.FOUNDATION:
                    return bool.Parse(panacheSoftwareConfiguration.CallMethod.APICallsSecure)
                        ? panacheSoftwareConfiguration.Url.FoundationServiceURLSecure
                        : panacheSoftwareConfiguration.Url.FoundationServiceURL;
                case APITypes.GATEWAY:
                    return bool.Parse(panacheSoftwareConfiguration.CallMethod.APICallsSecure)
                        ? panacheSoftwareConfiguration.Url.APIGatewayURLSecure
                        : panacheSoftwareConfiguration.Url.APIGatewayURL;
                case APITypes.IDENTITY:
                    return bool.Parse(panacheSoftwareConfiguration.CallMethod.APICallsSecure)
                        ? panacheSoftwareConfiguration.Url.IdentityServerURLSecure
                        : panacheSoftwareConfiguration.Url.IdentityServerURL;
                case APITypes.TASK:
                    return bool.Parse(panacheSoftwareConfiguration.CallMethod.APICallsSecure)
                        ? panacheSoftwareConfiguration.Url.TaskServiceURLSecure
                        : panacheSoftwareConfiguration.Url.TaskServiceURL;
                case APITypes.TEAM:
                    return bool.Parse(panacheSoftwareConfiguration.CallMethod.APICallsSecure)
                        ? panacheSoftwareConfiguration.Url.TeamServiceURLSecure
                        : panacheSoftwareConfiguration.Url.TeamServiceURL;
                case APITypes.UICLIENT:
                    return bool.Parse(panacheSoftwareConfiguration.CallMethod.APICallsSecure)
                        ? panacheSoftwareConfiguration.Url.UIClientURLSecure
                        : panacheSoftwareConfiguration.Url.UIClientURL;
                case APITypes.CUSTOMFIELD:
                    return bool.Parse(panacheSoftwareConfiguration.CallMethod.APICallsSecure)
                        ? panacheSoftwareConfiguration.Url.CustomFieldServiceURLSecure
                        : panacheSoftwareConfiguration.Url.CustomFieldServiceURL;
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

        public async Task<bool> CheckGatewayConfig(string accessToken)
        {
            //If no accesstoken we won't be able to query the gateway so just assume it's okay, it
            //will be checked again once there is a valid token.
            if (string.IsNullOrWhiteSpace(accessToken))
                return true;

            var response = await MakeAPICallAsync(accessToken, HttpMethod.Get, APITypes.GATEWAY, $"administration/configuration");

            if(response.IsSuccessStatusCode)
            {
                var gatewayConfig = response.ContentAsType<Root>();

                if(gatewayConfig != null)
                {
                    if(gatewayConfig.routes != null)
                    {
                        if(gatewayConfig.routes.Count > 0)
                        {
                            return true;
                        }
                    }
                }
            }

            return false;
        }

        public async Task<bool> CreateBaseGatewayConfig(string accessToken)
        {
            var apiList = new APIList();

            apiList.APIListDetails.Add(new APIListDetail(GetBaseURL(APITypes.FOUNDATION, true), string.Empty));
            apiList.APIListDetails.Add(new APIListDetail(GetBaseURL(APITypes.TEAM, true), string.Empty));
            apiList.APIListDetails.Add(new APIListDetail(GetBaseURL(APITypes.IDENTITY, true), string.Empty));
            apiList.APIListDetails.Add(new APIListDetail(GetBaseURL(APITypes.TASK, true), string.Empty));
            apiList.APIListDetails.Add(new APIListDetail(GetBaseURL(APITypes.CLIENT, true), string.Empty));
            apiList.APIListDetails.Add(new APIListDetail(GetBaseURL(APITypes.CUSTOMFIELD, true), string.Empty));
            apiList.APIListDetails.Add(new APIListDetail(GetBaseURL(APITypes.FILE, true), string.Empty));

            return await ProcessAPIConfig(accessToken, apiList, GetBaseURL(APITypes.GATEWAY));
        }

        private async Task<OpenApiDocument> GetOpenAPIDocument(Uri APIBaseURI, string URIGetPart)
        {
            var httpClient = new HttpClient
            {
                BaseAddress = APIBaseURI
            };

            var stream = await httpClient.GetStreamAsync(URIGetPart);

            return new OpenApiStreamReader().Read(stream, out var diagnostic);
        }

        public async Task<bool> ProcessAPIConfig(string accessToken, APIList apiList, string GatewayURI)
        {
            if(string.IsNullOrWhiteSpace(GatewayURI))
                return false;

            var OcelotConfiguration = new Root();
            OcelotConfiguration.routes = new List<Route>();

            OcelotConfiguration.globalConfiguration = new GlobalConfiguration()
            {
                baseUrl = GatewayURI
            };

            foreach (var apiDetail in apiList.APIListDetails)
            {
                var routes = await GenerateRoutes(apiDetail);

                if(routes.Count > 0)
                    OcelotConfiguration.routes.AddRange(routes);
            }

            HttpContent contentPost = new StringContent(JsonConvert.SerializeObject(OcelotConfiguration), Encoding.UTF8, "application/json");

            var contentText = contentPost.ReadAsStringAsync();

            var response = await MakeAPICallAsync(accessToken, HttpMethod.Post, APITypes.GATEWAY, $"administration/configuration", contentPost);

            return response.IsSuccessStatusCode;
        }

        private async Task<IList<Route>> GenerateRoutes(APIListDetail apiListDetail)
        {
            var routes = new List<Route>();

            var openAPIDocument = await GetOpenAPIDocument(apiListDetail.APIBaseURI, apiListDetail.APIGetPart);         

            foreach (var path in openAPIDocument.Paths)
            {
                var route = new Route();

                route.downstreamPathTemplate = path.Key;
                route.upstreamPathTemplate = path.Key;
                route.upstreamHttpMethod = new List<string>();

                foreach (var operation in path.Value.Operations)
                {
                    route.upstreamHttpMethod.Add(operation.Key.ToString());
                }

                route.downstreamScheme = apiListDetail.HttpPart;

                route.authenticationOptions = new AuthenticationOptions()
                {
                    authenticationProviderKey = "GatewayKey"
                };

                route.downstreamHostAndPorts = new List<DownstreamHostAndPort>();

                route.downstreamHostAndPorts.Add(new DownstreamHostAndPort()
                {
                    host = apiListDetail.HostPart,
                    port = apiListDetail.PortPart
                });

                route.routeIsCaseSensitive = true;

                routes.Add(route);
            }

            return routes;
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
