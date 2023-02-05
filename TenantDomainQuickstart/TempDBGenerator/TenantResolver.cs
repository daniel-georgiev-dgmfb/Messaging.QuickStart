//  ----------------------------------------------------------------------- 
//   <copyright file="TenantResolver.cs" company="Glasswall Solutions Ltd.">
//       Glasswall Solutions Ltd.
//   </copyright>
//  ----------------------------------------------------------------------- 

namespace TempDBGenerator
{
    using System;
    using System.Threading.Tasks;
    using Glasswall.Common.Tenancy.Tenancy;
    using Glasswall.Kernel.Tenancy;

    public class TenantResolver : TenantResolver<Guid>
    {
        private readonly Guid _tenant;

        public TenantResolver(Guid tenant)
        {
            _tenant = tenant;
        }
        
        protected override Guid ResolveSource()
        {
            return _tenant;
        }

        protected override Task ResolveTenantInternal(Guid source, TenantResolutionContext context)
        {
            var descriptor = new TenantDescriptor(source);
            context.Resolved(descriptor);
            return Task.CompletedTask;
        }
    }
}