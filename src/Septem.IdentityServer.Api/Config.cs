using IdentityServer4.Models;
using System.Collections.Generic;
using IdentityServer4;

namespace Septem.IdentityServer.Api
{

    public static class Config
    {
        public static string BusinessClientUid { get; } = "0db66ad7-4613-f092-41b7-3fae65b4b79d";
        public static string RestaurantClientUid { get; } = "8e83a7af-b2c6-c291-44e0-4e88b588cf54";
        public static string EndUserClientUid { get; } = "7d4f9f3e-13db-e6ad-402b-f40814100693";
        public static string AdminClientUid { get; } = "7b417557-e8cc-f980-4863-fb9e7b58727d";
        public static string DemoClientUid { get; } = "5b27953f-80e0-07b3-4b1a-5b6b5342b2c1";
        public static string TaskManagerClientUid { get; } = "1d0f8a46-a756-8887-42eb-52288fe592ea";
        public static string CyberArenaClientUid { get; } = "a549caaa-53bb-2e84-40f1-de699f8b1813";
        public static string IikoSyncAppClientUid { get; } = "1e753d07-95c8-c28d-448d-935002191f24";
        public static string LicenseAdminClientUid { get; } = "13d64611-f302-5ab4-4255-d2f7f75bf777";


        public static IEnumerable<ApiScope> ApiScopes =>
            new ApiScope[]
            {
                new ApiScope(Shared.Apis.TaskManagerApiName),
                new ApiScope(Shared.Apis.AgendaAdminApiName),
                new ApiScope(Shared.Apis.AgendaBusinessApiName),
                new ApiScope(Shared.Apis.AgendaClientApiName),
                new ApiScope(Shared.Apis.AgendaRestaurantApiName),
                new ApiScope(Shared.Apis.CyberArenaApiName),
                new ApiScope(Shared.Apis.LicenseManagementWebApiName)
            };

        public static IEnumerable<IdentityResource> Ids =>
            new IdentityResource[]
            {
                new IdentityResources.Profile(),
                new IdentityResource(
                    "roles",
                    "Your role(s)",
                    new List<string>() { "role" }),
            };

        public static IEnumerable<ApiResource> Apis =>
            new[]
            {
                new ApiResource(Shared.Apis.AgendaBusinessApiName, "Agenda Business Mobile Api")
                {
                    ApiSecrets = { new Secret("9e0d4c47-8904-12ae-4810-304af38334bc".Sha256()) }
                },
                new ApiResource(Shared.Apis.AgendaClientApiName, "Agenda Client Mobile Api")
                {
                    ApiSecrets = { new Secret("5dcc3ce6-2362-5594-45c2-c5ab9586190d".Sha256()) }
                },
                new ApiResource(Shared.Apis.AgendaAdminApiName, "Agenda Admin Api")
                {
                    ApiSecrets = { new Secret("67f6c959-06ce-4ca0-4206-2c85fddb537a".Sha256()) }
                },
                new ApiResource(Shared.Apis.AgendaRestaurantApiName, "Agenda Restaurant Api")
                {
                    ApiSecrets = { new Secret("f28cd452-1e0c-90ba-483e-b23a9ea12670".Sha256()) }
                },
                new ApiResource(Shared.Apis.TaskManagerApiName, "Task Manager Api")
                {
                    ApiSecrets = { new Secret("e1e8efe6-fc48-b1a8-4ce2-901369cf8a18".Sha256()) }
                },
                new ApiResource(Shared.Apis.CyberArenaApiName, "Cyber Arena Api")
                {
                    ApiSecrets = { new Secret("ff8e2621-071a-5ca3-43af-0c264dd982d3".Sha256()) }
                },
                new ApiResource(Shared.Apis.LicenseManagementWebApiName, "License server Api")
                {
                    ApiSecrets = { new Secret("1acd2356-e460-dd92-4c13-2e6834cf9ffe".Sha256()) },
                    Scopes = new []
                    {
                        Shared.Apis.LicenseManagementWebApiName
                    }
                },
            };

        public static IEnumerable<Client> Clients =>
            new[]
            {
                new Client
                {
                    AccessTokenLifetime = 696969,
                    AllowOfflineAccess = true,
                    AllowedCorsOrigins = new List<string>
                    {
                        "http://app.novbem.az",
                        "http://app-test.novbem.az",

                        "https://app.novbem.az",
                        "https://app-test.novbem.az",

                        "http://reception.novbem.az",
                        "http://reception-test.novbem.az",

                        "https://reception.novbem.az",
                        "https://reception-test.novbem.az",

                        "http://restaurant.novbem.az",
                        "http://restaurant-test.novbem.az",

                        "https://restaurant.novbem.az",
                        "https://restaurant-test.novbem.az",

                        "http://localhost:8080",
                    },
                    UpdateAccessTokenClaimsOnRefresh = true,
                    ClientName = "EndUserClient",
                    ClientId = EndUserClientUid,
                    ClientSecrets = {new Secret("4937e7d0-7524-5a92-4941-e7b6fa22dbb9".Sha256())},
                    AllowedGrantTypes = GrantTypes.ResourceOwnerPasswordAndClientCredentials,
                    AllowedScopes =
                    {
                        IdentityServerConstants.StandardScopes.Profile,
                        Shared.Apis.AgendaClientApiName,
                        Shared.Apis.AgendaRestaurantApiName,
                        "roles",
                    },
                    Properties = new Dictionary<string, string>
                    {
                        {"dbConnection", "AgendaApiConnection"}
                    }
                },
                new Client
                {
                    AccessTokenLifetime = 696969,
                    AllowOfflineAccess = true,
                    AllowedCorsOrigins = new List<string>
                    {
                        "http://app.novbem.az",
                        "http://app-test.novbem.az",

                        "https://app.novbem.az",
                        "https://app-test.novbem.az",

                        "http://reception.novbem.az",
                        "http://reception-test.novbem.az",

                        "https://reception.novbem.az",
                        "https://reception-test.novbem.az",

                        "http://restaurant.novbem.az",
                        "http://restaurant-test.novbem.az",

                        "https://restaurant.novbem.az",
                        "https://restaurant-test.novbem.az",

                        "http://localhost:8080",
                    },
                    UpdateAccessTokenClaimsOnRefresh = true,
                    ClientName = "BusinessClient",
                    ClientId = BusinessClientUid,
                    ClientSecrets = {new Secret("b0ef52bf-e8c0-65b9-4598-c9db068e9a79".Sha256())},
                    AllowedGrantTypes = GrantTypes.ResourceOwnerPasswordAndClientCredentials,
                    AllowedScopes =
                    {
                        IdentityServerConstants.StandardScopes.Profile,
                        Shared.Apis.AgendaBusinessApiName,
                        "roles",
                    },
                    Properties = new Dictionary<string, string>
                    {
                        {"dbConnection", "AgendaApiConnection"}
                    }
                },
                new Client
                {
                    AccessTokenLifetime = 696969,
                    AllowOfflineAccess = true,
                    AllowedCorsOrigins = new List<string>
                    {
                        "http://app.novbem.az",
                        "http://app-test.novbem.az",

                        "https://app.novbem.az",
                        "https://app-test.novbem.az",

                        "http://reception.novbem.az",
                        "http://reception-test.novbem.az",

                        "https://reception.novbem.az",
                        "https://reception-test.novbem.az",

                        "http://restaurant.novbem.az",
                        "http://restaurant-test.novbem.az",

                        "https://restaurant.novbem.az",
                        "https://restaurant-test.novbem.az",

                        "http://localhost:8080",
                    },
                    UpdateAccessTokenClaimsOnRefresh = true,
                    ClientName = "DemoClient",
                    ClientId = DemoClientUid,
                    ClientSecrets = {new Secret("1b77911c-49dc-2588-47be-c4443e941661".Sha256())},
                    AllowedGrantTypes = GrantTypes.ResourceOwnerPasswordAndClientCredentials,
                    AllowedScopes =
                    {
                        IdentityServerConstants.StandardScopes.Profile,
                        Shared.Apis.AgendaBusinessApiName,
                        "roles",
                    },
                    Properties = new Dictionary<string, string>
                    {
                        {"dbConnection", "AgendaApiConnection"}
                    }
                },
                new Client
                {
                    AccessTokenLifetime = 696969,
                    AllowOfflineAccess = true,
                    AllowedCorsOrigins = new List<string>
                    {
                        "http://app.novbem.az",
                        "http://app-test.novbem.az",

                        "https://app.novbem.az",
                        "https://app-test.novbem.az",

                        "http://reception.novbem.az",
                        "http://reception-test.novbem.az",

                        "https://reception.novbem.az",
                        "https://reception-test.novbem.az",

                        "http://restaurant.novbem.az",
                        "http://restaurant-test.novbem.az",

                        "https://restaurant.novbem.az",
                        "https://restaurant-test.novbem.az",

                        "http://localhost:8080",
                    },
                    UpdateAccessTokenClaimsOnRefresh = true,
                    ClientName = "AdminClient",
                    ClientId = AdminClientUid,
                    ClientSecrets = {new Secret("4feae546-0dd5-27b1-4d59-490b455ca045".Sha256())},
                    AllowedGrantTypes = GrantTypes.ResourceOwnerPasswordAndClientCredentials,
                    AllowedScopes =
                    {
                        IdentityServerConstants.StandardScopes.Profile,
                        Shared.Apis.AgendaAdminApiName,
                        "roles",
                    },
                    Properties = new Dictionary<string, string>
                    {
                        {"dbConnection", "AgendaApiConnection"}
                    }
                },
                new Client
                {
                    AccessTokenLifetime = 696969,
                    AllowOfflineAccess = true,
                    AllowedCorsOrigins = new List<string>
                    {
                        "http://app.novbem.az",
                        "http://app-test.novbem.az",

                        "https://app.novbem.az",
                        "https://app-test.novbem.az",

                        "http://reception.novbem.az",
                        "http://reception-test.novbem.az",

                        "https://reception.novbem.az",
                        "https://reception-test.novbem.az",

                        "http://restaurant.novbem.az",
                        "http://restaurant-test.novbem.az",

                        "https://restaurant.novbem.az",
                        "https://restaurant-test.novbem.az",

                        "http://localhost:8080",
                    },
                    UpdateAccessTokenClaimsOnRefresh = true,
                    ClientName = "RestaurantClient",
                    ClientId = RestaurantClientUid,
                    ClientSecrets = {new Secret("57f58797-cbe2-2da9-435d-5a27b15ebdf0".Sha256())},
                    AllowedGrantTypes = GrantTypes.ResourceOwnerPasswordAndClientCredentials,
                    AllowedScopes =
                    {
                        IdentityServerConstants.StandardScopes.Profile,
                        Shared.Apis.AgendaRestaurantApiName,
                        "roles",
                    },
                    Properties = new Dictionary<string, string>
                    {
                        {"dbConnection", "AgendaApiConnection"}
                    }
                },
                new Client
                {
                    AccessTokenLifetime = 696969,
                    AllowOfflineAccess = true,
                    AllowedCorsOrigins = new List<string>
                    {
                        "http://localhost:8080",
                        "https://localhost:8080",
                        "http://localhost:8081",
                        "http://septem.zeynalli.com",
                        "https://septem.zeynalli.com",
                    },
                    UpdateAccessTokenClaimsOnRefresh = true,
                    ClientName = "TaskManagerClient",
                    ClientId = TaskManagerClientUid,
                    ClientSecrets = {new Secret("318d1a0b-a974-0baa-4779-68eaf9eb764a".Sha256())},
                    AllowedGrantTypes = GrantTypes.ResourceOwnerPasswordAndClientCredentials,
                    AllowedScopes =
                    {
                        IdentityServerConstants.StandardScopes.Profile,
                        Shared.Apis.TaskManagerApiName,
                        "roles",
                    },
                    Properties = new Dictionary<string, string>
                    {
                        {"dbConnection", "TaskManagerApiConnection"}
                    }
                },
                new Client
                {
                    AccessTokenLifetime = 696969,
                    AllowOfflineAccess = true,
                    AllowedCorsOrigins = new List<string>
                    {
                        "http://localhost:8080",
                        "https://localhost:8080",
                        "http://localhost:8081",
                        "https://localhost:8081",
                        "http://localhost:8082",
                        "https://localhost:8082",
                    },
                    UpdateAccessTokenClaimsOnRefresh = true,
                    ClientName = "CyberArena",
                    ClientId = CyberArenaClientUid,
                    ClientSecrets = {new Secret("7e777066-d4cf-e188-45ed-dff894c61c1d".Sha256())},
                    AllowedGrantTypes = GrantTypes.ResourceOwnerPasswordAndClientCredentials,
                    AllowedScopes =
                    {
                        IdentityServerConstants.StandardScopes.Profile,
                        Shared.Apis.CyberArenaApiName,
                        "roles",
                    },
                    Properties = new Dictionary<string, string>
                    {
                        {"dbConnection", "CyberArenaApiConnection"}
                    }
                },

                new Client {
                    AccessTokenLifetime = 200,
                    AllowOfflineAccess = true,
                    AllowedCorsOrigins = new List<string>
                    {
                        "http://localhost:8080",
                        "https://localhost:8080",
                        "http://localhost:8081",
                        "https://localhost:8081",
                    },
                    UpdateAccessTokenClaimsOnRefresh = true,
                    ClientName = "IikoSyncAppLicenseClient",
                    ClientId = IikoSyncAppClientUid,
                    ClientSecrets = {new Secret("6ee560ad-8b8d-2c86-4db8-57a66b9ff1e1".Sha256())},
                    AllowedGrantTypes = GrantTypes.ResourceOwnerPasswordAndClientCredentials,
                    AllowedScopes =
                    {
                        IdentityServerConstants.StandardScopes.Profile,
                        Shared.Apis.LicenseManagementWebApiName,
                        "roles",
                    },
                    Properties = new Dictionary<string, string>
                    {
                        {"dbConnection", "CyberArenaApiConnection"}
                    }
                },

                new Client {
                    AccessTokenLifetime = 200,
                    AllowOfflineAccess = true,
                    AllowedCorsOrigins = new List<string>
                    {
                        "http://localhost:8080",
                        "https://localhost:8080",
                        "http://localhost:8081",
                        "https://localhost:8081",
                    },
                    UpdateAccessTokenClaimsOnRefresh = true,
                    ClientName = "IikoSyncAppLicenseClient",
                    ClientId = LicenseAdminClientUid,
                    ClientSecrets = {new Secret("c6ea3a35-27b9-6898-4452-5db8dfaa34b1".Sha256())},
                    AllowedGrantTypes = GrantTypes.ResourceOwnerPasswordAndClientCredentials,
                    AllowedScopes =
                    {
                        IdentityServerConstants.StandardScopes.Profile,
                        Shared.Apis.LicenseManagementWebApiName,
                        "roles",
                    },
                    Properties = new Dictionary<string, string>
                    {
                        {"dbConnection", "CyberArenaApiConnection"}
                    }
                },
            };
    }
}
