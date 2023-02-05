//  ----------------------------------------------------------------------- 
//   <copyright file="User.cs" company="Glasswall Solutions Ltd.">
//       Glasswall Solutions Ltd.
//   </copyright>
//  ----------------------------------------------------------------------- 

namespace Glasswall.Domain.Tenancy
{
    using Kernel.Data;

    public class User : BaseModel
    {
        public string TimeZone { get; set; }

        public string Locale { get; set; }
    }
}