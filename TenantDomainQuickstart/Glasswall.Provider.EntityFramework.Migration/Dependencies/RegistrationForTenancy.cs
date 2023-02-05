using System;
using Glasswall.Kernel.Data.Tenancy;
using Glasswall.Kernel.DependencyResolver;
using Glasswall.Kernel.Security.Validation;
using Glasswall.Kernel.Tenancy;
using Glasswall.Kernel.Web;
using Glasswall.Platform.Cryptography.Extensions;
using Glasswall.Platform.Web.Tokens.Contexts;
using Glasswall.Tenant.Management;
using Glasswall.Tenant.Management.Extensions;
using Provider.EntityFramework.Migration;

namespace Glasswall.FileTrust.API.Dependencies
{
    internal class RegistrationForTenancy
    {
        public static void Register(IDependencyResolver resolver)
        {
            TenantManagmenExtensions.AddTenancy(resolver);
            resolver.RegisterType<ITenantResolver, StaticTenantResolver>(Lifetime.Transient);
            CryptographyExtensions.AddBackchannelCertificateValidation(resolver);
            resolver.RegisterType<IBackchannelCertificateValidationRule, CustomBackchannelValidationRule>(Lifetime.Transient);
            resolver.RegisterFactory<TenantResolutionContext>(() =>
            {
                var endpoint = new Endpoint("https://localhost:5001/api/tenant");
                var credentials = new ClientSecretTokenContext("service", "Glasswall", new Endpoint("https://localhost:7001/connect/token"));
                return new TenantResolutionContext(endpoint, credentials);
            }, Lifetime.Singleton);
            resolver.RegisterFactory<Guid>(() => Guid.Parse("1B7718A9-5630-4519-9C01-798C229098FF"), Lifetime.Singleton);
            resolver.RegisterType<ITenantFilterBuilder, TenantFilterBuilder>(Lifetime.Transient);
        }
    }
}