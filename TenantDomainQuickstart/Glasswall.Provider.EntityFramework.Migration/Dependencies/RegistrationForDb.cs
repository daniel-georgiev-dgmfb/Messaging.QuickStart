using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data.SqlClient;
using Glasswall.FileTrust.Domain;
using Glasswall.Kernel.Data;
using Glasswall.Kernel.Data.Connection;
using Glasswall.Kernel.Data.ORM;
using Glasswall.Kernel.DependencyResolver;
using Glasswall.Kernel.Reflection.Reflection;
using Glasswall.Kernel.Tenancy;
using Glasswall.Providers.EntityFramework;
using Glasswall.Providers.EntityFramework.Connection;
using Glasswall.Providers.EntityFramework.Extensions;
using Glasswall.Providers.Logging.Microsoft;
using Microsoft.EntityFrameworkCore;
using Provider.EntityFramework.Mapping.Mappings;
using Provider.EntityFramework.Migration;

namespace Glasswall.FileTrust.API.Dependencies
{
    internal class RegistrationForDb
    {
        private const string HubCacheKey = "Hub_Context";

        public static void Register(IDependencyResolver resolver)
        {
            var models = ReflectionHelper.GetAllTypes(new[] { typeof(BaseHubModel).Assembly }, t => !t.IsInterface && !t.IsAbstract && typeof(BaseModel).IsAssignableFrom(t));
            Func<IEnumerable<IDbMapper>> mapperFactory = () => resolver.ResolveAll<IDbMapper>();
            var configuration = new DbCustomConfiguration(() => models, mapperFactory, RegistrationForDb.HubCacheKey);
            DbContextExtensions.AddGlasswallDbContext(resolver, configuration);
            DbContextExtensions.AddTransientSqlProviderWithOptions(resolver, () => resolver.Resolve<IConnectionStringProvider<SqlConnectionStringBuilder>>(), typeof(Logger<>));
            resolver.RegisterType<IDbMapper, HubDbMapper>(Lifetime.Transient);
            resolver.RegisterFactory<Func<NameValueCollection>>(() =>
                    () =>
                    {
                        var tenantContext = resolver.Resolve<TenantResolutionContext>();
                        var tenantManager = resolver.Resolve<ITenantManager>();
                        var tenantDescriptor = tenantManager.GetTenantDescriptor(tenantContext)
                        .GetAwaiter()
                        .GetResult();
                        if (tenantDescriptor == null)
                            throw new InvalidOperationException("Tenant descriptor cannot be resolved.");
                        return tenantDescriptor.TenantAttributes;
                    }, Lifetime.Singleton);
        }
    }
}