//  ----------------------------------------------------------------------- 
//   <copyright file="EFCoreFilterExtensions.cs" company="Glasswall Solutions Ltd.">
//       Glasswall Solutions Ltd.
//   </copyright>
//  ----------------------------------------------------------------------- 

namespace Glasswall.Providers.EntityFramework.Filters
{
    using System;
    using System.Linq;
    using System.Reflection;
    using Kernel.Data.ORM;
    using Kernel.Data.Tenancy;
    using Microsoft.EntityFrameworkCore;

    public static class EfCoreFilterExtensions
    {
        public static readonly MethodInfo SetTenantFilterMethod = typeof(GlasswallDbContext)
            .GetMethods(BindingFlags.Public | BindingFlags.Static)
            .Single(t => t.IsGenericMethod && t.Name == "SetTenantFilter");

        private static void SetTenantFilter(this ModelBuilder modelBuilder, Type entityType,
            IDbCustomConfiguration configuration)
        {
            SetTenantFilterMethod.MakeGenericMethod(entityType)
                .Invoke(null, new object[] {modelBuilder, configuration});
        }

        public static void SetTenantFilter<TEntity>(this ModelBuilder modelBuilder,  IDbCustomConfiguration configuration)
            where TEntity : BaseTenantModel
        {
            modelBuilder.Entity<TEntity>()
                .HasQueryFilter(x => x.TenantId == configuration.TenantManager().ResolveTenant() && !x.IsDeleted);
        }
    }
}