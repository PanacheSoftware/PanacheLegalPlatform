using IdentityModel;
using IdentityServer4;
using IdentityServer4.Models;
using PanacheSoftware.Core.Types;
using System.Collections.Generic;
using static IdentityServer4.IdentityServerConstants;

namespace PanacheSoftware.Identity
{
    public static class Config
    {
        public static IEnumerable<IdentityResource> GetIdentityResources()
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

        public static IEnumerable<ApiResource> GetApis()
        {
            return new ApiResource[]
            {
                new ApiResource(LocalApi.ScopeName),
                new ApiResource(PanacheSoftwareScopeNames.ClientService){ UserClaims = { "tenantid" }, ApiSecrets = { new Secret("1314EF18-40FA-4B16-83DF-B276FF0D92A9".Sha256()) } },
                new ApiResource(PanacheSoftwareScopeNames.TeamService){ UserClaims = { "tenantid" }, ApiSecrets = { new Secret("5C9BF545-3C20-4448-9EEC-6B3E745B671E".Sha256()) } },
                new ApiResource(PanacheSoftwareScopeNames.FoundationService){ UserClaims = { "tenantid" }, ApiSecrets = { new Secret("70CD8BB9-5256-42CF-8B95-DD61C1051AD0".Sha256()) } },
                new ApiResource(PanacheSoftwareScopeNames.TaskService){ UserClaims = { "tenantid" }, ApiSecrets = { new Secret("AC654B02-E46B-4359-B908-87479CBE1CEB".Sha256()) } },
                new ApiResource(PanacheSoftwareScopeNames.FileService){ UserClaims = { "tenantid" }, ApiSecrets = { new Secret("839C649E-4FE3-410C-B43F-69C017A52676".Sha256()) } }
            };
        }

        public static IEnumerable<Client> GetClients()
        {
            return new[]
            {
                // client credentials flow client
                //new Client
                //{
                //    ClientId = "client",
                //    ClientName = "Client Credentials Client",

                //    AllowedGrantTypes = GrantTypes.ClientCredentials,
                //    ClientSecrets = { new Secret("511536EF-F270-4058-80CA-1C89C192F69A".Sha256()) },

                //    AllowedScopes = { LocalApi.ScopeName }
                //},

                // MVC client using hybrid flow
                //new Client
                //{
                //    ClientId = "mvc",
                //    ClientName = "MVC Client",

                //    //Don't show consent page
                //    RequireConsent = false,

                //    AllowedGrantTypes = GrantTypes.HybridAndClientCredentials,
                //    ClientSecrets = { new Secret("49C1A7E1-0C79-4A89-A3D6-A37998FB86B0".Sha256()) },

                //    RedirectUris = { "http://localhost:5002/signin-oidc" },
                //    FrontChannelLogoutUri = "http://localhost:5002/signout-oidc",
                //    PostLogoutRedirectUris = { "http://localhost:5002/signout-callback-oidc" },

                //    AllowOfflineAccess = true,
                //    AllowedScopes = { StandardScopes.OpenId, StandardScopes.Profile, LocalApi.ScopeName, StandardScopes.Email }
                //},

                //new Client
                //{
                //    ClientId = PanacheSoftwareScopeNames.WebClient,
                //    ClientName = "Panache Software Web client",

                //    //Don't show consent page
                //    RequireConsent = false,
                    
                //    AllowedGrantTypes = GrantTypes.HybridAndClientCredentials,
                //    ClientSecrets = { new Secret("49C1A7E1-0C79-4A89-A3D6-A37998FB86B0".Sha256()) },

                //    RedirectUris = { "https://localhost:44380/signin-oidc" },
                //    FrontChannelLogoutUri = "https://localhost:44380/signout-oidc",
                //    PostLogoutRedirectUris = { "https://localhost:44380/signout-callback-oidc" },

                //    AllowOfflineAccess = true,
                //    AllowedScopes = {
                //        StandardScopes.OpenId,
                //        StandardScopes.Profile,
                //        LocalApi.ScopeName,
                //        StandardScopes.Email,
                //        PanacheSoftwareScopeNames.ClientService,
                //        PanacheSoftwareScopeNames.TeamService,
                //        PanacheSoftwareScopeNames.FolderService
                //    }
                //},

                new Client
                {
                    ClientId = PanacheSoftwareScopeNames.ClientUI,
                    ClientName = "Panache Software UI Client",

                    //Don't show consent page
                    RequireConsent = false,

                    AllowedGrantTypes = GrantTypes.HybridAndClientCredentials,
                    ClientSecrets = { new Secret("49C1A7E1-0C79-4A89-A3D6-A37998FB86B0".Sha256()) },

                    RedirectUris = { "https://localhost:44380/signin-oidc" },
                    FrontChannelLogoutUri = "https://localhost:44380/signout-oidc",
                    PostLogoutRedirectUris = { "https://localhost:44380/signout-callback-oidc" },

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
                        PanacheSoftwareScopeNames.FileService
                    }
                },

                // SPA client using implicit flow
                //new Client
                //{
                //    ClientId = "spa",
                //    ClientName = "SPA Client",
                //    ClientUri = "http://identityserver.io",

                //    AllowedGrantTypes = GrantTypes.Implicit,
                //    AllowAccessTokensViaBrowser = true,

                //    RedirectUris =
                //    {
                //        "http://localhost:5002/index.html",
                //        "http://localhost:5002/callback.html",
                //        "http://localhost:5002/silent.html",
                //        "http://localhost:5002/popup.html",
                //    },

                //    PostLogoutRedirectUris = { "http://localhost:5002/index.html" },
                //    AllowedCorsOrigins = { "http://localhost:5002" },

                //    AllowedScopes = {  StandardScopes.OpenId, StandardScopes.Profile, LocalApi.ScopeName }
                //}
            };
        }
    }
}
