using IdentityModel;
using IdentityServer4;
using IdentityServer4.Models;
using PanacheSoftware.Core.Types;
using System.Collections.Generic;
using PanacheSoftware.Core.Domain.Configuration;
using PanacheSoftware.Http;
using static IdentityServer4.IdentityServerConstants;

namespace PanacheSoftware.Identity
{
    public class Config
    {
        private readonly PanacheSoftwareConfiguration _panacheSoftwareConfiguration;

        public Config(PanacheSoftwareConfiguration panacheSoftwareConfiguration)
        {
            _panacheSoftwareConfiguration = panacheSoftwareConfiguration;
        }

        public IEnumerable<IdentityResource> GetIdentityResources()
        {
            var customProfile = new IdentityResource(
                name: PanacheSoftwareScopeNames.IdentityResourceProfile,
                displayName: "PanacheSoftware Profile",
                userClaims: new[] { "tenantid" });

            return new IdentityResource[]
            {
                new IdentityResources.OpenId(),
                new IdentityResources.Profile(),
                new IdentityResources.Email(),
                customProfile
            };
        }

        public IEnumerable<ApiResource> GetApis()
        {
            return new ApiResource[]
            {
                new ApiResource(LocalApi.ScopeName),
                new ApiResource(PanacheSoftwareScopeNames.ClientService){ UserClaims = { "tenantid" }, ApiSecrets = { new Secret(_panacheSoftwareConfiguration.Secret.ClientServiceSecret.Sha256()) }, Scopes = {PanacheSoftwareScopeNames.ClientService}},
                new ApiResource(PanacheSoftwareScopeNames.TeamService){ UserClaims = { "tenantid" }, ApiSecrets = { new Secret(_panacheSoftwareConfiguration.Secret.TeamServiceSecret.Sha256()) }, Scopes = {PanacheSoftwareScopeNames.TeamService} },
                new ApiResource(PanacheSoftwareScopeNames.FoundationService){ UserClaims = { "tenantid" }, ApiSecrets = { new Secret(_panacheSoftwareConfiguration.Secret.FoundationServiceSecret.Sha256()) }, Scopes = {PanacheSoftwareScopeNames.FoundationService} },
                new ApiResource(PanacheSoftwareScopeNames.TaskService){ UserClaims = { "tenantid" }, ApiSecrets = { new Secret(_panacheSoftwareConfiguration.Secret.TaskServiceSecret.Sha256()) }, Scopes = {PanacheSoftwareScopeNames.TaskService} },
                new ApiResource(PanacheSoftwareScopeNames.FileService){ UserClaims = { "tenantid" }, ApiSecrets = { new Secret(_panacheSoftwareConfiguration.Secret.FileServiceSecret.Sha256()) }, Scopes = {PanacheSoftwareScopeNames.FileService} },
                new ApiResource(PanacheSoftwareScopeNames.APIGateway){ UserClaims = {"tenantid"}, ApiSecrets = { new Secret(_panacheSoftwareConfiguration.Secret.APIGatewaySecret.Sha256()) }, Scopes = {PanacheSoftwareScopeNames.APIGateway}},
                new ApiResource(PanacheSoftwareScopeNames.CustomFieldService){ UserClaims = {"tenantid"}, ApiSecrets = { new Secret(_panacheSoftwareConfiguration.Secret.CustomFieldServiceSecret.Sha256()) }, Scopes = {PanacheSoftwareScopeNames.CustomFieldService}}
            };
        }

        public IEnumerable<ApiScope> GetApiScopes()
        {
            return new List<ApiScope>
            {
                new ApiScope(name: LocalApi.ScopeName, displayName: LocalApi.ScopeName),
                new ApiScope(name: PanacheSoftwareScopeNames.ClientService, displayName:PanacheSoftwareScopeNames.ClientService),
                new ApiScope(name: PanacheSoftwareScopeNames.TeamService, displayName:PanacheSoftwareScopeNames.TeamService),
                new ApiScope(name: PanacheSoftwareScopeNames.FoundationService, displayName:PanacheSoftwareScopeNames.FoundationService),
                new ApiScope(name: PanacheSoftwareScopeNames.TaskService, displayName:PanacheSoftwareScopeNames.TaskService),
                new ApiScope(name: PanacheSoftwareScopeNames.FileService, displayName:PanacheSoftwareScopeNames.FileService),
                new ApiScope(name: PanacheSoftwareScopeNames.APIGateway, displayName:PanacheSoftwareScopeNames.APIGateway),
                new ApiScope(name: PanacheSoftwareScopeNames.CustomFieldService, displayName:PanacheSoftwareScopeNames.CustomFieldService)
            };
        }

        public IEnumerable<Client> GetClients()
        {
            var UIClientURL = bool.Parse(_panacheSoftwareConfiguration.CallMethod.UICallsSecure)
                ? _panacheSoftwareConfiguration.Url.UIClientURLSecure
                : _panacheSoftwareConfiguration.Url.UIClientURL;

            return new[]
            {
                new Client
                {
                    ClientId = PanacheSoftwareScopeNames.ClientUI,
                    ClientName = "Panache Software UI Client",

                    //Don't show consent page
                    RequireConsent = false,

                    AllowedGrantTypes = GrantTypes.Code,
                    ClientSecrets = { new Secret(_panacheSoftwareConfiguration.Secret.UIClientSecret.Sha256()) },

                    RedirectUris = { $"{UIClientURL}/signin-oidc" },
                    FrontChannelLogoutUri = $"{UIClientURL}/signout-oidc",
                    PostLogoutRedirectUris = { $"{UIClientURL}/signout-callback-oidc" },

                    AlwaysIncludeUserClaimsInIdToken = true,

                    AccessTokenType = AccessTokenType.Reference,

                    AllowOfflineAccess = true,
                    AllowedScopes = {
                        StandardScopes.OpenId,
                        StandardScopes.Profile,
                        LocalApi.ScopeName,
                        StandardScopes.Email,
                        StandardScopes.OfflineAccess,
                        PanacheSoftwareScopeNames.IdentityResourceProfile,
                        PanacheSoftwareScopeNames.ClientService,
                        PanacheSoftwareScopeNames.TeamService,
                        PanacheSoftwareScopeNames.FoundationService,
                        PanacheSoftwareScopeNames.TaskService,
                        PanacheSoftwareScopeNames.FileService,
                        PanacheSoftwareScopeNames.APIGateway,
                        PanacheSoftwareScopeNames.CustomFieldService
                    }
                },
            };
        }
    }
}
