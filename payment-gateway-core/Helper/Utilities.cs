using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace payment_gateway_core.Helper
{
    public static class Utilities
    {
        public static IEnumerable<KeyValuePair<string, string>> GetPropertiesAndValues<T>(this T obj) where T : class, new()
            => typeof(T).GetProperties().Select(obj.GetPropertyValue);

        private static KeyValuePair<string, string> GetPropertyValue<T>(this T obj, PropertyInfo property) where T : class, new()
        {
            var propName = property.Name.Split('.');
            return propName.Count() > 1
                ? GetPropertyValue(obj, property)
                : new KeyValuePair<string, string>(property.Name, property.GetValue(obj)?.ToString());
        }

        public static IEnumerable<KeyValuePair<string, string>> FilteredProperties<T>(this T model, ICollection<string> excluded = null) where T : class, new()
        {
            var properties = model.GetPropertiesAndValues();

            if (excluded == null || !excluded.Any()) return properties;

            bool Predicate(KeyValuePair<string, string> x) => !excluded.Contains(x.Key);
            return properties.Where(Predicate);
        }

        public static bool LuhnCheck(string digits)
        {
            return digits.All(char.IsDigit) && digits.Reverse()
                       .Select(c => c - 48)
                       .Select((thisNum, i) => i % 2 == 0
                           ? thisNum
                           : ((thisNum *= 2) > 9 ? thisNum - 9 : thisNum)
                       ).Sum() % 10 == 0;
        }

        public static string MaskStringValue(this string value, int numLast)
        {
            if (value.Length < numLast) 
                return new string('*', value.Length);

            var lng = value.Length;
            var show = value.Substring(lng - numLast, numLast);
            var rest = lng - show.Length;
            return new string('*', rest) + show;
        }
    }
}
