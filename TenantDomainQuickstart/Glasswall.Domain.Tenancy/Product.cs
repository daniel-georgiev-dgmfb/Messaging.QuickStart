//  ----------------------------------------------------------------------- 
//   <copyright file="Product.cs" company="Glasswall Solutions Ltd.">
//       Glasswall Solutions Ltd.
//   </copyright>
//  ----------------------------------------------------------------------- 

namespace Glasswall.Domain.Tenancy
{
    using Kernel.Data;

    public class Product : BaseModel
    {
        public string Name { get; set; }
    }
}