using System;
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using TempDBGenerator.Seeding;

namespace TempDBGenerator.Helpers
{
    internal class PermissionHelper
    {
        public class PermissionAttributes
        {
            public string Name { get; set; }
            public string DisplayName { get; set; }
            public int Value { get; set; }
        }

        public static IEnumerable<PermissionAttributes> GetPermissionAttributes()
        {
            //var attributes = new List<PermissionAttributes>();
            var pinfo = typeof(Permissions).GetFields();
            foreach (var f in pinfo)
            {
                if (!f.IsStatic)
                    continue;
                var description = f.GetCustomAttributes(typeof(DescriptionAttribute), false).Cast<DescriptionAttribute>().FirstOrDefault();
                var attribute = new PermissionAttributes { Name = f.Name, Value = (int)f.GetValue(null) };
                //attributes.Add(attribute);
                if (description != null)
                    attribute.DisplayName = description.Description;
                yield return attribute;
            }
        }
    }
}
