//  ----------------------------------------------------------------------- 
//   <copyright file="TenantUser.cs" company="Glasswall Solutions Ltd.">
//       Glasswall Solutions Ltd.
//   </copyright>
//  ----------------------------------------------------------------------- 

namespace Glasswall.Domain.Tenancy
{
    using System;
    using Kernel.Data.Tenancy;

    public class TenantUser : BaseTenantModel
    {
        public virtual User User { get; set; }

        public virtual Tenant Tenant { get; set; }

        public bool Active { get; set; }
    }
}