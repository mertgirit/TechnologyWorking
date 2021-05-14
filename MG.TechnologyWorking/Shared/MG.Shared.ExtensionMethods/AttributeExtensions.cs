using System;
using System.Linq;

namespace MG.Shared.ExtensionMethods
{
    public static class AttributeExtensions
    {
        public static TValue GetAttributeValue<TAttribute, TValue>(this Type type, Func<TAttribute, TValue> valueSelector)
            where TAttribute : Attribute
        {
            var attribute = type.GetCustomAttributes(typeof(TAttribute), true).FirstOrDefault() as TAttribute;

            if (attribute != null)
            {
                return valueSelector(attribute);
            }
            return default(TValue);
        }
    }
}