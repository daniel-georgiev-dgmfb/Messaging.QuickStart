//  ----------------------------------------------------------------------- 
//   <copyright file="GroupUser.cs" company="Glasswall Solutions Ltd.">
//       Glasswall Solutions Ltd.
//   </copyright>
//  ----------------------------------------------------------------------- 

namespace Glasswall.Domain.Tenancy
{
    using System;
    using Kernel.Data;

    public class GroupUser : BaseModel
    {
        public Guid GroupId { get; set; }

        public virtual Group Group { get; set; }

        public Guid UserId { get; set; }

        public virtual User User { get; set; }
    }
}