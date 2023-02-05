//  ----------------------------------------------------------------------- 
//   <copyright file="Group.cs" company="Glasswall Solutions Ltd.">
//       Glasswall Solutions Ltd.
//   </copyright>
//  ----------------------------------------------------------------------- 

namespace Glasswall.Domain.Tenancy
{
    using System;
    using System.Collections.Generic;
    using Kernel.Data.Tenancy;

    public class Group : BaseTenantModel
    {
        private ICollection<GroupUser> _groupUsers;

        public string Name { get; set; }

        public string DisplayName { get; set; }

        public bool Active { get; set; }

        public virtual Tenant Tenant { get; set; }

        public Guid ProductId { get; set; }

        public virtual Product Product { get; set; }

        public virtual ICollection<GroupUser> GroupUsers
        {
            get => _groupUsers ?? (_groupUsers = new HashSet<GroupUser>());
            protected set => _groupUsers = value;
        }
    }
}