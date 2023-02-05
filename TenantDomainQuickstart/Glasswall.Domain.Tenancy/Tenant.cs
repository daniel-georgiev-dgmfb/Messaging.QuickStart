//  ----------------------------------------------------------------------- 
//   <copyright file="Tenant.cs" company="Glasswall Solutions Ltd.">
//       Glasswall Solutions Ltd.
//   </copyright>
//  ----------------------------------------------------------------------- 

namespace Glasswall.Domain.Tenancy
{
    using System.Collections.Generic;
    using Kernel.Data;
    using Kernel.Data.Tenancy;

    public class Tenant : BaseModel
    {
        private ICollection<TenantUser> _tenantUsers;

        public string Shard { get; set; }

        public string Database { get; set; }

        public string Region { get; set; }

        public bool Active { get; set; }

        public string OwnerName { get; set; }

        public string OwnerEmail { get; set; }

        public virtual ICollection<TenantUser> TenantUsers
        {
            get => _tenantUsers ?? (_tenantUsers = new HashSet<TenantUser>());
            protected set => _tenantUsers = value;
        }
    }
}