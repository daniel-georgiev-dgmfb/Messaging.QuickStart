//  ----------------------------------------------------------------------- 
//   <copyright file="Role.cs" company="Glasswall Solutions Ltd.">
//       Glasswall Solutions Ltd.
//   </copyright>
//  ----------------------------------------------------------------------- 

namespace Glasswall.Domain.Tenancy
{
    using System;
    using System.Collections.Generic;
    using Kernel.Data.Tenancy;

    public class Role : BaseTenantModel
    {
        private ICollection<RolePermission> _rolePermissions;

        public string DisplayName { get; set; }

        public string Description { get; set; }

        public Guid ProductId { get; set; }

        public virtual Product Product { get; set; }

        public virtual Tenant Tenant { get; set; }

        public virtual ICollection<RolePermission> RolePermissions
        {
            get => _rolePermissions ?? (_rolePermissions = new HashSet<RolePermission>());
            protected set => _rolePermissions = value;
        }
    }
}