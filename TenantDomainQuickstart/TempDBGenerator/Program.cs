namespace TempDBGenerator
{
    using System;
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.Data.SqlClient;
    using System.Diagnostics;
    using Configuration;
    using Glasswall.Common.Configuration;
    using Glasswall.Common.Serialisation.JSON.Extensions;
    using Glasswall.Domain.Tenancy;
    using Glasswall.Kernel.Configuration;
    using Glasswall.Kernel.Data;
    using Glasswall.Kernel.Data.Connection;
    using Glasswall.Kernel.Data.ORM;
    using Glasswall.Kernel.DependencyResolver;
    using Glasswall.Kernel.Logging;
    using Glasswall.Kernel.Reflection.Reflection;
    using Glasswall.Kernel.Security.SecretManagement;
    using Glasswall.Kernel.Security.Validation;
    using Glasswall.Kernel.Tenancy;
    using Glasswall.Kernel.Web;
    using Glasswall.Platform.Cryptography.Extensions;
    using Glasswall.Platform.Cryptography.Secrets;
    using Glasswall.Platform.Cryptography.Stores.Azure;
    using Glasswall.Platform.Web.HttpClient.Extensions;
    using Glasswall.Platform.Web.Tokens.Contexts;
    using Glasswall.Platform.Web.Tokens.Extensions;
    using Glasswall.Providers.EntityFramework.Connection;
    using Glasswall.Providers.EntityFramework.Extensions;
    using Glasswall.Providers.Logging.Console;
    using Glasswall.Providers.Logging.Microsoft;
    using Glasswall.Providers.Microsoft.DependencyInjection;
    using Glasswall.Tenancy.Domain.EF.Mapping;
    using Glasswall.Tenant.Management.Extensions;
    using MemoryCacheProvider.Extensions;
    using Validation.Backchannel;

    class Program
    {
        private const int NumberOfTenants = 5;

        static void Main(string[] args)
        {
            var timer = new Stopwatch();
            var resolver = new MicrosoftDependencyInjection();
            
            TenantManagmenExtensions.AddTenancy(resolver);
            TokenManagmentExtensions.AddTokenManagement(resolver);
            HttpClientExtensions.AddHttpClient(resolver);
            CryptographyExtensions.AddBackchannelCertificateValidation(resolver);
            LoggingExtensions.AddLogging(resolver);
            ConsoleLoggingExtensions.AddConsole(resolver);
            CacheProviderExtensions.AddInMemoryCache(resolver);
            JsonSerializerInitialiser.AddJsonSerialisation(resolver);

            resolver.RegisterType<ISecretManager, SecretManager>(Lifetime.Transient);
            resolver.RegisterType<ISecretStore, AzureSecretVault>(Lifetime.Transient);
            resolver.RegisterType<IEntityCreator, EntityCreator>(Lifetime.Transient);
            resolver.RegisterType<IRoleCreationHandler, RoleCreationHandler>(Lifetime.Transient);
            resolver.RegisterFactory<TenantResolutionContext>(() =>
            {
                var endpoint = new Endpoint("https://localhost:5001/api/tenant");
                var credentials = new ClientSecretTokenContext("service", "Glasswall", new Endpoint("https://cas.wotsits.filetrust.io/Connect/Token"));
                return new TenantResolutionContext(endpoint, credentials);
            }, Lifetime.Singleton);
            resolver.RegisterType<IConnectionDefinitionParser, ConnectionDefinitionParser>(Lifetime.Transient);
            resolver.RegisterType<IConnectionStringProvider<SqlConnectionStringBuilder>, SQLServerConnectionStringBuilder>(Lifetime.Transient);
            resolver.RegisterType<IDbMapper, TenancyMapper>(Lifetime.Transient);
            resolver.RegisterType<IConfiguration, EnvironmentVariableConfiguration>(Lifetime.Singleton);
            resolver.RegisterType<IBackchannelCertificateValidationRule, CustomBackchannelValidationRule>(Lifetime.Transient);
            
            resolver.RegisterType<ICertificateValidationConfigurationProvider, DefaultCertificateValidationConfigurationProvider>(Lifetime.Transient);
            var models = ReflectionHelper.GetAllTypes(new[] { typeof(Tenant).Assembly }, t => !t.IsInterface && !t.IsAbstract && typeof(BaseModel).IsAssignableFrom(t));
            Func<IEnumerable<IDbMapper>> mapperFactory = () => resolver.ResolveAll<IDbMapper>();

            var configuration = new DbCustomConfiguration(() => models, mapperFactory, "Test");
            DbContextExtensions.AddGlasswallDbContext(resolver, configuration);
            DbContextExtensions.AddTransientSqlProviderWithOptions(resolver, () => resolver.Resolve<IConnectionStringProvider<SqlConnectionStringBuilder>>(), typeof(Logger<>));

            resolver.RegisterFactory<IEnumerable<ITenantResolver>>(() => new[] { new TenantResolver(Guid.Parse("9629FA3E-0E12-4942-9288-127E9A0B6B31")) }, Lifetime.Singleton);
            
            resolver.RegisterFactory<Func<NameValueCollection>>(() =>
                    () => new NameValueCollection
                    {
                        {
                            "DataSource",
                            "localhost\\SQLEXPRESS"
                        },
                        {
                            "DataBase",
                            "TenantDomain7"
                        },
                        {
                            "UserName",
                            String.Empty
                        },
                        {
                            "Password",
                            String.Empty
                        }
                    }, Lifetime.Transient);
            
            resolver.Initialise();

            var logger = resolver.Resolve<IGWLogger<Program>>();
            var entityCreator = resolver.Resolve<IEntityCreator>();
            var roleCreationHandler = resolver.Resolve<IRoleCreationHandler>();
            var context = resolver.Resolve<IDbContext>();

            try
            {
                logger.LogInformation("Creating Tenant Entities");
                var tenant1 = entityCreator.CreateTenantEntity("Tenant1", "Tenant One Display Name");
                var tenant2 = entityCreator.CreateTenantEntity("Tenant2", "Tenant Two Display Name");
                var tenant3 = entityCreator.CreateTenantEntity("Tenant3", "Tenant Three Display Name");

                logger.LogInformation("Creating Group Entities");
                var tenant1SupportGroup = entityCreator.CreateGroupEntity("Tenant1Support", "Tenant 1 Support Group");
                var tenant1AdminGroup = entityCreator.CreateGroupEntity("Tenant1Administrator", "Tenant 1 Administrator Group");
                var tenant1ManagementGroup = entityCreator.CreateGroupEntity("Tenant1Management", "Tenant 1 Management Group");
                var tenant1InternGroup = entityCreator.CreateGroupEntity("Tenant1Interns", "Tenant 1 Interns Group");
                var tenant2SupportGroup = entityCreator.CreateGroupEntity("Tenant2Support", "Tenant 2 Support Group");
                var tenant2AdminGroup = entityCreator.CreateGroupEntity("Tenant2Administrator", "Tenant 2 Administrator Group");
                var tenant2ManagementGroup = entityCreator.CreateGroupEntity("Tenant2Management", "Tenant 2 Management Group");
                var tenant2InternGroup = entityCreator.CreateGroupEntity("Tenant2Interns", "Tenant 2 Interns Group");
                var tenant3SupportGroup = entityCreator.CreateGroupEntity("Tenant3Support", "Tenant 3 Support Group");
                var tenant3AdminGroup = entityCreator.CreateGroupEntity("Tenant3Administrator", "Tenant 3 Administrator Group");
                var tenant3ManagementGroup = entityCreator.CreateGroupEntity("Tenant3Management", "Tenant 3 Management Group");
                var tenant3InternGroup = entityCreator.CreateGroupEntity("Tenant3Interns", "Tenant 3 Interns Group");

                logger.LogInformation("Creating User Entities");
                var user1 = entityCreator.CreateUserEntity("User 1");
                var user2 = entityCreator.CreateUserEntity("User 2");
                var user3 = entityCreator.CreateUserEntity("User 3");
                var user4 = entityCreator.CreateUserEntity("User 4");
                var user5 = entityCreator.CreateUserEntity("User 5");
                var user6 = entityCreator.CreateUserEntity("User 6");
                var user7 = entityCreator.CreateUserEntity("User 7");

                logger.LogInformation("Creating Role Entities");
                var tenant1SuperUserRole = roleCreationHandler.CreateSuperUserRole();
                var tenant1TiViewerRole = roleCreationHandler.CreateThreatIntelligenceViewerRole();
                var tenant1PolicyAuthorRole = roleCreationHandler.CreatePolicyAuthorRole();
                var tenant1FileReleaserRole = roleCreationHandler.CreateFileReleaserRole();
                var tenant2SuperUserRole = roleCreationHandler.CreateSuperUserRole();
                var tenant2TiViewerRole = roleCreationHandler.CreateThreatIntelligenceViewerRole();
                var tenant2PolicyAuthorRole = roleCreationHandler.CreatePolicyAuthorRole();
                var tenant2FileReleaserRole = roleCreationHandler.CreateFileReleaserRole();
                var tenant3SuperUserRole = roleCreationHandler.CreateSuperUserRole();
                var tenant3TiViewerRole = roleCreationHandler.CreateThreatIntelligenceViewerRole();
                var tenant3PolicyAuthorRole = roleCreationHandler.CreatePolicyAuthorRole();
                var tenant3FileReleaserRole = roleCreationHandler.CreateFileReleaserRole();

                logger.LogInformation("Creating Group User Entities");
                var user1Tenant1SupportGroupUser = entityCreator.CreateGroupUserEntity("User 1 Tenant 1 Support Group User", tenant1SupportGroup, user1);
                var user1Tenant3SupportGroupUser = entityCreator.CreateGroupUserEntity("User 1 Tenant 3 Support  Group User", tenant3SupportGroup, user1);
                var user2Tenant1ManagementGroupUser = entityCreator.CreateGroupUserEntity("User 2 Tenant 3 Management Group User", tenant1ManagementGroup, user2);
                var user2Tenant3ManagementGroupUser = entityCreator.CreateGroupUserEntity("User 2 Tenant 3 Management Group User", tenant3ManagementGroup, user2);
                var user3Tenant1InternGroupUser = entityCreator.CreateGroupUserEntity("User 3 Tenant 1 Intern Group User", tenant1InternGroup, user3);
                var user4Tenant1AdminGroupUser = entityCreator.CreateGroupUserEntity("User 4 Tenant 1 Admin Group User", tenant1AdminGroup, user4);
                var user4Tenant2AdminGroupUser = entityCreator.CreateGroupUserEntity("User 4 Tenant 2 Admin Group User", tenant2AdminGroup, user4);
                var user4Tenant3AdminGroupUser = entityCreator.CreateGroupUserEntity("User 4 Tenant 3 Admin Group User", tenant3AdminGroup, user4);
                var user5Tenant2SupportGroupUser = entityCreator.CreateGroupUserEntity("User 5 Tenant 2 Support Group User", tenant2SupportGroup, user5);
                var user6Tenant2ManagementGroupUser = entityCreator.CreateGroupUserEntity("User 6 Tenant 2 Management Group User", tenant2ManagementGroup, user6);
                var user7Tenant2InternGroupUser = entityCreator.CreateGroupUserEntity("User 7 Tenant 2 Intern Group User", tenant2InternGroup, user7);
                var user7Tenant3InternGroupUser = entityCreator.CreateGroupUserEntity("User 7 Tenant 3 Intern Group User", tenant3InternGroup, user7);

                logger.LogInformation("Creating Group Role Entities");
                var tenant1SupportGroupFileReleaseRole = entityCreator.CreateGroupRoleEntity("Tenant 1 Support Group File Release Role", tenant1SupportGroup, tenant1FileReleaserRole);
                var tenant1SupportGroupPolicyAuthorRole = entityCreator.CreateGroupRoleEntity("Tenant 1 Support Group Policy Author Role", tenant1SupportGroup, tenant1PolicyAuthorRole);
                var tenant1AdminGroupRole = entityCreator.CreateGroupRoleEntity("Tenant 1 Admin Group Super User Role", tenant1AdminGroup, tenant1SuperUserRole);
                var tenant1ManagementGroupRole = entityCreator.CreateGroupRoleEntity("Tenant 1 Management Group TI Viewer Role", tenant1ManagementGroup, tenant1TiViewerRole);
                var tenant1InternGroupFileReleaseRole = entityCreator.CreateGroupRoleEntity("Tenant 1 Intern Group File Release Role", tenant1InternGroup, tenant1FileReleaserRole);
                var tenant2SupportGroupFileReleaseRole = entityCreator.CreateGroupRoleEntity("Tenant 2 Support Group File Release Role", tenant2SupportGroup, tenant2FileReleaserRole);
                var tenant2SupportGroupPolicyAuthorRole = entityCreator.CreateGroupRoleEntity("Tenant 2 Support Group Policy Author Role", tenant2SupportGroup, tenant2PolicyAuthorRole);
                var tenant2AdminGroupRole = entityCreator.CreateGroupRoleEntity("Tenant 2 Admin Group Super User Role", tenant2AdminGroup, tenant2SuperUserRole);
                var tenant2ManagementGroupRole = entityCreator.CreateGroupRoleEntity("Tenant 2 Management Group TI Viewer Role", tenant2ManagementGroup, tenant2TiViewerRole);
                var tenant2InternGroupFileReleaseRole = entityCreator.CreateGroupRoleEntity("Tenant 2 Intern Group File Release Role", tenant2InternGroup, tenant2FileReleaserRole);
                var tenant3SupportGroupFileReleaseRole = entityCreator.CreateGroupRoleEntity("Tenant 3 Support Group File Release Role", tenant3SupportGroup, tenant3FileReleaserRole);
                var tenant3SupportGroupPolicyAuthorRole = entityCreator.CreateGroupRoleEntity("Tenant 3 Support Group Policy Author Role", tenant3SupportGroup, tenant3PolicyAuthorRole);
                var tenant3AdminGroupRole = entityCreator.CreateGroupRoleEntity("Tenant 3 Admin Group Super User Role", tenant3AdminGroup, tenant3SuperUserRole);
                var tenant3ManagementGroupRole = entityCreator.CreateGroupRoleEntity("Tenant 3 Management Group TI Viewer Role", tenant3ManagementGroup, tenant3TiViewerRole);
                var tenant3InternGroupFileReleaseRole = entityCreator.CreateGroupRoleEntity("Tenant 3 Intern Group File Release Role", tenant3InternGroup, tenant3FileReleaserRole);

                logger.LogInformation("Creating Tenant User Entities");
                var tenant1User1 = entityCreator.CreateTenantUserEntity("Tenant 1 User 1", tenant1, user1);
                var tenant1User2 = entityCreator.CreateTenantUserEntity("Tenant 1 User 2", tenant1, user2);
                var tenant1User3 = entityCreator.CreateTenantUserEntity("Tenant 1 User 3", tenant1, user3);
                var tenant1User4 = entityCreator.CreateTenantUserEntity("Tenant 1 User 4", tenant1, user4);
                var tenant2User5 = entityCreator.CreateTenantUserEntity("Tenant 2 User 5", tenant2, user5);
                var tenant2User6 = entityCreator.CreateTenantUserEntity("Tenant 2 User 6", tenant2, user6);
                var tenant2User7 = entityCreator.CreateTenantUserEntity("Tenant 2 User 7", tenant2, user7);
                var tenant2User4 = entityCreator.CreateTenantUserEntity("Tenant 2 User 4", tenant2, user4);
                var tenant3User1 = entityCreator.CreateTenantUserEntity("Tenant 3 User 1", tenant3, user1);
                var tenant3User2 = entityCreator.CreateTenantUserEntity("Tenant 3 User 2", tenant3, user2);
                var tenant3User7 = entityCreator.CreateTenantUserEntity("Tenant 3 User 7", tenant3, user7);
                var tenant3User4 = entityCreator.CreateTenantUserEntity("Tenant 3 User 4", tenant3, user4);

                logger.LogInformation("Adding Groups to Tenants");
                tenant1.Groups.Add(tenant1AdminGroup);
                tenant1.Groups.Add(tenant1InternGroup);
                tenant1.Groups.Add(tenant1ManagementGroup);
                tenant1.Groups.Add(tenant1SupportGroup);
                tenant2.Groups.Add(tenant2AdminGroup);
                tenant2.Groups.Add(tenant2InternGroup);
                tenant2.Groups.Add(tenant2ManagementGroup);
                tenant2.Groups.Add(tenant2SupportGroup);
                tenant3.Groups.Add(tenant3AdminGroup);
                tenant3.Groups.Add(tenant3InternGroup);
                tenant3.Groups.Add(tenant3ManagementGroup);
                tenant3.Groups.Add(tenant3SupportGroup);

                logger.LogInformation("Adding Tenant Users to Tenants");
                tenant1.TenantUsers.Add(tenant1User1);
                tenant1.TenantUsers.Add(tenant1User2);
                tenant1.TenantUsers.Add(tenant1User3);
                tenant1.TenantUsers.Add(tenant1User4);
                tenant2.TenantUsers.Add(tenant2User4);
                tenant2.TenantUsers.Add(tenant2User5);
                tenant2.TenantUsers.Add(tenant2User6);
                tenant2.TenantUsers.Add(tenant2User7);
                tenant3.TenantUsers.Add(tenant3User4);
                tenant3.TenantUsers.Add(tenant3User1);
                tenant3.TenantUsers.Add(tenant3User2);
                tenant3.TenantUsers.Add(tenant3User7);

                logger.LogInformation("Linking Tenants to Roles");
                tenant1SuperUserRole.Tenant = tenant1;
                tenant1FileReleaserRole.Tenant = tenant1;
                tenant1PolicyAuthorRole.Tenant = tenant1;
                tenant1TiViewerRole.Tenant = tenant1;
                tenant2SuperUserRole.Tenant = tenant2;
                tenant2FileReleaserRole.Tenant = tenant2;
                tenant2PolicyAuthorRole.Tenant = tenant2;
                tenant2TiViewerRole.Tenant = tenant2;
                tenant3SuperUserRole.Tenant = tenant3;
                tenant3FileReleaserRole.Tenant = tenant3;
                tenant3PolicyAuthorRole.Tenant = tenant3;
                tenant3TiViewerRole.Tenant = tenant3;

                logger.LogInformation("Adding Group Users to Groups");
                tenant1AdminGroup.GroupUsers.Add(user4Tenant1AdminGroupUser);
                tenant1InternGroup.GroupUsers.Add(user3Tenant1InternGroupUser);
                tenant1ManagementGroup.GroupUsers.Add(user2Tenant1ManagementGroupUser);
                tenant1SupportGroup.GroupUsers.Add(user1Tenant1SupportGroupUser);
                tenant2AdminGroup.GroupUsers.Add(user4Tenant2AdminGroupUser);
                tenant2InternGroup.GroupUsers.Add(user7Tenant2InternGroupUser);
                tenant2ManagementGroup.GroupUsers.Add(user6Tenant2ManagementGroupUser);
                tenant2SupportGroup.GroupUsers.Add(user5Tenant2SupportGroupUser);
                tenant3AdminGroup.GroupUsers.Add(user4Tenant3AdminGroupUser);
                tenant3InternGroup.GroupUsers.Add(user7Tenant3InternGroupUser);
                tenant3ManagementGroup.GroupUsers.Add(user2Tenant3ManagementGroupUser);
                tenant3SupportGroup.GroupUsers.Add(user1Tenant3SupportGroupUser);

                logger.LogInformation("Adding Group Users to Users");
                user1.GroupUsers.Add(user1Tenant1SupportGroupUser);
                user1.GroupUsers.Add(user1Tenant3SupportGroupUser);
                user2.GroupUsers.Add(user2Tenant1ManagementGroupUser);
                user2.GroupUsers.Add(user2Tenant3ManagementGroupUser);
                user3.GroupUsers.Add(user3Tenant1InternGroupUser);
                user4.GroupUsers.Add(user4Tenant1AdminGroupUser);
                user4.GroupUsers.Add(user4Tenant2AdminGroupUser);
                user4.GroupUsers.Add(user4Tenant3AdminGroupUser);
                user5.GroupUsers.Add(user5Tenant2SupportGroupUser);
                user6.GroupUsers.Add(user6Tenant2ManagementGroupUser);
                user7.GroupUsers.Add(user7Tenant2InternGroupUser);
                user7.GroupUsers.Add(user7Tenant3InternGroupUser);

                logger.LogInformation("Adding Tenant Users to Users");
                user1.TenantUsers.Add(tenant1User1);
                user1.TenantUsers.Add(tenant3User1);
                user2.TenantUsers.Add(tenant1User2);
                user2.TenantUsers.Add(tenant3User2);
                user3.TenantUsers.Add(tenant1User3);
                user4.TenantUsers.Add(tenant1User4);
                user4.TenantUsers.Add(tenant2User4);
                user4.TenantUsers.Add(tenant3User4);
                user5.TenantUsers.Add(tenant2User5);
                user6.TenantUsers.Add(tenant2User6);
                user7.TenantUsers.Add(tenant2User7);
                user7.TenantUsers.Add(tenant3User7);

                logger.LogInformation("Adding Tenant Users to Tenants");
                tenant1.TenantUsers.Add(tenant1User1);
                tenant1.TenantUsers.Add(tenant1User2);
                tenant1.TenantUsers.Add(tenant1User3);
                tenant1.TenantUsers.Add(tenant1User4);
                tenant2.TenantUsers.Add(tenant2User5);
                tenant2.TenantUsers.Add(tenant2User6);
                tenant2.TenantUsers.Add(tenant2User7);
                tenant2.TenantUsers.Add(tenant2User4);
                tenant3.TenantUsers.Add(tenant3User1);
                tenant3.TenantUsers.Add(tenant3User2);
                tenant3.TenantUsers.Add(tenant3User4);
                tenant3.TenantUsers.Add(tenant3User7);

                logger.LogInformation("Adding Group Roles to Roles");
                tenant1SuperUserRole.GroupRoles.Add(tenant1AdminGroupRole);
                tenant1TiViewerRole.GroupRoles.Add(tenant1ManagementGroupRole);
                tenant1PolicyAuthorRole.GroupRoles.Add(tenant1SupportGroupPolicyAuthorRole);
                tenant1FileReleaserRole.GroupRoles.Add(tenant1SupportGroupFileReleaseRole);
                tenant2SuperUserRole.GroupRoles.Add(tenant2AdminGroupRole);
                tenant2TiViewerRole.GroupRoles.Add(tenant2ManagementGroupRole);
                tenant2PolicyAuthorRole.GroupRoles.Add(tenant2SupportGroupPolicyAuthorRole);
                tenant2FileReleaserRole.GroupRoles.Add(tenant2SupportGroupFileReleaseRole);
                tenant3SuperUserRole.GroupRoles.Add(tenant3AdminGroupRole);
                tenant3TiViewerRole.GroupRoles.Add(tenant3ManagementGroupRole);
                tenant3PolicyAuthorRole.GroupRoles.Add(tenant3SupportGroupPolicyAuthorRole);
                tenant3FileReleaserRole.GroupRoles.Add(tenant3SupportGroupFileReleaseRole);

                logger.LogInformation("Adding Group Roles to Groups");
                tenant1SupportGroup.Roles.Add(tenant1SupportGroupFileReleaseRole);
                tenant1SupportGroup.Roles.Add(tenant1SupportGroupPolicyAuthorRole);
                tenant1AdminGroup.Roles.Add(tenant1AdminGroupRole);
                tenant1ManagementGroup.Roles.Add(tenant1ManagementGroupRole);
                tenant1InternGroup.Roles.Add(tenant1InternGroupFileReleaseRole);
                tenant2SupportGroup.Roles.Add(tenant2SupportGroupFileReleaseRole);
                tenant2SupportGroup.Roles.Add(tenant2SupportGroupPolicyAuthorRole);
                tenant2AdminGroup.Roles.Add(tenant2AdminGroupRole);
                tenant2ManagementGroup.Roles.Add(tenant2ManagementGroupRole);
                tenant2InternGroup.Roles.Add(tenant2InternGroupFileReleaseRole);
                tenant3SupportGroup.Roles.Add(tenant3SupportGroupFileReleaseRole);
                tenant3SupportGroup.Roles.Add(tenant3SupportGroupPolicyAuthorRole);
                tenant3AdminGroup.Roles.Add(tenant3AdminGroupRole);
                tenant3ManagementGroup.Roles.Add(tenant3ManagementGroupRole);
                tenant3InternGroup.Roles.Add(tenant3InternGroupFileReleaseRole);
              
                logger.LogInformation("Adding Tenant to Context");
                context.Add(tenant1);
                context.Add(tenant2);
                context.Add(tenant3);

                logger.LogInformation("Calling SaveChanges on the context");
                context.SaveChanges();
                logger.LogInformation("Seeding database: {0} complete successfully in {1}", "TenantDomain7", timer.Elapsed);
            }
            catch(Exception e)
            {
                logger.LogError(e, "Seeding database: {0} failed", "TenantDomain7");
            }
            timer.Stop();
            logger.LogInformation("Press Enter to exist");
            Console.ReadLine();
        }
    }
}