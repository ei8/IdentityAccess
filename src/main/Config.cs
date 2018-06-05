﻿using IdentityServer4;
using IdentityServer4.Models;
using IdentityServer4.Test;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace works.ei8.IdentityAccess
{
    public class Config
    {
        public static IEnumerable<ApiResource> GetApiResources()
        {
            return new List<ApiResource>
            {
                new ApiResource("api1", "My API") { ApiSecrets = { new Secret("secret".Sha256()) } }
            };
        }

        public static IEnumerable<Client> GetClients(IConfigurationSection configuration)
        {
            return new List<Client>
            {
                new Client
                {
                    ClientId = "xamarin",
                    ClientName = "eShop Xamarin OpenId Client",
                    AccessTokenType = AccessTokenType.Reference,
                    AllowedGrantTypes = GrantTypes.HybridAndClientCredentials, // TODO: GrantTypes.Hybrid,                    
                    //Used to retrieve the access token on the back channel.
                    ClientSecrets =
                    {
                        new Secret("secret".Sha256())
                    },
                    RedirectUris = { $"{configuration.GetValue<string>("Xamarin")}/cortex/diary/callback" },
                    RequireConsent = false,
                    RequirePkce = true,
                    PostLogoutRedirectUris = { Config.GetLogoutRedirectUri(configuration) },
                    AllowedCorsOrigins = { $"{configuration.GetValue<string>("Xamarin")}" },
                    AllowedScopes = new List<string>
                    {
                        IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServerConstants.StandardScopes.Profile,
                        IdentityServerConstants.StandardScopes.OfflineAccess,
                        "api1"
                        //"orders",
                        //"basket",
                        //"locations",
                        //"marketing"
                    },
                    //Allow requesting refresh tokens for long lived API access
                    AllowOfflineAccess = true,
                    AllowAccessTokensViaBrowser = true,
                    //Access token life time is 7200 seconds (2 hour)
                    AccessTokenLifetime = 172800,
                    //Identity token life time is 7200 seconds (2 hour)
                    IdentityTokenLifetime = 172800
                }
            };
        }

        internal static string GetLogoutRedirectUri(IConfigurationSection configuration)
        {
            // TODO: create system wide logout uri for all clients
            return $"{configuration.GetValue<string>("Xamarin")}{Constants.Paths.Logout}";
        }

        public static List<TestUser> GetUsers()
        {
            return new List<TestUser>
            {
                new TestUser
                {
                    SubjectId = "1",
                    Username = "alice",
                    Password = "password",
                    Claims = new Claim[]
                    {
                        new Claim("name", "Alice"),
                        new Claim("website", "www.alice.com")
                    }
                },
                new TestUser
                {
                    SubjectId = "2",
                    Username = "bob",
                    Password = "password"
                }
            };
        }

        public static IEnumerable<IdentityResource> GetIdentityResources()
        {
            return new List<IdentityResource>
            {
                new IdentityResources.OpenId(),
                new IdentityResources.Profile(),
            };
        }
    }
}
