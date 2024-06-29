// Ignore Spelling: Deconstruct

using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace WixSharp
{
    /// <summary>
    /// This class allows attaching arbitrary data to any object. This behavior resembles
    /// AttachedProperty in WPF.
    /// </summary>
    public static class AttachedProperties
    {
        /// <summary>
        /// The object cache. Contains object that have values attached.
        /// </summary>
        public static ConditionalWeakTable<object, Dictionary<string, object>> ObjectCache = new ConditionalWeakTable<object,
            Dictionary<string, object>>();

        /// <summary>
        /// Sets the attached value.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj">The object.</param>
        /// <param name="name">The name.</param>
        /// <param name="value">The value.</param>
        public static void SetAttachedValue<T>(this T obj, string name, object value) where T : class
        {
            Dictionary<string, object> properties = ObjectCache.GetOrCreateValue(obj);

            if (properties.ContainsKey(name))
                properties[name] = value;
            else
                properties.Add(name, value);
        }

        /// <summary>
        /// Gets the attached value.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj">The object.</param>
        /// <param name="name">The name.</param>
        /// <returns></returns>
        public static T GetAttachedValue<T>(this object obj, string name)
        {
            Dictionary<string, object> properties;
            if (ObjectCache.TryGetValue(obj, out properties) && properties.ContainsKey(name))
                return (T)properties[name];
            else
                return default(T);
        }

        /// <summary>
        /// Gets the attached value.
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <param name="name">The name.</param>
        /// <returns></returns>
        public static object GetAttachedValue(this object obj, string name)
        {
            return obj.GetAttachedValue<object>(name);
        }
    }
}