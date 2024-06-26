// Ignore Spelling: Deconstruct

using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace WixSharp
{
    public static class AttachedProperies
    {
        public static ConditionalWeakTable<object,
            Dictionary<string, object>> ObjectCache = new ConditionalWeakTable<object,
            Dictionary<string, object>>();

        public static void SetAttachedValue<T>(this T obj, string name, object value) where T : class
        {
            Dictionary<string, object> properties = ObjectCache.GetOrCreateValue(obj);

            if (properties.ContainsKey(name))
                properties[name] = value;
            else
                properties.Add(name, value);
        }

        public static T GetAttachedValue<T>(this object obj, string name)
        {
            Dictionary<string, object> properties;
            if (ObjectCache.TryGetValue(obj, out properties) && properties.ContainsKey(name))
                return (T)properties[name];
            else
                return default(T);
        }

        public static object GetAttachedValue(this object obj, string name)
        {
            return obj.GetAttachedValue<object>(name);
        }
    }
}