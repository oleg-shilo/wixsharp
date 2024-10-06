// Ignore Spelling: Deconstruct

using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security;
using System.Xml.Linq;

namespace WixSharp
{
    /// <summary>
    /// Serializes CLR entity into XML, based on the type members being marked for the serialization
    /// with <see cref="WixSharp.XmlAttribute"/>.
    /// <code>
    ///public class RemoveFolderEx : WixEntity, IGenericEntity
    ///{
    ///[Xml]
    ///public InstallEvent? On;
    ///
    ///[Xml]
    ///public string Property;
    ///
    ///[Xml]
    ///public string Id;
    ///}
    /// </code>
    /// </summary>
    public static class XmlMapping
    {
        /// <summary>
        /// Serializes the <see cref="WixSharp.WixObject"/> into XML based on the members marked
        /// with <see cref="WixSharp.XmlAttribute"/> and <see cref="WixSharp.WixObject.Attributes"/>.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        internal static XElement ToXElement(this WixObject obj)
        {
            var root = new XElement(obj.GetType().Name);

            root.AddAttributes(obj.Attributes).Add(obj.MapToXmlAttributes());
            root.Add(obj.MapToXmlCData());

            return root;
        }

        /// <summary>
        /// Serializes the <see cref="WixSharp.WixObject"/> into XML based on the members marked
        /// with <see cref="WixSharp.XmlAttribute"/> and <see cref="WixSharp.WixObject.Attributes"/>.
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="elementName"></param>
        /// <returns></returns>
        public static XElement ToXElement(this WixObject obj, XName elementName)
        {
            var root = new XElement(elementName);

            root.AddAttributes(obj.Attributes).Add(obj.MapToXmlAttributes());
            root.Add(obj.MapToXmlCData());

            return root;
        }

        /// <summary>
        /// Serializes the <see cref="WixSharp.WixObject"/> into XML based on the members marked
        /// with <see cref="WixSharp.XmlAttribute"/> and <see cref="WixSharp.WixObject.Attributes"/>.
        /// </summary>
        /// <param name="obj">The obj.</param>
        /// <param name="extension">The extension.</param>
        /// <returns></returns>
        public static XElement ToXElement(this WixObject obj, WixExtension extension)
        {
            var root = new XElement(extension.ToXName(obj.GetType().Name));

            root.AddAttributes(obj.Attributes).Add(obj.MapToXmlAttributes());
            root.Add(obj.MapToXmlCData());

            return root;
        }

        /// <summary>
        /// Serializes the <see cref="WixSharp.WixObject"/> into XML based on the members marked
        /// with <see cref="WixSharp.XmlAttribute"/> and <see cref="WixSharp.WixObject.Attributes"/>.
        /// </summary>
        /// <param name="obj">The obj.</param>
        /// <param name="extension">The extension.</param>
        /// <param name="elementName">Name of the element.</param>
        /// <returns></returns>
        public static XElement ToXElement(this WixObject obj, WixExtension extension, string elementName)
        {
            var root = new XElement(extension.ToXName(elementName));

            root.AddAttributes(obj.Attributes).Add(obj.MapToXmlAttributes());
            root.Add(obj.MapToXmlCData());

            return root;
        }

        /// <summary>
        /// Serializes the object into XML based on the members marked with <see cref="WixSharp.XmlAttribute"/>.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static XAttribute[] MapToXmlAttributes(this object obj)
        {
            var emptyArgs = new object[0];

            var result = new List<XAttribute>();

            var items = GetMemberInfo(obj)
                                      .Select(x =>
                                      {
                                          var xmlAttr = (XmlAttribute)x.GetCustomAttributes(typeof(XmlAttribute), false)
                                                                       .FirstOrDefault();

                                          string @namespace = null;
                                          if (xmlAttr != null)
                                              @namespace = xmlAttr.Namespace;

                                          bool IsCData = false;
                                          string name = null;
                                          if (xmlAttr != null)
                                          {
                                              name = xmlAttr.Name ?? x.Name;
                                              IsCData = xmlAttr.IsCData;
                                          }

                                          object value = null;
                                          if (!IsCData)
                                          {
                                              switch (x)
                                              {
                                                  case FieldInfo fieldInfo:
                                                      value = fieldInfo.GetValue(obj);
                                                      break;

                                                  case PropertyInfo propertyInfo:
                                                      value = propertyInfo.GetValue(obj, emptyArgs);
                                                      break;
                                              }
                                          }

                                          return new
                                          {
                                              Name = name,
                                              Value = value,
                                              Namespace = @namespace
                                          };
                                      })
                                      .Where(x => x.Name != null && x.Value != null)
                                      .ToArray();

            foreach (var item in items)
            {
                string xmlValue = item.Value.ToString();

                if (item.Value is bool?)
                {
                    var boolVal = (item.Value as bool?);
                    if (!boolVal.HasValue)
                        continue;
                    else
                        xmlValue = boolVal.Value.ToYesNo();
                }
                else if (item.Value is bool boolean)
                {
                    xmlValue = boolean.ToYesNo();
                }

                XNamespace ns = item.Namespace ?? "";
                result.Add(new XAttribute(ns + item.Name, xmlValue));
            }

            return result.ToArray();
        }

        static XCData MapToXmlCData(this object obj)
        {
            var emptyArgs = new object[0];

            XCData result = null;

            var items = GetMemberInfo(obj)
                .Select(x =>
                {
                    var xmlAttr = (XmlAttribute)x.GetCustomAttributes(typeof(XmlAttribute), false).FirstOrDefault();

                    bool IsCData = false;
                    if (xmlAttr != null)
                        IsCData = xmlAttr.IsCData;

                    object value = null;

                    if (IsCData)
                    {
                        switch (x)
                        {
                            case FieldInfo fieldInfo:
                                value = fieldInfo.GetValue(obj);
                                break;

                            case PropertyInfo propertyInfo:
                                value = propertyInfo.GetValue(obj, emptyArgs);
                                break;
                        }
                    }

                    return new
                    {
                        Value = value
                    };
                }).Where(x => x.Value != null);

            foreach (var item in items)
            {
                string xmlValue = item.Value.ToString();

                result = new XCData(xmlValue);
            }

            return result;
        }

        static IEnumerable<MemberInfo> GetMemberInfo(object obj)
        {
            // BindingFlags.NonPublic is needed to cover "internal" but not necessarily "private"
            var fields = obj.GetType()
                .GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
                .Cast<MemberInfo>()
                .ToArray();

            var props = obj.GetType()
                .GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
                .Where(x => x.CanRead)
                .Cast<MemberInfo>()
                .ToArray();

            return props.Concat(fields);
        }

        /// <summary>
        /// Converts source string to SecureString
        /// </summary>
        /// <param name="source">Insecure string</param>
        /// <returns>Secure version of the source string</returns>
        public static SecureString ToSecureString(this string source)
        {
            if (string.IsNullOrEmpty(source))
                return null;

            var result = new SecureString();
            foreach (char c in source)
                result.AppendChar(c);

            result.MakeReadOnly();

            return result;
        }

        /// <summary>
        /// Converts secure version of the string to insecure string
        /// </summary>
        /// <param name="input">Secure string</param>
        /// <returns>Insecure version of the SecureString</returns>
        public static string ToInsecureString(this SecureString input)
        {
            IntPtr bstr = Marshal.SecureStringToBSTR(input);
            try
            {
                return Marshal.PtrToStringBSTR(bstr);
            }
            finally
            {
                Marshal.FreeBSTR(bstr);
            }
        }
    }
}