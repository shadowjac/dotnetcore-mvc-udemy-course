using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GraniteHouse.Extensions
{
    public static class IEnumerableExtensions
    {
        public static IEnumerable<SelectListItem> ToSelectListItem<T>(this IEnumerable<T> data, int selectedValue)
        {
            return data.Select(p => new SelectListItem
            {
                Text = p.GetPropertyValue("Name"),
                Value = p.GetPropertyValue("Id"),
                Selected  = p.GetPropertyValue("Id").Equals(selectedValue.ToString())
            });
        }
    }
}
