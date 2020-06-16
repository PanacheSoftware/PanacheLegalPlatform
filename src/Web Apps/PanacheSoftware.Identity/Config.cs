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
                claimTypes: new[] { "tenantid" });

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
                new ApiResource(PanacheSoftwareScopeNames.ClientService){ UserClaims = { "tenantid" }, ApiSecrets = { new Secret(_panacheSoftwareConfiguration.Secret.ClientServiceSecret.Sha256()) } },
                new ApiResource(PanacheSoftwareScopeNames.TeamService){ UserClaims = { "tenantid" }, ApiSecrets = { new Secret(_panacheSoftwareConfiguration.Secret.TeamServiceSecret.Sha256()) } },
                new ApiResource(PanacheSoftwareScopeNames.FoundationService){ UserClaims = { "tenantid" }, ApiSecrets = { new Secret(_panacheSoftwareConfiguration.Secret.FoundationServiceSecret.Sha256()) } },
                new ApiResource(PanacheSoftwareScopeNames.TaskService){ UserClaims = { "tenantid" }, ApiSecrets = { new Secret(_panacheSoftwareConfiguration.Secret.TaskServiceSecret.Sha256()) } },
                new ApiResource(PanacheSoftwareScopeNames.FileService){ UserClaims = { "tenantid" }, ApiSecrets = { new Secret(_panacheSoftwareConfiguration.Secret.FileServiceSecret.Sha256()) } },
                new ApiResource(PanacheSoftwareScopeNames.APIGateway){ UserClaims = {"tenantid"}, ApiSecrets = { new Secret(_panacheSoftwareConfiguration.Secret.APIGatewaySecret.Sha256()) }}
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

                    AllowedGrantTypes = GrantTypes.HybridAndClientCredentials,
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
                        PanacheSoftwareScopeNames.APIGateway
                    }
                },
            };
        }
    }
}
