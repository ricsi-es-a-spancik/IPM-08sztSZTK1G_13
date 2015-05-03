using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ELTE.IssueR.Models
{
    public static class Utility
    {
        /// <summary>
        /// Generates a selection list which contains all enum values of the given enum type.
        /// </summary>
        /// <typeparam name="TEnum">Enum type.</typeparam>
        /// <param name="selectedValue">The enum value which will be marked as the active element of the selection list.</param>
        /// <returns></returns>
        public static IEnumerable<SelectListItem> GetEnumSelectionList<TEnum>() where TEnum : struct, IConvertible
        {
            if (!typeof(TEnum).IsEnum)
            {
                throw new ArgumentException("T must be an enumerated type");
            }

            IEnumerable<TEnum> values = Enum.GetValues(typeof(TEnum)).Cast<TEnum>();
            if(!values.Any())
            {
                throw new ArgumentException("No enum value exists of type T.");
            }

            TEnum selectedValue = values.First();

            IEnumerable<SelectListItem> items =
                from value in values
                select new SelectListItem
                {
                    Text = value.ToString().Replace('_', ' '),
                    Value = value.ToString(),
                    Selected = value.Equals(selectedValue)
                };

            return items;
        }

        public static string GetEnumValueAsString<TEnum>(TEnum value)
        {
            return value.ToString().Replace('_', ' ');
        }
    }
}