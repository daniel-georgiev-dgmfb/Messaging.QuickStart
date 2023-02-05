//  ----------------------------------------------------------------------- 
//   <copyright file="Permission.cs" company="Glasswall Solutions Ltd.">
//       Glasswall Solutions Ltd.
//   </copyright>
//  ----------------------------------------------------------------------- 

namespace Glasswall.Domain.Tenancy
{
    using Kernel.Data;
    public class Permission : BaseModel
    {
        public string DisplayName { get; set; }

        public string Description { get; set; }

        public bool Active { get; set; }

        public int EnumKey { get; set; }

        public virtual Product Product { get; set; }
    }
}