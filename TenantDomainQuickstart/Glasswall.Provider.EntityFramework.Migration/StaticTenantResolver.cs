using System;
using System.Threading.Tasks;
using Glasswall.Common.Tenancy.Tenancy;
using Glasswall.Kernel.Tenancy;

namespace Provider.EntityFramework.Migration
{
    public class StaticTenantResolver : TenantResolver<Guid>
    {
        private readonly Guid _tenant;

        public StaticTenantResolver(Guid tenant)
        {
            this._tenant = tenant;
        }
       

        protected override Guid ResolveSource()
        {
            return this._tenant;
        }

        protected override Task ResolveTenantInternal(Guid source, TenantResolutionContext context)
        {
            context.Resolved(new TenantDescriptor(source));
            return Task.CompletedTask;
        }
    }
}