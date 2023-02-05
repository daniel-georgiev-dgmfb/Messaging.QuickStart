//  ----------------------------------------------------------------------- 
//   <copyright file="RolePermission.cs" company="Glasswall Solutions Ltd.">
//       Glasswall Solutions Ltd.
//   </copyright>
//  ----------------------------------------------------------------------- 

namespace Glasswall.Domain.Tenancy
{
    using Kernel.Data;

    public class RolePermission : BaseModel
    {
        public virtual Role Role { get; set; }

        public virtual Permission Permission { get; set; }
    }
}