// Ignore Spelling: Deconstruct

using System;
using System.Collections;
using System.Collections.Generic;
using static System.Collections.Specialized.BitVector32;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Diagnostics.Tracing;
using System.Drawing;
using System.Drawing.Imaging;
using static System.Environment;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security;
using System.Security.Principal;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using System.Xml.Linq;
using System.Xml.Serialization;
using Microsoft.Win32;
using WixSharp.CommonTasks;
using static WixSharp.SetupEventArgs;
using WixToolset.Dtf.WindowsInstaller;

using IO = System.IO;

#pragma warning disable CA1416 // Validate platform compatibility

namespace WixSharp
{
    /// <summary>
    /// </summary>
    public static partial class Extensions
    {
        /// <summary>
        /// Sets the parent component attribute 'Permanent'.
        /// </summary>
        /// <typeparam name="T">The type of the T.</typeparam>
        /// <param name="obj">The obj.</param>
        /// <param name="isPermanent">if set to <c>true</c> [is permanent].</param>
        /// <returns></returns>
        public static T SetComponentPermanent<T>(this T obj, bool isPermanent) where T : WixEntity
        {
            //While it is tempting to move the implementation to WixEntity the extension method gives a
            //better support for Fluent API as it returns not the base but the actual type.
            obj.SetAttributeDefinition("Component:Permanent", isPermanent.ToYesNo());
            return obj;
        }

        /// <summary>
        /// Determines whether the current user is administrator.
        /// </summary>
        /// <param name="identity">The identity.</param>
        /// <returns></returns>
        static public bool IsAdmin(this WindowsIdentity identity)
        {
            var p = new WindowsPrincipal(identity);
            return p.IsInRole(WindowsBuiltInRole.Administrator);
        }

        static internal IEnumerable<TSource> DistinctBy<TSource, TKey>(this IEnumerable<TSource> items, Func<TSource, TKey> keySelector)
        {
            return items.GroupBy(keySelector).Select(g => g.First());
        }

        static internal List<T> FilterDuplicates<T>(this List<T> items)
        {
            var distinctItems = items.Distinct().ToArray();
            items.Clear();
            items.AddRange(distinctItems);
            return items;
        }

        static internal IEnumerable<T> AllChildren<T>(this T node, Func<T, IEnumerable<T>> getChildren)
        {
            return new T[] { node }.AllChildren(getChildren);
        }

        static internal IEnumerable<T> AllChildren<T>(this IEnumerable<T> collection, Func<T, IEnumerable<T>> getChildren)
        {
            int iterator = 0;
            var result = new List<T>();

            result.AddRange(collection);

            while (iterator < result.Count)
            {
                var children = getChildren(result[iterator]);
                result.AddRange(children);
                iterator++;
            }

            return result.ToArray();
        }

        /// <summary>
        /// Converts string value of a version into the <see cref="System.Version"/> object.
        /// <para>
        /// This method handles alpha-numeric strings. For example "v1.2.3-HotFix" is converted in
        /// "1.2.3" <see cref="System.Version"/> object.
        /// </para>
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static Version ToRawVersion(this string obj)
        {
            try
            {
                var version_digits = new string(obj.Where(x => char.IsDigit(x) || x == '.').ToArray());
                return new Version(version_digits);
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Checks if the type implements the interface .
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="type">The type.</param>
        /// <returns></returns>
        public static bool Implements<T>(this Type type)
            => type.GetInterfaces().Contains(typeof(T));

        internal static Type RootDeclaringType(this Type type)
        {
            var retval = type;
            while (retval.DeclaringType != null)
                retval = retval.DeclaringType;
            return retval;
        }

        internal static bool Implements(this Type type, string interfaceName)
            => type.GetInterfaces().Any(i => i.FullName == interfaceName);

        /// <summary>
        /// Gets the base name of the generic type (e.g. "System.Geneics.List" for type "System.Geneics.List&lt;T&gt;").
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns></returns>
        public static string GetGenericTypeBaseName(this Type type)
        {
            if (type.IsGenericType)
                return type.FullName.Split('`').FirstOrDefault();
            return null;
        }

        /// <summary>
        /// Invokes the method ob the object with the specified name.
        /// </summary>
        /// <param name="object">The object.</param>
        /// <param name="name">The name.</param>
        /// <param name="args">The arguments.</param>
        /// <returns></returns>
        public static object Invoke(this object @object, string name, params object[] args)
            => @object.GetType().GetMethod(name).Invoke(@object, args);

        /// <summary>
        /// Adds the element to a given XML element. It is a Fluent version of <see cref="T:System.Xml.Linq.XElement.Add"/>.
        /// </summary>
        /// <param name="obj">The instance of the <see cref="T:System.Xml.Linq.XElement"/>.</param>
        /// <param name="element">Element to add.</param>
        /// <returns>Added <see cref="T:System.Xml.Linq.XElement"/>.</returns>
        public static XElement AddElement(this XElement obj, XElement element)
        {
            obj.Add(element);
            return element;
        }

        /// <summary>
        /// Returns LocalName.
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <returns></returns>
        public static string LocalName(this XElement obj) => obj.Name.LocalName;

        /// <summary>
        /// Returns LocalName.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static string LocalName(this XAttribute obj) => obj.Name.LocalName;

        internal static AssemblyName[] GetWixSharpDependencies(this System.Reflection.Assembly asm)
            => asm.GetReferencedAssemblies()
               .Where(a => a.Name.StartsWith("WixSharp.") ||
                           a.Name.StartsWith("Caliburn.") ||
                           a.Name.StartsWith("System.Windows.Interactivity")).ToArray();

        /// <summary>
        /// Adds the element to a given XML element. It is a Fluent version of <see cref="T:System.Xml.Linq.XElement.Add"/>.
        /// <para>
        /// <c>elementName</c> can be either the name of the element to be added or the sequence of
        /// the elements specified by path (e.g. <c>AddElement("Product/Package")</c>).
        /// </para>
        /// </summary>
        /// <param name="obj">The instance of the <see cref="T:System.Xml.Linq.XElement"/>.</param>
        /// <param name="elementName">Element to add.</param>
        /// <returns>Added <see cref="T:System.Xml.Linq.XElement"/>.</returns>
        public static XElement AddElement(this XElement obj, string elementName)
        {
            XNamespace docNs = null;

            if (obj.Document != null)
                docNs = obj.Document.Root.GetDefaultNamespace();

            var ns = obj.GetDefaultNamespace();
            var parent = obj;
            foreach (var item in elementName.Split('/'))
                if (ns != null && ns != docNs)
                    parent = parent.AddElement(new XElement(ns + item));
                else
                    parent = parent.AddElement(new XElement(item));
            return parent;
        }

        /// <summary>
        /// Adds the element to a given XML element. It is a Fluent version of <see cref="T:System.Xml.Linq.XElement.Add"/>.
        /// </summary>
        /// <param name="obj">The instance of the <see cref="T:System.Xml.Linq.XElement"/>.</param>
        /// <param name="elementName">Name of the element.</param>
        /// <param name="attributesDefinition">
        /// The attributes definition. Rules of the composing the definition are the same as for
        /// <see cref="P:WixSharp.WixEntity.AttributesDefinition"/>.
        /// </param>
        /// <param name="value">The value of the added element.</param>
        /// <returns>Added <see cref="T:System.Xml.Linq.XElement"/>.</returns>
        public static XElement AddElement(this XElement obj, XName elementName, string attributesDefinition = null, string value = null)
        {
            var parent = obj;
            XElement element;
            parent = parent.AddElement(element = new XElement(elementName)).AddAttributes(attributesDefinition);
            if (value != null)
                element.Value = value;
            return parent;
        }

        /// <summary>
        /// Adds the element to a given XML element and sets the attributes of the newly created element.
        /// <para>
        /// <c>elementName</c> can be either the name of the element to be added or the sequence of
        /// the elements specified by path (e.g. <c>AddElement("Product/Package")</c>).
        /// </para>
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <param name="elementName">The element.</param>
        /// <param name="attributesDefinition">
        /// The attributes definition. Rules of the composing the definition are the same as for
        /// <see cref="P:WixSharp.WixEntity.AttributesDefinition"/>.
        /// </param>
        /// <param name="value">The value of the added element.</param>
        /// <returns></returns>
        public static XElement AddElement(this XElement obj, string elementName, string attributesDefinition, string value = null)
        {
            var result = obj.AddElement(elementName).AddAttributes(attributesDefinition);
            if (value != null)
                result.Value = value;
            return result;
        }

        /// <summary>
        /// Sets the value of the first child element (with 'elementName'). If the element s not
        /// found then it is created.
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <param name="elementName">Name of the element.</param>
        /// <param name="attributesDefinition">The attributes definition.</param>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        public static XElement SetElementValue(this XElement obj, string elementName, string attributesDefinition, string value)
        {
            var result = obj.Select(elementName);

            if (result == null)
                result = obj.AddElement(elementName);

            result.AddAttributes(attributesDefinition);

            if (value != null)
                result.Value = value;
            return result;
        }

        /// <summary>
        /// Determines whether the string is an absolute path.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <returns><c>true</c> if it is an absolute path; otherwise, <c>false</c>.</returns>
        public static bool IsAbsolutePath(this string path)
        {
            return IO.Path.IsPathRooted(path);
        }

        internal static bool IsPublicWixProperty(this string name)
            => name == name.ToUpper();

        /// <summary>
        /// Determines whether the character is a digit.
        /// </summary>
        /// <param name="c">The c.</param>
        /// <returns><c>true</c> if the specified c is digit; otherwise, <c>false</c>.</returns>
        public static bool IsDigit(this char c)
        {
            return char.IsDigit(c);
        }

        /// <summary>
        /// Gets the index of an item from the collection.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="collection">The collection.</param>
        /// <param name="item">The item.</param>
        /// <returns></returns>
        public static int FindIndex<T>(this IEnumerable<T> collection, T item)
        {
            return collection.ToArray().FindIndex(x => x.Equals(item));
        }

        /// <summary>
        /// Merges a dictionary into this one. Replacing values that are present in both
        /// and adding values that are not.
        /// </summary>
        /// <param name="a">Merge target</param>
        /// <param name="b">Merge source</param>
        public static void Merge(this Dictionary<string, string> a, Dictionary<string, string> b)
        {
            foreach (var kvp in b)
            {
                a.Set(kvp.Key, kvp.Value);
            }
        }

        /// <summary>
        /// Converts key/value map into the dictionary. The map entry format is as follows: &lt;key&gt;=&lt;value&gt;[;&lt;key&gt;=&lt;value&gt;].
        /// </summary>
        /// <param name="map">The map.</param>
        /// <param name="itemDelimiter">The item delimiter.</param>
        /// <param name="valueDelimiter">The value delimiter.</param>
        /// <returns></returns>
        /// <exception cref="System.Exception">Invalid map entry</exception>
        public static Dictionary<string, string> ToDictionary(this string map, char itemDelimiter = ';', char valueDelimiter = '=')
        {
            var retval = new Dictionary<string, string>();
            if (!map.IsEmpty())
            {
                foreach (string pair in map.Trim().Split(new[] { itemDelimiter }, StringSplitOptions.RemoveEmptyEntries))
                {
                    if (pair.IsNotEmpty())
                        try
                        {
                            string[] tokens = pair.Split(new[] { valueDelimiter }, 2, StringSplitOptions.RemoveEmptyEntries);
                            string name = tokens[0].Trim();

                            string value = "";
                            if (tokens.Count() > 1)
                                value = tokens[1].Trim();

                            retval[name] = value;
                        }
                        catch (Exception e)
                        {
                            throw new Exception("Invalid map entry", e);
                        }
                }
            }
            return retval;
        }

        /// <summary>
        /// Adds the element to a given XML element. It is a Fluent version of <see cref="T:System.Xml.Linq.XElement.Add"/>.
        /// </summary>
        /// <param name="obj">The instance of the <see cref="T:System.Xml.Linq.XElement"/>.</param>
        /// <param name="element">Element to add.</param>
        /// <param name="attributes">The collection of Name/Value attributes to add.</param>
        /// <returns>Added <see cref="T:System.Xml.Linq.XElement"/>.</returns>
        public static XElement AddElement(this XElement obj, XElement element, Dictionary<string, string> attributes)
        {
            obj.Add(element.AddAttributes(attributes));
            return element;
        }

        /// <summary>
        /// Adds/sets the attributes to the to a given XML element ( <see cref="T:System.Xml.Linq.XElement"/>).
        /// <para>It is a renamed version of <see cref="Extensions.SetAttributes"/></para>
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <param name="attributesDefinition">
        /// The attributes definition. Rules of the composing the definition are the same as for
        /// <see cref="WixObject.AttributesDefinition"/>.
        /// </param>
        /// <returns></returns>
        public static XElement AddAttributes(this XElement obj, string attributesDefinition)
        {
            return obj.AddAttributes(attributesDefinition?.ToDictionary());
        }

        /// <summary>
        /// Returns the value of teh element attributes with the specified name.
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <param name="name">The name.</param>
        /// <returns></returns>
        public static string Attr(this XElement obj, XName name)
        {
            return obj.Attribute(name)?.Value;
        }

        /// <summary>
        /// Sets/adds the attributes to the given XML element ( <see cref="T:System.Xml.Linq.XElement"/>).
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <param name="attributesDefinition">
        /// The attributes definition. Rules of the composing the definition are the same as for
        /// <see cref="WixObject.AttributesDefinition"/>.
        /// </param>
        /// <returns></returns>
        public static XElement SetAttributes(this XElement obj, string attributesDefinition)
        {
            return obj.AddAttributes(attributesDefinition.ToDictionary());
        }

        /// <summary>
        /// Gets the attribute of the given XML element ( <see cref="T:System.Xml.Linq.XElement"/>).
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <param name="name">The name.</param>
        /// <param name="defaultValue">The default value to return if the attribute is not present.</param>
        /// <returns></returns>
        public static string GetAttribute(this XElement obj, XName name, string defaultValue = null)
        {
            return obj.Attribute(name)?.Value ?? defaultValue;
        }

        /// <summary>
        /// Converts AttributesDefinition into XAttribute array. Rules of the composing the
        /// definition are the same as for <see cref="WixObject.AttributesDefinition"/>
        /// </summary>
        /// <param name="attributesDefinition">The attributes definition.</param>
        public static XAttribute[] ToXAttributes(this string attributesDefinition)
        {
            return attributesDefinition.ToDictionary().Select(x => new XAttribute(x.Key, x.Value)).ToArray();
        }

        internal static XElement SetAttributeFromFieldsOf(this XElement obj, object src)
        {
            src.GetType()
               .GetFields()
               .ForEach(f => obj.SetAttribute(f.Name, f.GetValue(src)));
            return obj;
        }

        /// <summary>
        /// Sets the attribute.
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <param name="name">The name.</param>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        public static XElement SetAttribute(this XElement obj, XName name, object value)
        {
            if (value is string && (value as string).IsEmpty())
            {
                obj.SetAttributeValue(name, null);
            }
            else if (value is bool?)
            {
                var attrValue = (bool?)value;
                obj.SetAttributeValue(name, attrValue.ToNullOrYesNo());
            }
            else if (value is bool)
            {
                var attrValue = (bool)value;
                obj.SetAttributeValue(name, attrValue.ToYesNo());
            }
            else
            {
                obj.SetAttributeValue(name, value);
            }
            return obj;
        }

        /// <summary>
        /// Sets the value of the attribute. This is a fluent version of XElement.SetAttributeValue.
        /// <para>Note <c>name</c> can include xml namespace prefix:
        /// <code>
        ///element.SetAttribute("{dep}ProviderKey", "01234567-8901-2345-6789-012345678901");
        /// </code>
        /// Though in this case the required namespace must be already added to the element/document.
        /// </para>
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <param name="name">The name.</param>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        public static XElement SetAttribute(this XElement obj, string name, object value)
        {
            XName x_name = name;

            if (name.StartsWith("{"))
            {
                var tokens = name.Substring(1).Split(new[] { '}' }, 2);
                var xml_namespace = tokens.First();
                var prefix = obj.GetNamespaceOfPrefix(xml_namespace);
                if (prefix != null)
                    x_name = obj.GetNamespaceOfPrefix(xml_namespace) + tokens.Last();
            }

            return SetAttribute(obj, x_name, value);
        }

        /// <summary>
        /// Sets the value of the <see cref="System.Xml.Linq.XElement"/> object.
        /// </summary>
        /// <param name="obj">The object <see cref="System.Xml.Linq.XElement"/>.</param>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        public static XElement SetValue(this XElement obj, object value)
        {
            obj.Value = value?.ToString();
            return obj;
        }

        /// <summary>
        /// Sets the value of the attribute. This is a fluent version of XElement.SetAttributeValue
        /// that takes the Name/Value string definition as a single input parameter.
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <param name="nameValuePair">
        /// The attribute name/value pair of the "[name]=[value]" format (e.g. ""Version=!(bind.FileVersion.Utils.dll)").
        /// </param>
        /// <returns></returns>
        public static XElement SetAttribute(this XElement obj, string nameValuePair)
        {
            var pair = nameValuePair.ToDictionary().FirstOrDefault();

            if (pair.Value is string && pair.Value.IsEmpty())
                obj.SetAttributeValue(pair.Key, null);
            else
                obj.SetAttributeValue(pair.Key, pair.Value);
            return obj;
        }

        /// <summary>
        /// Adds the attributes to the to a given XML element ( <see cref="T:System.Xml.Linq.XElement"/>).
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <param name="attributes">The attributes.</param>
        /// <returns></returns>
        public static XElement AddAttributes(this XElement obj, IEnumerable<XAttribute> attributes)
        {
            foreach (var item in attributes)
                obj.Add(item);
            return obj;
        }

        /// <summary>
        /// Adds the attributes to the to a given XML element ( <see cref="T:System.Xml.Linq.XElement"/>).
        /// </summary>
        /// <param name="obj">The instance of the <see cref="T:System.Xml.Linq.XElement"/>.</param>
        /// <param name="attributes">The collection of Name/Value attributes to add.</param>
        /// <returns><see cref="T:System.Xml.Linq.XElement"/> with added attributes.</returns>
        public static XElement AddAttributes(this XElement obj, Dictionary<string, string> attributes)
        {
            if (attributes != null && attributes.Any())
            {
                // "Component:ProviderKey=12345 {dep}ProductKey=12345 Component:{dep}ProductKey=12345
                var optimizedAttributes = attributes.Where(x => !x.Key.Contains(":") &&
                                                                !x.Key.StartsWith("{"));

                var optimizedAttributesMap = optimizedAttributes.ToDictionary(t => t.Key, t => t.Value);

                var compositValues = string.Join(";", attributes.Except(optimizedAttributes).Select(x => x.Key + "=" + x.Value).ToArray());
                if (compositValues.IsNotEmpty())
                    optimizedAttributesMap.Add("WixSharpCustomAttributes", compositValues);

                foreach (var key in optimizedAttributesMap.Keys)
                    obj.SetAttributeValue(key, optimizedAttributesMap[key]);
            }
            return obj;
        }

        /// <summary>
        /// Determines whether the specified <see cref="T:System.Xml.Linq.XElement"/> has attribute.
        /// </summary>
        /// <param name="obj">The obj.</param>
        /// <param name="name">The name.</param>
        /// <returns>
        /// <c>true</c> if the specified <see cref="T:System.Xml.Linq.XElement"/> has attribute;
        /// otherwise, <c>false</c>.
        /// </returns>
        public static bool HasAttribute(this XElement obj, string name)
        {
            return obj.Attribute(name) != null;
        }

        /// <summary>
        /// Determines whether the specified <see cref="T:System.Xml.Linq.XElement"/> has attribute
        /// and the attribute value passes the test by <c>attributeValueSelector</c>.
        /// </summary>
        /// <param name="obj">The obj.</param>
        /// <param name="name">The name.</param>
        /// <param name="attributeValueSelector">
        /// The attribute value selector. Allows testing the attribute value.
        /// </param>
        /// <returns>
        /// <c>true</c> if the specified <see cref="T:System.Xml.Linq.XElement"/> has attribute;
        /// otherwise, <c>false</c>.
        /// </returns>
        public static bool HasAttribute(this XElement obj, string name, Predicate<string> attributeValueSelector)
        {
            return obj.Attribute(name) != null && attributeValueSelector(obj.Attribute(name).Value);
        }

        /// <summary>
        /// Determines whether the specified <see cref="T:System.Xml.Linq.XElement"/> has attribute
        /// and the specific attribute value.
        /// </summary>
        /// <param name="obj">The obj.</param>
        /// <param name="name">The name.</param>
        /// <param name="attributeValue">The attribute value.</param>
        /// <returns>
        /// <c>true</c> if the specified <see cref="T:System.Xml.Linq.XElement"/> has attribute;
        /// otherwise, <c>false</c>.
        /// </returns>
        public static bool HasAttribute(this XElement obj, string name, string attributeValue)
        {
            return obj.Attribute(name) != null && obj.Attribute(name).Value == attributeValue;
        }

        /// <summary>
        /// Search for the first parent element (in the "parents chain") with the specified name of
        /// the given XML element ( <see cref="T:System.Xml.Linq.XElement"/>).
        /// </summary>
        /// <param name="obj">The instance of the <see cref="T:System.Xml.Linq.XElement"/>.</param>
        /// <param name="parentName">Name of the parent element to search.</param>
        /// <returns><see cref="T:System.Xml.Linq.XElement"/> with the matching name.</returns>
        public static XElement Parent(this XElement obj, string parentName)
        {
            XElement element = obj.Parent;
            do
            {
                if (element.Name.LocalName == parentName)
                    return element;
                else
                    element = element.Parent;
            }
            while (element != null);

            return null;
        }

        /// <summary>
        /// Copies attribute value from one <see cref="T:System.Xml.Linq.XElement"/> to another. If
        /// the attribute already exists, its value is modified.
        /// </summary>
        /// <param name="dest">
        /// The instance of the <see cref="T:System.Xml.Linq.XElement"/> to copy the attribute to.
        /// </param>
        /// <param name="src">
        /// The instance of the <see cref="T:System.Xml.Linq.XElement"/> to copy the attribute from.
        /// </param>
        /// <param name="attributeName">Name of the source attribute to copy.</param>
        /// <returns>
        /// The instance of the <see cref="T:System.Xml.Linq.XElement"/> to copy the attribute to.
        /// </returns>
        public static XElement CopyAttributeFrom(this XElement dest, XElement src, string attributeName)
        {
            if (src.Attribute(attributeName) != null)
            {
                if (dest.Attribute(attributeName) != null)
                    dest.Attribute(attributeName).Value = src.Attribute(attributeName).Value;
                else
                    dest.Add(new XAttribute(attributeName, src.Attribute(attributeName).Value));
            }
            return dest;
        }

        /// <summary>
        /// Injects the Wxs (WiX source) into Wxs document. It merges 'Wix/Package' elements of
        /// document with 'Wix/Package' elements of wxsFile.
        /// <para>
        /// This method is nothing else but a 'syntactic sugar' method, which wraps the following code:
        /// <code>
        ///document.Root.Select(Compiler.ProductElementName)
        ///             .Add(XDocument.Load(wxsFile)
        ///             .Root.Select(Compiler.ProductElementName).Elements());
        /// </code>
        /// </para>
        /// <example>The following is an example of using InjectWxs.
        /// <code>
        /// Compiler.WixSourceGenerated +=
        /// document =&gt; document.InjectWxs("CommonProperies.wxs");
        ///
        /// //where CommonProperies.wxs contains the following XML
        /// &lt;?xml version="1.0" encoding="Windows-1252"?&gt;
        /// &lt;Wix xmlns = "http://schemas.microsoft.com/wix/2006/wi" &gt;
        ///   &lt;Product&gt;
        ///     &lt;Property Id="Prop1" Value="1" /&gt;
        ///     &lt;Property Id="Prop2" Value="2" /&gt;
        ///     &lt;Property Id="Prop3" Value="3" /&gt;
        ///     &lt;Property Id="Prop4" Value="4" /&gt;
        ///   &lt;/Product&gt;
        /// &lt;/Wix&gt;
        /// </code>
        /// </example>
        /// </summary>
        /// <param name="document">The document.</param>
        /// <param name="wxsFile">The WXS file.</param>
        /// <returns></returns>
        public static XDocument InjectWxs(this XDocument document, string wxsFile)
        {
            document.Root.Select(Compiler.ProductElementName)
                         .Add(XDocument.Load(wxsFile)
                                       .Root.Select(Compiler.ProductElementName).Elements());
            return document;
        }

        /// <summary>
        /// Reads the attribute value. Returns null if attribute doesn't exist.
        /// </summary>
        /// <param name="e">The e.</param>
        /// <param name="attributeName">Name of the attribute.</param>
        /// <returns></returns>
        public static string ReadAttribute(this XElement e, string attributeName)
        {
            if (e.Attribute(attributeName) != null)
                return e.Attribute(attributeName).Value;
            else
                return null;
        }

        internal static string Attr(this XElement e, string attributeName)
        {
            if (e.Attribute(attributeName) != null)
                return e.Attribute(attributeName).Value;
            else
                return null;
        }

        /// <summary>
        /// Selects distinct items from the collection.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list">The list.</param>
        /// <param name="keySelector">The key selector.</param>
        /// <returns></returns>
        public static IEnumerable<T> DistinctBy<T>(this IEnumerable<T> list, Func<T, object> keySelector)
        {
            return list.GroupBy(keySelector).Select(x => x.First());
        }

        /// <summary>
        /// A generic LINQ equivalent of C# foreach loop.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="collection">The collection.</param>
        /// <param name="action">The action.</param>
        /// <returns></returns>
        public static IEnumerable<T> ForEach<T>(this IEnumerable<T> collection, Action<T> action)
        {
            // clone the copy so the collection is not impacted by the action
            foreach (T item in collection.ToArray())
            {
                action(item);
            }
            return collection;
        }

        /// <summary>
        /// A generic LINQ equivalent of C# foreach loop.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="T1">The type of the 1.</typeparam>
        /// <param name="collection">The collection.</param>
        /// <param name="action">The action.</param>
        /// <returns></returns>
        public static IEnumerable<T> ForEach<T, T1>(this IEnumerable<T> collection, Func<T, T1> action)
        {
            foreach (T item in collection)
            {
                action(item);
            }
            return collection;
        }

        /// <summary>
        /// Returns all items from the collection except specified one.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="collection">The collection.</param>
        /// <param name="exceptItem">The except item.</param>
        /// <returns></returns>
        public static IEnumerable<T> Except<T>(this IEnumerable<T> collection, T exceptItem)
            => collection.Where(x => !x.Equals(exceptItem));

        /// <summary>
        /// Determines whether the input value is one of the specified values.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value">The value.</param>
        /// <param name="values">The values.</param>
        /// <returns><c>true</c> if [is one of] [the specified values]; otherwise, <c>false</c>.</returns>
        public static bool IsOneOf<T>(this T value, params T[] values)
        {
            return values.Any(x => value.Equals(x));
        }

        /// <summary>
        /// Enqueues the range of the items into a instance of the <see cref="T:System.Collections.Generics.Queue"/>.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="queue">The queue.</param>
        /// <param name="collection">The collection.</param>
        /// <returns></returns>
        public static Queue<T> EnqueueRange<T>(this Queue<T> queue, IEnumerable<T> collection)
        {
            foreach (T item in collection)
            {
                queue.Enqueue(item);
            }
            return queue;
        }

        /// <summary>
        /// Gets the combined hash code of all items in the collection. This method is convenient to
        /// use to verify that the collections have identical items.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="collection">The collection.</param>
        /// <returns></returns>
        public static int GetItemsHashCode<T>(this IEnumerable<T> collection) where T : class
        {
            var hash = new StringBuilder();
            foreach (T item in collection)
            {
                hash.Append((item == null ? "null".GetHashCode() : item.GetHashCode()).ToString());
            }
            return hash.ToString().GetHashCode();
        }

        /// <summary>
        /// Copies attribute value from one <see cref="T:System.Xml.Linq.XElement"/> to another. If
        /// the attribute already exists, its value is modified.
        /// </summary>
        /// <param name="dest">
        /// The instance of the <see cref="T:System.Xml.Linq.XElement"/> to copy the attribute to.
        /// </param>
        /// <param name="destAttributeName">Name of the destination attribute to copy.</param>
        /// <param name="src">
        /// The instance of the <see cref="T:System.Xml.Linq.XElement"/> to copy the attribute from.
        /// </param>
        /// <param name="srcAttributeName">Name of the source attribute to copy.</param>
        /// <returns>
        /// The instance of the <see cref="T:System.Xml.Linq.XElement"/> to copy the attribute to.
        /// </returns>
        public static XElement CopyAttributeFrom(this XElement dest, string destAttributeName, XElement src, string srcAttributeName)
        {
            if (src.Attribute(srcAttributeName) != null)
            {
                if (dest.Attribute(destAttributeName) != null)
                    dest.Attribute(destAttributeName).Value = src.Attribute(srcAttributeName).Value;
                else
                    dest.Add(new XAttribute(destAttributeName, src.Attribute(srcAttributeName).Value));
            }
            return dest;
        }

        /// <summary>
        /// Replaces all Wix# predefined string constants in the Wix# directory path with their WiX equivalents.
        /// <para>
        /// Processed string can be used as an Id for referencing from other Wix# components and
        /// setting the corresponding path from <c>MsiExec.exe</c> command line.
        /// </para>
        /// </summary>
        /// <param name="path">The Wix# directory path.</param>
        /// <returns>Replacement/conversion result.</returns>
        public static string ToDirID(this string path)
        {
            return path.Expand();
        }

        /// <summary>
        /// Safely converts string to int.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="defaultValue">The default value.</param>
        /// <returns></returns>
        public static int ToInt(this string value, int defaultValue = 0)
            => int.TryParse(value, out int result) ? result : defaultValue;

        /// <summary>
        /// Determines if the integer is an even value
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool IsEven(this int value)
        {
            return (value % 2) == 0;
        }

        /// <summary>
        /// Determines if the integer is an odd value
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool IsOdd(this int value)
        {
            return (value % 2) != 0;
        }

        /// <summary>
        /// Determines the <see cref="WixSharp.FeatureDisplay"/> from a given integer.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        public static FeatureDisplay MapToFeatureDisplay(this int value)
        {
            // https://msdn.microsoft.com/en-us/library/windows/desktop/aa368585%28v=vs.85%29.aspx?f=255&MSPPError=-2147217396
            return value == 0 ? FeatureDisplay.hidden :
                   value.IsEven() ? FeatureDisplay.collapse :
                   FeatureDisplay.expand;
        }

        /// <summary>
        /// Converts string to <see cref="WixSharp.Id"/>.
        /// </summary>
        /// <param name="obj">Id string value.</param>
        /// <returns></returns>
        public static Id ToId(this string obj)
        {
            return new Id(obj);
        }

        /// <summary>
        /// Returns Exception.ToString result containing no debug information.
        /// <para>
        /// Sanitizes the result before returning by removing any debug info (file locations) from
        /// the exception trace stack.
        /// </para>
        /// </summary>
        /// <param name="ex">The ex.</param>
        /// <returns></returns>
        public static string ToPublicString(this Exception ex)
        {
            var text = ex.ToString();
            var lines = text.Replace("\r", "")
                            .Split('\n')
                            .Select(l =>
                            {
                                var match = Regex.Match(l, "\\s.:\\\\.");
                                if (match.Success)
                                    return l.Substring(0, match.Index) + " <hidden content>";
                                else
                                    return l;
                            })
                            .ToArray();

            return string.Join(Environment.NewLine, lines);
        }

        /// <summary>
        /// Checks if the given text ends with any provided string patterns (suffixes).
        /// </summary>
        /// <param name="text">The text.</param>
        /// <param name="ignoreCase">if set to <c>true</c> [ignore case].</param>
        /// <param name="patterns">The patterns.</param>
        /// <returns></returns>
        public static bool EndsWithAny(this string text, bool ignoreCase, params string[] patterns)
            => patterns.Any(item => text.EndsWith(item, ignoreCase));

        /// <summary>
        /// Tests if the text ends with the specified pattern.
        /// </summary>
        /// <param name="text">The text to test.</param>
        /// <param name="pattern">The value.</param>
        /// <param name="ignoreCase">if set to <c>true</c> [ignore case].</param>
        /// <returns></returns>
        public static bool EndsWith(this string text, string pattern, bool ignoreCase)
        {
            if (ignoreCase)
                return text.EndsWith(pattern, StringComparison.CurrentCultureIgnoreCase);
            else
                return text.EndsWith(pattern);
        }

        /// <summary>
        /// Replaces double-quotation characters with single-quotation ones.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <returns></returns>
        public static string ToSingleQuots(this string text)
        {
            return text.Replace("\"", "'");
        }

        /// <summary>
        /// Tests if the text starts with the specified pattern.
        /// </summary>
        /// <param name="text">The text to test.</param>
        /// <param name="pattern">The value.</param>
        /// <param name="ignoreCase">if set to <c>true</c> [ignore case].</param>
        /// <returns></returns>
        public static bool StartsWith(this string text, string pattern, bool ignoreCase)
        {
            if (ignoreCase)
                return text.StartsWith(pattern, StringComparison.CurrentCultureIgnoreCase);
            else
                return text.StartsWith(pattern);
        }

        /// <summary>
        /// A simple generic wrapper around more specialized <see cref="T:String.Join"/>, which is
        /// limited to work with string arrays only.
        /// </summary>
        /// <param name="strings">The strings.</param>
        /// <param name="separator">The separator.</param>
        /// <param name="selector">
        /// A transform function to apply to each source element; the second parameter of the
        /// function represents the index of the source element.
        /// </param>
        /// <returns></returns>
        public static string JoinBy(this IEnumerable<string> strings, string separator, Func<string, string> selector = null)
        {
            if (selector != null)
                return string.Join(separator, strings.Select(selector).ToArray());
            else
                return string.Join(separator, strings.ToArray());
        }

        /// <summary>
        /// A simple generic wrapper around more specialized <see cref="T:String.Join"/>, which is
        /// limited to work with string arrays only.
        /// </summary>
        /// <param name="strings">The strings.</param>
        /// <param name="separator">The separator.</param>
        /// <param name="selector">
        /// A transform function to apply to each source element; the second parameter of the
        /// function represents the index of the source element.
        /// </param>
        /// <returns></returns>
        [Obsolete(message: "This method name is obsolete use `JoinBy` instead", error: false)]
        public static string Join(this IEnumerable<string> strings, string separator, Func<string, string> selector = null)
            => Join(strings, separator, selector);

        ///<summary>Finds the index of the first item matching an expression in an enumerable.</summary>
        ///<param name="items">The enumerable to search.</param>
        ///<param name="predicate">The expression to test the items against.</param>
        ///<returns>The index of the first matching item, or -1 if no items match.</returns>
        public static int FindIndex<T>(this IEnumerable<T> items, Func<T, bool> predicate)
        {
            if (items == null) throw new ArgumentNullException(nameof(items));
            if (predicate == null) throw new ArgumentNullException(nameof(predicate));

            int retVal = 0;
            foreach (var item in items)
            {
                if (predicate(item)) return retVal;
                retVal++;
            }
            return -1;
        }

        /// <summary>
        /// Safely converts string to IntPtr.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        public static IntPtr ToIntPtr(this string value)
        {
            int.TryParse(value, out int result);
            return (IntPtr)result;
        }

        static readonly char[] xmlDelimiters = "<>&".ToCharArray();

        /// <summary>
        /// Returns the string data as a <see cref="T:System.Xml.Linq.XCData"/> if the value
        /// contains XML tags begin and end characters and it is not already a CDATA expression.
        /// </summary>
        /// <returns></returns>
        public static object ToXValue(this object value)
        {
            if (value == null)
                return null;

            string text = value.ToString();

            if (AutoElements.ForceCDataForConditions || (text.IndexOfAny(xmlDelimiters) != -1 && !text.Contains("![CDATA[")))
                return new XCData(text);
            else
                return text; //return raw value
        }

        /// <summary>
        /// Creates an Instance of <see cref="WixSharp.Bootstrapper.Payload" /> for the specified `sourceFile`.
        /// </summary>
        /// <param name="sourceFile">The source file.</param>
        /// <param name="name">The name f the payload.</param>
        /// <returns></returns>
        public static Bootstrapper.Payload ToPayload(this string sourceFile, string name = null)
        {
            if (sourceFile.IsNotEmpty())
                return new Bootstrapper.Payload { SourceFile = sourceFile, Name = name };
            else
                return null;
        }

        /// <summary>
        /// Converts semicolon or comma delimited list of language/culture name into the list of LCIDs.
        /// </summary>
        /// <param name="languages">The languages.</param>
        /// <returns></returns>
        public static string ToLcidList(this string languages)
        {
            var result = string.Join(",", languages.Split(',', ';')
                                                   .Select(x => new CultureInfo(x.Trim()).LCID.ToString())
                                                   .Distinct()
                                                   .ToArray());
            return result;
        }

        /// <summary>
        /// LCID of the first language in the semicolon or comma delimited list of languages
        /// </summary>
        /// <param name="languages">The languages.</param>
        /// <returns></returns>
        internal static string FirstLcid(this string languages)
        {
            var result = languages.Split(',', ';')
                                  .Select(x => new CultureInfo(x.Trim()).LCID.ToString())
                                  .FirstOrDefault();
            return result;
        }

        /// <summary>
        /// Determines whether this path is a file.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <returns><c>true</c> if the specified path is file; otherwise, <c>false</c>.</returns>
        public static bool IsFile(this string path)
            => IO.File.Exists(path);

        /// <summary>
        /// Checks if the file exists.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <returns></returns>
        public static bool FileExists(this string path)
            => IO.File.Exists(path);

        /// <summary>
        /// Determines whether this instance is directory.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <returns><c>true</c> if the specified path is directory; otherwise, <c>false</c>.</returns>
        public static bool IsDirectory(this string path)
            => IO.Directory.Exists(path);

        /// <summary>
        /// Determines if the specified path exists.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <returns></returns>
        public static bool PathExists(this string path) => IO.File.Exists(path) ? true : IO.Directory.Exists(path);

        /// <summary>
        /// Deletes File/Directory from by the specified path if it exists.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <param name="throw">if set to <c>false</c> handle all exceptions silently.</param>
        /// <returns></returns>
        public static string DeleteIfExists(this string path, bool @throw = false)
        {
            void deleteFile(string file)
            {
                try
                {
                    var fullPath = IO.Path.GetFullPath(file);
                    if (IO.File.Exists(fullPath))
                        IO.File.Delete(fullPath);
                }
                catch
                {
                    if (@throw)
                        throw;
                }
            }

            void deleteDir(string file)
            {
                try
                {
                    var fullPath = IO.Path.GetFullPath(file);
                    if (IO.Directory.Exists(fullPath))
                        IO.Directory.Delete(fullPath);
                }
                catch
                {
                    if (@throw)
                        throw;
                }
            }

            if (path.IsDirectory())
            {
                IO.Directory.GetFiles(path, "*", IO.SearchOption.AllDirectories)
                            .ForEach(deleteFile);

                IO.Directory.GetDirectories(path, "*", IO.SearchOption.AllDirectories)
                            .OrderByDescending(x => x)
                            .ForEach(deleteDir);

                deleteDir(path);
            }
            else
            {
                deleteFile(path);
            }

            return path;
        }

        /// <summary>
        /// Surrounds the specified text into quotation characters.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <param name="quotationCharacter">The quotation character.</param>
        /// <returns></returns>
        public static string Enquote(this string text, char quotationCharacter = '"')
        {
            return string.Format("{1}{0}{1}", text, quotationCharacter);
        }

        /// <summary>
        /// Deflates the whitespaces from the text.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <param name="whitespace">The whitespace.</param>
        /// <returns></returns>
        public static string DeflateWhitespaces(this string text, string whitespace = " ")
        {
            return text?.Replace(whitespace, "");
        }

        /// <summary>
        /// Simple wrapper around System.String.Compare(string strA, string strB, bool ignoreCase);
        /// </summary>
        /// <param name="strA">The string a.</param>
        /// <param name="strB">The string b.</param>
        /// <param name="ignoreCase">if set to <c>true</c> [ignore case].</param>
        /// <returns></returns>
        public static bool SameAs(this string strA, string strB, bool ignoreCase = false)
        {
            return 0 == string.Compare(strA, strB, ignoreCase);
        }

        /// <summary>
        /// Returns true if both values represent the same path.
        /// </summary>
        /// <param name="pathA">The path a.</param>
        /// <param name="pathB">The path b.</param>
        /// <returns></returns>
        public static bool SamePathAs(this string pathA, string pathB)
        {
            if (pathA.IsNotEmpty() && pathB.IsNotEmpty())
                return 0 == string.Compare(IO.Path.GetFullPath(pathA), IO.Path.GetFullPath(pathB), true);
            else
                return false;
        }

        /// <summary>
        /// Gets the location of the assembly.
        /// <p>
        /// Can discover the original location of the assembly being loaded from memory in case of a
        /// CS-Script assembly.
        /// </p>
        /// </summary>
        /// <param name="assembly">The assembly.</param>
        /// <returns></returns>
        public static string GetLocation(this System.Reflection.Assembly assembly)
        {
            if (assembly == null)
                return null;

            var location = "";
            try
            {
#pragma warning disable IL3000 // Avoid accessing Assembly file path when publishing as a single file
                location = assembly.Location;
#pragma warning restore IL3000
            }
            catch { }

            string scriptAsmLocation = Environment.GetEnvironmentVariable("location:" + assembly.GetHashCode());

            if (location.IsEmpty() && scriptAsmLocation.IsNotEmpty())
                location = scriptAsmLocation;

            return location;
        }

        /// <summary>
        /// The change the directory of the file path.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <param name="newDir">The new directory.</param>
        /// <returns></returns>
        public static string PathChangeDirectory(this string path, string newDir)
        {
            return IO.Path.Combine(newDir, IO.Path.GetFileName(path));
        }

#pragma warning disable CS0419 // Ambiguous reference in cref attribute

        /// <summary>
        /// Combines path parts. Encapsulates <see cref="Path.Combine" />
        /// </summary>
        /// <param name="path1">The path1.</param>
        /// <param name="path2">The path2.</param>
        /// <returns></returns>
#pragma warning restore CS0419 // Ambiguous reference in cref attribute

        public static string PathCombine(this string path1, string path2)
        {
            return IO.Path.Combine(path1, path2);
        }

        /// <summary>
        /// Combines path parts. Encapsulates <see cref="Path.Combine(string[])" />
        /// </summary>
        /// <param name="path1">The path1.</param>
        /// <param name="parts">The parts of the path.</param>
        /// <returns></returns>
        public static string PathCombine(this string path1, params object[] parts)
        {
            var allParts = new[] { path1 }.Concat(parts.Select(x => x?.ToString() ?? "")).ToArray();
            return IO.Path.Combine(allParts);
        }

        /// <summary>
        /// Gets the special folder path combined with an array of strings into a path.
        /// </summary>
        /// <param name="folder">The folder.</param>
        /// <param name="parts">The parts.</param>
        /// <returns></returns>
        public static string GetPath(this Environment.SpecialFolder folder, params object[] parts)
        {
            var allParts = new[] { Environment.GetFolderPath(folder) }.Concat(parts.Select(x => x?.ToString() ?? ""));
            return Path.Combine(allParts.ToArray());
        }

        /// <summary>
        /// The change the file name of the file path.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <param name="newFileName">The new file name.</param>
        /// <returns></returns>
        public static string PathChangeFileName(this string path, string newFileName)
        {
            return IO.Path.Combine(IO.Path.GetDirectoryName(path), newFileName);
        }

        /// <summary>
        /// Converts string value representing path into an absolute path. If string is null or
        /// empty it is treated as the CurrentDirectory equivalent.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <returns></returns>
        public static string ToAbsolutePath(this string path)
        {
            return IO.Path.GetFullPath(path.IsEmpty() ? Environment.CurrentDirectory : path);
        }

        /// <summary>
        /// Identical to <see cref="System.IO.Path.GetFileName(string)"/>. It is useful for Wix#
        /// consuming code as it allows avoiding "using System.IO;" directive, which interferes with
        /// Wix# types.
        /// </summary>
        /// <param name="path">The path.</param>
        public static string PathGetFileName(this string path)
        {
            return IO.Path.GetFileName(path);
        }

        /// <summary>
        /// Identical to <see cref="System.IO.Path.GetFileNameWithoutExtension(string)"/>. It is useful for Wix#
        /// consuming code as it allows avoiding "using System.IO;" directive, which interferes with
        /// Wix# types.
        /// </summary>
        /// <param name="path">The path.</param>
        public static string PathGetFileNameWithoutExtension(this string path)
        {
            return IO.Path.GetFileNameWithoutExtension(path);
        }

        /// <summary>
        /// Identical to <see cref="System.IO.Path.GetExtension(string)"/>. It is useful for Wix#
        /// consuming code as it allows avoiding "using System.IO;" directive, which interferes with
        /// Wix# types.
        /// </summary>
        /// <param name="path">The file extension.</param>
        public static string PathGetExtension(this string path)
        {
            return IO.Path.GetExtension(path);
        }

        /// <summary>
        /// Change extension of the file path.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <param name="extension">The extension.</param>
        /// <returns></returns>
        public static string PathChangeExtension(this string path, string extension)
        {
            return IO.Path.ChangeExtension(path, extension);
        }

        /// <summary>
        /// Adds the extension to the file path unless it already has the same extension.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <param name="extension">The extension.</param>
        /// <returns></returns>
        public static string PathEnsureExtension(this string path, string extension)
        {
            if (path.PathGetExtension().SameAs(extension, ignoreCase: true))
                return path;
            else
                return path + extension;
        }

        /// <summary>
        /// Ensures the directory exists.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <returns></returns>
        public static string EnsureDirExists(this string path)
        {
            Directory.CreateDirectory(path);
            return path;
        }

        /// <summary>
        /// Ensures the file exists.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <returns></returns>
        public static string EnsureFileExists(this string path)
        {
            var filePath = path.PathGetFullPath();

            if (!IO.File.Exists(filePath))
            {
                filePath.PathGetDirName().EnsureDirExists();
                IO.File.WriteAllBytes(filePath, new byte[0]);
            }
            return path;
        }

        /// <summary>
        /// Gets the full path.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <returns></returns>
        public static string PathGetFullPath(this string path)
        {
            return IO.Path.GetFullPath(path);
        }

        /// <summary>
        /// Formats the specified string.
        /// </summary>
        /// <param name="obj">The string to format.</param>
        /// <param name="args">The formatting arguments.</param>
        /// <returns>The formatted string.</returns>
        public static string FormatWith(this string obj, params object[] args)
        {
            return string.Format(obj, args);
        }

        /// <summary>
        /// Fluent method for performing an action with the object.
        /// </summary>
        /// <param name="obj">The obj.</param>
        /// <param name="action">The action.</param>
        /// <returns></returns>
        public static T With<T>(this T obj, Action<T> action)
        {
            action(obj);
            return obj;
        }

        /// <summary>
        /// A method for convenient deconstruction of a slice of an array into a tuple.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <param name="head"></param>
        /// <param name="tail"></param>
        public static void Deconstruct<T>(this IEnumerable<T> list, out T head, out IEnumerable<T> tail)
        {
            head = list.First(); // throws InvalidOperationException for empty list
            tail = list.Skip(1);
        }

        /// <summary>
        /// A method for convenient deconstruction of a slice of an array into a tuple
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <param name="head"></param>
        /// <param name="next"></param>
        /// <param name="tail"></param>
        public static void Deconstruct<T>(this IEnumerable<T> list, out T head, out T next, out IEnumerable<T> tail)
        {
            head = list.First();
            (next, tail) = list.Skip(1);
        }

        /// <summary>
        /// Splits string by lines. The method handles both '\r\n' and '\n' line endings.
        /// </summary>
        /// <param name="text">The text to be split.</param>
        /// <returns></returns>
        public static string[] GetLines(this string text)
        {
            return text.Replace("\r\n", "\n").Split('\n');
        }

        /// <summary>
        /// Replaces all Wix# predefined string constants (Environment Constants) in the Wix#
        /// directory path with their WiX equivalents and escapes all WiX illegal characters (e.g.
        /// space character).
        /// <para>
        /// <para>
        /// It also replaces all "illegal" characters (e.g. !,\) with '_' character to allow the
        /// path value to be used as a WiX Id XML attribute.
        /// </para>
        /// <example>The following is an example of expanding directory name paths.
        /// <code>
        ///@"%ProgramFiles%\My Company\My Product".Expand()       -&gt; @"ProgramFilesFolder\My_Company\My_Product"
        ///@"ProgramFilesFolder\My Company\My Product".Expand()   -&gt; @"ProgramFilesFolder\My_Company\My_Product"
        ///@"[ProgramFilesFolder]\My Company\My Product".Expand() -&gt; @"ProgramFilesFolder\My_Company\My_Product"
        /// </code>
        /// </example>
        /// </para>
        /// For the list of supported constants analyses <c>WixSharp.Compiler.EnvironmentConstantsMapping.Keys</c>.
        /// </summary>
        /// <param name="path">The Wix# directory path.</param>
        /// <param name="doNotFixStartDigit">
        /// if set to <c>true</c> starting from digit character is permitted.
        /// </param>
        /// <returns>Replacement result.</returns>
        public static string Expand(this string path, bool doNotFixStartDigit = false)
        {
            var result = path.ExpandWixEnvConsts()
                             .Replace("\\", ".")
                             .EscapeIllegalCharacters(doNotFixStartDigit);

            if (result.FirstOrDefault() == '.')
                result = "_" + result.Substring(1);

            return result;
        }

        /// <summary>
        /// Determines whether the string contains WiX constants (values enclosed into square brackets).
        /// </summary>
        /// <param name="data">The data.</param>
        /// <returns></returns>
        public static bool ContainsWixConstants(this string data)
        {
            return data.Contains("[") || data.Contains("]");
        }

        /// <summary>
        /// Determines whether the value is a WiX constant (e.g. 'SystemFolder').
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns><c>true</c> if the specified value is a WiX constant; otherwise, <c>false</c>.</returns>
        public static bool IsWixConstant(this string value)
        {
            return Compiler.EnvironmentConstantsMapping.ContainsValue(value);
        }

        /// <summary>
        /// Maps the Wix# constants included in path into their x64 equivalents.
        /// <para>
        /// For example %ProgramFiles%\My Company\My Product should be preprocessed into
        /// %ProgramFiles64%\My Company\My Product
        /// </para>
        /// </summary>
        /// <param name="path">The path.</param>
        /// <returns></returns>
        public static string Map64Dirs(this string path)
        {
            //directory ID (e.g. %ProgramFiles%\My Company\My Product should be preprocessed into %ProgramFiles64%\My Company\My Product)
            foreach (string key in Compiler.EnvironmentFolders64Mapping.Keys)
            {
                if (path.Contains(key))
                    path = path.Replace(key, Compiler.EnvironmentFolders64Mapping[key]);
            }
            return path;
        }

        /// <summary>
        /// Replaces all Wix# predefined string constants (Environment Constants) in the Wix#
        /// directory path with their WiX equivalents and escapes all WiX illegal characters (e.g.
        /// space character).
        /// <para><example>The following is an example of expanding directory name paths.
        /// <code>
        ///@"%ProgramFiles%\My Company\My Product".Expand()       -&gt; @"ProgramFilesFolder\My_Company\My_Product"
        ///@"ProgramFilesFolder\My Company\My Product".Expand()   -&gt; @"ProgramFilesFolder\My_Company\My_Product"
        ///@"[ProgramFilesFolder]\My Company\My Product".Expand() -&gt; @"ProgramFilesFolder\My_Company\My_Product"
        /// </code>
        /// </example>
        /// </para>
        /// For the list of supported constants analyse <c>WixSharp.Compiler.EnvironmentConstantsMapping.Keys</c>.
        /// </summary>
        /// <param name="path">The Wix# directory path.</param>
        /// <returns>Replacement result.</returns>
        public static string ExpandWixEnvConsts(this string path)
        {
            //directory ID (e.g. %ProgramFiles%\My Company\My Product should be interpreted as ProgramFilesFolder\My Company\My Product)
            foreach (string key in Compiler.EnvironmentConstantsMapping.Keys)
                path = path.Replace(key, Compiler.EnvironmentConstantsMapping[key])
                           .Replace("[" + Compiler.EnvironmentConstantsMapping[key] + "]", Compiler.EnvironmentConstantsMapping[key]);
            return path;
        }

        /// <summary>
        /// Normalizes the wix environment constants and custom properties.
        /// <para>
        /// This method is not the same as `ExpandWixEnvConsts`. The key difference is that it
        /// handles custom properties, leaves square brackets unchanged and also normalizes
        /// directory separators. Normalization is critical for string values that are used as `ExeFileShortcut.Target`:
        /// </para>
        /// <para><example>
        /// <code>
        ///@"%INSTALLDIR%\my_app.exe".NormalizeWixString() -&gt; "[INSTALLDIR]my_app.exe"
        ///@"%INSTALLDIR%my_app.exe".NormalizeWixString()  -&gt; "[INSTALLDIR]my_app.exe"
        ///@"[INSTALLDIR]my_app.exe".NormalizeWixString()  -&gt; "[INSTALLDIR]my_app.exe"
        /// </code>
        /// </example>
        /// </para>
        /// </summary>
        /// <param name="path">The path.</param>
        /// <returns></returns>
        public static string NormalizeWixString(this string path)
        {
            // EnvironmentConstantsMapping.Keys include '%' chars: { "%ProgramFiles%",
            // "ProgramFilesFolder" },
            foreach (string key in Compiler.EnvironmentConstantsMapping.Keys)
                path = path
                           // if the constant came to this method already extended/normalized then
                           // the call `.Replace(key.Trim('%'),
                           // Compiler.EnvironmentConstantsMapping[key]` would insert suffix
                           // `Folder` one extra time (e.g. SystemFolder64Folder->SystemFolder64FolderFolder)

                           // Another problem is that *64Folder/*FilesFolder can be ruined by *
                           // replacement ProgramFiles64Folder/ProgramFilesFolder <- ProgramFiles
                           // System64Folder, SystemFolder <- System

                           // The solution is not elegant in terms of performance but adequate. We
                           // don't need performance during the compilation.

                           // protect `System` and `ProgramFiles`
                           .Replace("System64Folder", "Sys64Folder")
                           .Replace("SystemFolder", "SysFolder")
                           .Replace("ProgramFiles64Folder", "ProgFiles64Folder")
                           .Replace("ProgramFilesFolder", "ProgFilesFolder")

                           .Replace(key, Compiler.EnvironmentConstantsMapping[key])
                           // .Replace(key.Trim('%'), Compiler.EnvironmentConstantsMapping[key])
                           .Replace($"[{key.Trim('%')}]", Compiler.EnvironmentConstantsMapping[key])

                           // restore `System` and `ProgramFiles`
                           .Replace("Sys64Folder", "System64Folder")
                           .Replace("SysFolder", "SystemFolder")
                           .Replace("ProgFiles64Folder", "ProgramFiles64Folder")
                           .Replace("ProgFilesFolder", "ProgramFilesFolder");

            // ensure `%System64Folder%msiexec.exe` and `%System64Folder%\msiexec.exe` are converted
            // in `[System64Folder]msiexec.exe`
            var chars = path.Replace(@"%\", "%")
                            .ToArray();

            // Handle `%MY_CUSTOM_PROPERTY%MyApp.exe`
            bool leftToken = true;
            for (int i = 0; i < chars.Length; i++)
                if (chars[i] == '%')
                {
                    chars[i] = leftToken ? '[' : ']';
                    leftToken = !leftToken;
                }
            var result = new string(chars);

            return result;
        }

        /// <summary>
        /// Unescape '\%' characters in the tokens representing environment variables (e.g.
        /// "%ProgramFiles%\My Product").
        /// <para>
        /// Required for avoiding collisions between environment variables and WiX constants. For
        /// example to prevent "%ProgramFiles%\My Product" being later converted into
        /// "ProgramFilesFolder\My Product"
        /// </para>
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        internal static string UnEscapeEnvars(this string text)
        {
            return text.Replace("\\%", "%");
        }

        /// <summary>
        /// Escape '%' characters in the tokens representing environment variables (e.g.
        /// "%ProgramFiles%\My Product").
        /// <para>
        /// Required for avoiding collisions between environment variables and WiX constants. For
        /// example to prevent "%ProgramFiles%\My Product" being later converted into
        /// "ProgramFilesFolder\My Product"
        /// </para>
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        internal static string EscapeEnvars(this string text)
        {
            return text.Replace("%", "\\%");
        }

        internal static string ReplaceWixSharpEnvConsts(this string path)
        {
            //%ProgramFiles%\My Company\My Product -> [ProgramFilesFolder]\My Company\My Product
            foreach (string key in Compiler.EnvironmentConstantsMapping.Keys)
                path = path.Replace(key, "[" + Compiler.EnvironmentConstantsMapping[key] + "]");
            return path.Replace("%INSTALLDIR%", "[INSTALLDIR]"); //well known constant that is not a part of WiX constants set
        }

        /// <summary>
        /// Expands the EnvironmentVariable It is nothing else but a an extension method wrapping
        /// Environment.ExpandEnvironmentVariables to allow fluent API.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <returns></returns>
        public static string ExpandEnvVars(this string path)
        {
            if (path == null)
                return path;
            else
                return Environment.ExpandEnvironmentVariables(path);
        }

        /// <summary>
        /// Escapes the illegal characters in the WiX Id value.
        /// </summary>
        /// <param name="data">The data.</param>
        /// <param name="doNotFixStartDigit">
        /// if set to <c>true</c> starting from digit character is permitted.
        /// </param>
        /// <returns></returns>
        public static string EscapeIllegalCharacters(this string data, bool doNotFixStartDigit = false)
        {
            List<char> legalChars = new List<char>();

            legalChars.AddRange("._0123456789".ToCharArray());
            legalChars.AddRange("abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ".ToCharArray());

            // Replace non-legal character to their character code. This is to prevent a bug that
            // the directory Ids are duplicated if directory names consists of non-legal characters,
            // and the lengths are same.
            var escaped = new StringBuilder();
            foreach (var ch in data)
            {
                if (legalChars.Contains(ch))
                {
                    escaped.Append(ch);
                }
                else
                {
                    escaped.Append(string.Format("{0:x}", (int)ch));
                }
            }
            string retval = escaped.ToString();

            if (!doNotFixStartDigit && (retval.IsNotEmpty() && (retval[0].IsDigit() || retval.StartsWith("."))))
                return "_" + retval;
            else
                return retval;
        }

        internal static string ExpandCommandPath(this string path)
        {
            foreach (string key in Compiler.EnvironmentConstantsMapping.Keys)
                path = path.Replace(key, "[" + Compiler.EnvironmentConstantsMapping[key] + "]");

            return path;
        }

        internal static bool ContainsSimilarKey<T>(this Dictionary<string, T> dictionary, string lookupKey)
        {
            foreach (string key in dictionary.Keys)
                if (string.Compare(key, lookupKey, true) == 0)
                    return true;
            return false;
        }

        /// <summary>
        /// Reverse equivalent of Enum.HasFlag of .NET v4.5
        /// </summary>
        static public bool PresentIn<T>(this T enumValue, T obj) where T : struct, IConvertible
        {
            if (!typeof(T).IsEnum)
                throw new ArgumentException("T must be an enumerated type");

            int iObj = ((IConvertible)obj).ToInt32(null);
            int iEnumValue = ((IConvertible)enumValue).ToInt32(null);

            return (iObj & iEnumValue) == iEnumValue;
        }

        internal static string ToYesNo(this bool obj)
        {
            return obj ? "yes" : "no";
        }

        internal static string ToNullOrYesNo(this bool? obj)
        {
            if (obj.HasValue)
                return obj.Value ? "yes" : "no";
            else
                return null;
        }

        /// <summary>
        /// Determines if the specified sequence has no items. It is opposite of IEnumerable&lt;TSource&gt;.Any().
        /// </summary>
        /// <typeparam name="TSource">The type of the T source.</typeparam>
        /// <param name="source">The source.</param>
        /// <param name="predicate">A function to test each element for a condition.</param>
        /// <returns></returns>
        public static bool None<TSource>(this IEnumerable<TSource> source, Func<TSource, bool> predicate = null)
            => predicate != null ? !source.Any(predicate) : !source.Any();

        /// <summary>
        /// Determines if the collection contains a single item.
        /// </summary>
        /// <typeparam name="TSource">The type of the source.</typeparam>
        /// <param name="source">The source.</param>
        /// <returns></returns>
        public static bool SingleItem<TSource>(this IEnumerable<TSource> source) => source.Count() == 1;

        /// <summary>
        /// Determines whether the given string is empty.
        /// </summary>
        /// <param name="s">The string to analyze.</param>
        /// <returns><c>true</c> if the specified s is empty; otherwise, <c>false</c>.</returns>
        public static bool IsEmpty(this string s)
        {
            return string.IsNullOrEmpty(s);
        }

        /// <summary>
        /// Determines whether the given string is empty or not.
        /// </summary>
        /// <param name="s">The string to analyses.</param>
        /// <returns><c>true</c> if the specified string is not empty; otherwise, <c>false</c>.</returns>
        public static bool IsNotEmpty(this string s)
        {
            return !string.IsNullOrEmpty(s);
        }

        /// <summary>
        /// Sorts the elements of a sequence in ascending order with item's default comparison operators.
        /// </summary>
        /// <typeparam name="TSource">The type of the source.</typeparam>
        /// <param name="source">The source.</param>
        /// <returns></returns>
        public static IOrderedEnumerable<TSource> Order<TSource>(this IEnumerable<TSource> source)
        {
            return source.OrderBy(x => x);
        }

        /// <summary>
        /// Sorts the elements of a sequence in descending order with item's default comparison operators.
        /// </summary>
        /// <typeparam name="TSource">The type of the source.</typeparam>
        /// <param name="source">The source.</param>
        /// <returns></returns>
        public static IOrderedEnumerable<TSource> OrderDescending<TSource>(this IEnumerable<TSource> source)
        {
            return source.OrderByDescending(x => x);
        }

        /// <summary>
        /// Returns the first item that starts with one of the specified possible prefixes. The
        /// method returns the matching item value without the prefix.
        /// </summary>
        /// <example>
        /// This method is convenient a convenient way of parsing command line arguments.
        /// <code>
        /// // Command line: "app.exe -out:.\log.file"
        /// // outFile value is ".\log.file"
        /// string outFile = Environment.GetCommandLineArgs().FirstPrefixedValue("-out:", "-o:");
        /// </code>
        /// </example>
        /// <param name="items">The items.</param>
        /// <param name="possiblePreffixes">The possible prefixes.</param>
        /// <returns></returns>
        public static string FirstPrefixedValue(this IEnumerable<string> items, params string[] possiblePreffixes)
        {
            foreach (var preffix in possiblePreffixes)
            {
                string match = items.Where(x => x.StartsWith(preffix, true))
                                    .LastOrDefault();

                if (match.IsNotEmpty())
                {
                    return match.Substring(preffix.Length);
                }
            }

            return "";
        }

        /// <summary>
        /// Returns all leading white-space characters.
        /// </summary>
        /// <param name="s">The string to analyse.</param>
        /// <returns>Total count of leading white-space characters</returns>
        public static int GetLeftIndent(this string s)
        {
            return s.Length - s.TrimStart('\n', '\r', '\t', ' ').Length;
        }

        /// <summary>
        /// Concats the specified strings. In the result string all items are separated with the
        /// specified delimiter.
        /// </summary>
        /// <param name="strings">The strings.</param>
        /// <param name="delimiter">The delimiter.</param>
        /// <returns></returns>
        public static string ConcatItems(this IEnumerable<string> strings, string delimiter)
        {
            StringBuilder retval = new StringBuilder();
            foreach (var s in strings)
            {
                retval.Append(s);
                retval.Append(delimiter);
            }
            return retval.ToString();
        }

        /// <summary>
        /// Selects from the given element the first child element matching the specified path (e.g. <c>Select("Product/Package")</c>).
        /// </summary>
        /// <param name="element">The element to be searched.</param>
        /// <param name="path">The path.</param>
        /// <returns>The element matching the path.</returns>
        public static XElement Select(this XContainer element, string path)
        {
            string[] parts = path.Split('/');

            var e = element.Elements()
                           .Where(el => el.Name.LocalName == parts[0])
                           .GetEnumerator();

            if (!e.MoveNext())
                return null;

            if (parts.Length == 1) //the last link in the chain
                return e.Current;
            else
                return e.Current.Select(path.Substring(parts[0].Length + 1)); //be careful RECURSION
        }

        static bool IsXmlNamespaceAsigningDangerous()
        {
            var doc = XDocument.Parse(@"<Root xmlns=""http://www.test.com/xml/2015"">
                                          <Element xmlns=""""/>
                                        </Root>");

            XNamespace ns = doc.Root.Name.NamespaceName;

            var e = doc.Root.Elements().First();
            e.Name = ns + e.Name.LocalName;

            try
            {
                var xml = doc.ToString();
                return false;
            }
            catch
            {
                return true;
            }
        }

        static bool? canSetXmlNamespaceSafely;

        static bool CanSetXmlNamespaceSafely
        {
            get
            {
                if (!canSetXmlNamespaceSafely.HasValue)
                    canSetXmlNamespaceSafely = !IsXmlNamespaceAsigningDangerous();
                return canSetXmlNamespaceSafely.Value;
            }
        }

        internal static XDocument AddDefaultNamespaces(this XDocument doc)
        {
            //part of Issue#67 workaround
            //For some reason after changing the XML namespace dox.ToString() triggers exception "The prefix '' cannot be redefined
            //from 'http://schemas.microsoft.com/wix/2006/wi' to '' within the same start element tag."
            //This error leads to the failure of the XML serialization with StringWriterWithEncoding (in BuildWxs).
            //Strangle enough serialization is affected by presence of WixVariable element !!??
            //While ToString fails always
            if (CanSetXmlNamespaceSafely)
            {
                XNamespace ns = doc.Root.Name.NamespaceName;
                doc.Root.Descendants().ForEach(x =>
                {
                    if (x.Name.Namespace.NamespaceName.IsEmpty())
                        x.Name = ns + x.Name.LocalName;
                });
            }
            else
            {
                //Using simplistic, inefficient but safe string manipulation with regeneration of all elements
                var xml = doc.ToString().Replace("xmlns=\"\"", "");
                var newRoot = XElement.Parse(xml);

                doc.Root.RemoveAll();

                //need to add namespaces (via attributes) as well
                doc.Root.Add(newRoot.Attributes());
                doc.Root.Add(newRoot.Elements());
            }
            return doc;
        }

        /// <summary>
        /// Selects from the given element the first child element Directory matching the specified
        /// path (e.g. <c>Select("ProgramFiles/MyCompany") by</c>).
        /// </summary>
        /// <param name="element">The element to be searched.</param>
        /// <param name="path">The path.</param>
        /// <returns>The element matching the path.</returns>
        public static XElement FindDirectory(this XElement element, string path)
        {
            string[] parts = path.Split('/');

            var e = (from el in element.Elements()
                     where el.Name.LocalName == "Directory" && el.Attribute("Name") != null && el.Attribute("Name").Value == parts[0]
                     select el).GetEnumerator();

            if (!e.MoveNext())
                return null;

            if (parts.Length == 1) //the last link in the chain
                return e.Current;
            else
                return e.Current.FindDirectory(path.Substring(parts[0].Length + 1)); //be careful RECURSION
        }

        /// <summary>
        /// Determines whether the XElement has the specified <c>LocalName</c>.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <param name="elementName">Name of the element.</param>
        /// <param name="ignoreCase">if set to <c>true</c> [ignore case].</param>
        /// <returns></returns>
        public static bool HasLocalName(this XElement element, string elementName, bool ignoreCase = false)
        {
            return element.Name.LocalName.SameAs(elementName, ignoreCase);
        }

        /// <summary>
        /// Iterates through the all already prepared/processed components grouped by features and
        /// either add the new component to the existing group or to the freshly created one.
        /// <para>featureComponents[feature].Add(componentId);</para>
        /// </summary>
        /// <param name="featureComponents">The feature components.</param>
        /// <param name="features">The features.</param>
        /// <param name="componentId">The component identifier.</param>
        public static void Map(this Dictionary<Feature, List<string>> featureComponents, IEnumerable<Feature> features, string componentId)
        {
            foreach (var item in features)
                featureComponents.MapOld(item, componentId);
        }

        internal static void MapOld(this Dictionary<Feature, List<string>> featureComponents, Feature feature, string componentId)
        {
            if (!featureComponents.ContainsKey(feature))
                featureComponents[feature] = new List<string>();

            featureComponents[feature].Add(componentId);
        }

        /// <summary>
        /// Adds the XML (*.wxi) include. <example>The following is an example of including external
        /// XML files.
        /// <code>
        ///project.AddXmlInclude("CommonProperies.wxi")
        ///.AddXmlInclude("CommonProperies2.wxi");
        ///
        ///new File(@"Files\Bin\MyApp.exe")
        ///.AddXmlInclude("FileItems.wxi", parentElement: "Component");
        /// </code>
        /// </example>
        /// </summary>
        /// <typeparam name="T">The type of the T.</typeparam>
        /// <param name="entity">The entity.</param>
        /// <param name="xmlFile">The XML file.</param>
        /// <param name="parentElement">The parent element.</param>
        /// <returns></returns>
        public static T AddXmlInclude<T>(this T entity, string xmlFile, string parentElement = null) where T : WixEntity
        {
            entity.AddInclude(xmlFile, parentElement);
            return entity;
        }

        /// <summary>
        /// Adds the specified XML content as a WiX Fragment/FragmentRef elements combination.
        /// </summary>
        /// <param name="placementElement">
        /// The element the reference to the fragment should be placed at.
        /// </param>
        /// <param name="content">The fragment content.</param>
        /// <returns></returns>
        public static XElement AddWixFragment(this XElement placementElement, params XElement[] content)
        {
            var fragment = new XElement("Fragment");
            foreach (XElement item in content)
            {
                item.EnsureId();

                var refElement = new XElement(item.Name.Namespace + item.Name.LocalName + "Ref", item.Attribute("Id"));
                placementElement.Add(refElement);

                fragment.Add(item);
            }
            placementElement.Document.Root.Add(fragment);
            return placementElement;
        }

        static readonly Dictionary<string, int> autoXIds = new Dictionary<string, int>();

        static void EnsureId(this XElement element)
        {
            if (element.Attribute("Id") == null)
            {
                string name = element.Name.LocalName;
                if (autoXIds.ContainsKey(name))
                    autoXIds[name] = autoXIds[name] + 1;
                else
                    autoXIds[name] = 1;
                element.SetAttribute("Id", name + autoXIds[name]);
            }
        }

        /// <summary>
        /// Adds the specified extension to <see cref="T:WixSharp.WixProject"/>
        /// </summary>
        /// <param name="project">The project.</param>
        /// <param name="extension">The extension.</param>
        /// <returns></returns>
        [Obsolete(message: "This method has been renamed. Use `Include` instead", error: false)]
        static public WixProject IncludeWixExtension(this WixProject project, WixExtension extension)
        {
            project.IncludeWixExtension(extension.Assembly, extension.XmlNamespacePrefix, extension.XmlNamespace);
            return project;
        }

        /// <summary>
        /// Adds the specified extension to <see cref="T:WixSharp.WixProject"/>.
        /// </summary>
        /// <param name="project">The project.</param>
        /// <param name="extension">The extension.</param>
        /// <returns></returns>
        static public WixProject Include(this WixProject project, WixExtension extension)
        {
            project.IncludeWixExtension(extension.Assembly, extension.XmlNamespacePrefix, extension.XmlNamespace);
            return project;
        }

        /// <summary>
        /// Adds the specified extension to <see cref="T:WixSharp.WixProject"/>
        /// </summary>
        /// <param name="project">The project.</param>
        /// <param name="extensionAssembly">The extension assembly.</param>
        /// <param name="namespacePrefix">The namespace prefix.</param>
        /// <param name="namespace">The namespace.</param>
        /// <returns></returns>
        static public WixProject IncludeWixExtension(this WixProject project, string extensionAssembly, string namespacePrefix, string @namespace)
        {
            if (!project.WixExtensions.Contains(extensionAssembly))
                project.WixExtensions.Add(extensionAssembly);

            if (namespacePrefix.IsNotEmpty())
            {
                var namespaceDeclaration = WixExtension.GetNamespaceDeclaration(namespacePrefix, @namespace);
                //could use detection of duplicate prefixes
                if (!project.WixNamespaces.Contains(namespaceDeclaration))
                    project.WixNamespaces.Add(namespaceDeclaration);
            }
            return project;
        }

        /// <summary>
        /// Selects single descendant element with a given name (LocalName). Throws if no or more
        /// then one match found
        /// </summary>
        /// <param name="container">The element to be searched.</param>
        /// <param name="elementName">The element local name.</param>
        /// <returns>The elements matching the name.</returns>
        public static XElement FindSingle(this XContainer container, string elementName)
        {
            return container.Descendants().Single(x => x.Name.LocalName == elementName);
        }

        /// <summary>
        /// Selects all descendant elements with a given name (LocalName). Throws if no or more then
        /// one match found
        /// </summary>
        /// <param name="element">The element to be searched.</param>
        /// <param name="elementName">The element local name.</param>
        /// <returns>The elements matching the name.</returns>
        public static XElement[] FindAll(this XContainer element, string elementName)
        {
            return element.Descendants().Where(x => x.Name.LocalName == elementName).ToArray();
        }

        /// <summary>
        /// Reads the localized string from the WXL localization file.
        /// </summary>
        /// <param name="wxlFile">The WXL file.</param>
        /// <param name="stringId">The string identifier.</param>
        /// <returns></returns>
        public static string GetLocalizedString(this string wxlFile, string stringId)
            => XDocument.Load(wxlFile).FindAll("String").First(x => x.HasAttribute("Id", stringId)).Attr("Value");

        /// <summary>
        /// Selects the first descendant element with a given name (LocalName).
        /// </summary>
        /// <param name="element">The element to be searched.</param>
        /// <param name="elementName">The element local name.</param>
        /// <returns>The element matching the name.</returns>
        public static XElement FindFirst(this XContainer element, string elementName)
        {
            return element.Descendants().Where(x => x.Name.LocalName == elementName).FirstOrDefault();
        }

        /// <summary>
        /// Selects the first descendant "Component" element and returns its parent XElement.
        /// </summary>
        /// <param name="element">The element to be searched.</param>
        /// <returns>Parent XElement of the first component.</returns>
        public static XElement FindFirstComponentParent(this XContainer element)
        {
            return element.FindFirst("Component")?.Parent;
        }

        /// <summary>
        /// Selects the first parent element with the "Component" name.
        /// </summary>
        /// <param name="element">The element to search the component element for.</param>
        /// <returns>Parent component of the XElement.</returns>
        public static XElement Component(this XElement element)
        {
            return element.Parent("Component");
        }

        /// <summary>
        /// Selects the first descendant "Directory" element that has no other sub-directories
        /// (child "Directory" XElements).
        /// </summary>
        /// <param name="element">The element to be searched.</param>
        /// <returns>Directory XElement.</returns>
        public static XElement FindLastDirectory(this XContainer element)
        {
            return element.Descendants("Directory")
                          .FirstOrDefault(x => x.Element("Directory") == null);
        }

        /// <summary>
        /// Removes the element from its current parent and inserts it into another element.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <param name="newParent">The new parent.</param>
        /// <returns></returns>
        public static XElement MoveTo(this XElement element, XElement newParent)
        {
            element.Remove();
            newParent.Add(element);
            return element;
        }

        /// <summary>
        /// Selects, from the given element, the child element matching the specified path.
        /// <para>
        /// If the child element is not found, a new element is created matching the path (e.g. <c>SelectOrCreate("userSettings/MyApp.Properties.Settings/setting")</c>).
        /// </para>
        /// </summary>
        /// <param name="element">The element to be searched.</param>
        /// <param name="path">The path.</param>
        /// <returns>The element matching the path.</returns>
        public static XElement SelectOrCreate(this XContainer element, string path)
        {
            string[] parts = path.Split('/');

            var e = element.Elements()
                           .Where(el => el.Name.LocalName == parts[0])
                           .GetEnumerator();

            XElement currentElement = null;
            if (!e.MoveNext())
            {
                if (element is XDocument doc)
                {
                    if (doc.Root == null)
                        doc.Add(currentElement = new XElement(parts[0]));
                    else
                        throw new Exception("This operation would create an XML document with multiple roots.");
                }
                else if (element is XElement el)
                {
                    currentElement = el.AddElement(new XElement(parts[0]));
                }
            }
            else
                currentElement = e.Current;

            if (parts.Length == 1) //the last link in the chain
                return currentElement;
            else
                return currentElement.SelectOrCreate(path.Substring(parts[0].Length + 1)); //be careful RECURSION
        }

        /// <summary>
        /// Gets WiX compatible string representation (e.g. HKCR, HKLM).
        /// </summary>
        /// <param name="value">The <see cref="T:Microsoft.Win32.RegistryHive"/> value to convert.</param>
        /// <returns>WiX compatible string representation.</returns>
        public static string ToWString(this Microsoft.Win32.RegistryHive value)
        {
            switch (value)
            {
                case Microsoft.Win32.RegistryHive.ClassesRoot: return "HKCR";
                case Microsoft.Win32.RegistryHive.CurrentUser: return "HKCU";
                case Microsoft.Win32.RegistryHive.LocalMachine: return "HKLM";
                case Microsoft.Win32.RegistryHive.Users: return "HKU";
                default: return "unsupported root type";
            }
        }

        /// <summary>
        /// Converts <see cref="T:WixSharp.Sequence"/> into the WiX identifier by removing WiX
        /// illegal characters.
        /// </summary>
        /// <param name="value">The <see cref="T:WixSharp.Sequence"/> value.</param>
        /// <returns>Valid WiX identifier.</returns>
        internal static string ToWString(this Sequence value)
        {
            return value.ToString().Replace(" ", "_");
        }

        /// <summary>
        /// Converts the string into the WiX identifier by removing WiX illegal characters.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>Valid WiX identifier.</returns>
        internal static string ToWString(this string value)
        {
            return value.Replace(" ", "_");
        }

        internal static string Serialize(this Dictionary<string, string> data)
        {
            return string.Join("\n", data.Select(x => x.Key + "=" + x.Value.EscapeKeyValue()).ToArray());
        }

        /// <summary>
        /// Sets the environment variables based on Key/Value pares of the dictionary.
        /// </summary>
        /// <param name="data">The data.</param>
        /// <returns></returns>
        public static IDictionary<string, string> SetEnvironmentVariables(this IDictionary<string, string> data)
        {
            foreach (var key in data.Keys)
                Environment.SetEnvironmentVariable(key, data[key]);

            return data;
        }

        /// <summary>
        /// Escapes any serialization tokens in the key value string. These tokens are '=', ';' and '\n'.
        /// </summary>
        /// <param name="data">The data.</param>
        /// <returns></returns>
        public static string EscapeKeyValue(this string data)
        {
            return data.Replace(";", "{$SMCOL}")
                       .Replace("=", "{$EQV}")
                       .Replace("\n", "{$NL}");
        }

        /// <summary>
        /// Unescapes any serialization tokens in the key value string. These tokens are '{$EQV}',
        /// '{$SMCOL}' and '{$NL}'.
        /// </summary>
        /// <param name="data">The data.</param>
        /// <returns></returns>
        public static string UnescapeKeyValue(this string data)
        {
            return data.Replace("{$NL}", "\n")
                       .Replace("{$SMCOL}", ";")
                       .Replace("{$EQV}", "=");
        }

        /// <summary>
        /// Merges and replaces key values in a given dictionary ( <c>map</c> parameter) with
        /// another dictionary values. Another dictionary is provided in a serialized form (
        /// <c>data</c> parameter).
        /// </summary>
        /// <param name="map">The dictionary, which is a subject of the merge operation.</param>
        /// <param name="data">The merge key/values source in it's serialized form data.</param>
        /// <returns></returns>
        public static Dictionary<string, string> MergeReplace(this Dictionary<string, string> map, string data)
        {
            if (data.IsNotEmpty())
                foreach (var item in data.ToDictionary(itemDelimiter: '\n'))
                    map[item.Key] = item.Value.UnescapeKeyValue();
            return map;
        }

        /// <summary>
        /// Gets the value by specified key. Return <c>null</c> if the dictionary does not contains
        /// the specified key.
        /// </summary>
        /// <typeparam name="T1">The type of the 1.</typeparam>
        /// <typeparam name="T2">The type of the 2.</typeparam>
        /// <param name="map">The map.</param>
        /// <param name="key">The key.</param>
        /// <returns></returns>
        public static T2 Get<T1, T2>(this Dictionary<T1, T2> map, T1 key) where T2 : class
            => map.ContainsKey(key) ? map[key] : null;

        /// <summary>
        /// Sets the adds or sets key/value pair.
        /// <para>Removes the key/value pair if the specified value is <c>null</c>.</para>
        /// </summary>
        /// <typeparam name="T1">The type of the 1.</typeparam>
        /// <typeparam name="T2">The type of the 2.</typeparam>
        /// <param name="map">The map.</param>
        /// <param name="key">The key.</param>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        public static Dictionary<T1, T2> Set<T1, T2>(this Dictionary<T1, T2> map, T1 key, T2 value) where T2 : class
        {
            if (value != null)
                map[key] = value;
            else if (map.ContainsKey(key))
                map.Remove(key);
            return map;
        }

        /// <summary>
        /// Clones the specified collection.
        /// </summary>
        /// <param name="collection">The collection.</param>
        /// <returns></returns>
        public static Dictionary<string, string> Clone(this Dictionary<string, string> collection)
            => new Dictionary<string, string>(collection);

        /// <summary>
        /// Converts the string into the <see cref="T:WixSharp.Condition"/> instance.
        /// </summary>
        /// <param name="value">The string value.</param>
        /// <returns><see cref="T:WixSharp.Condition"/> instance.</returns>
        public static Condition ToCondition(this string value)
        {
            return Condition.Create(value);
        }

        /// <summary>
        /// Generates string representation without revision part.
        /// </summary>
        /// <param name="ver">The instance of the <see cref="T:System.Version"/>.</param>
        /// <returns><see cref="T:System.String"/></returns>
        public static string ToNoRevisionString(this Version ver)
        {
            return string.Format("{0}.{1}.{2}", ver.Major, ver.Minor, ver.Build);
        }

        /// <summary>
        /// Determines whether this instance is uninitialized.
        /// </summary>
        /// <param name="ver">The ver.</param>
        /// <returns>
        ///   <c>true</c> if the specified ver is uninitialized; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsUninitialized(this Version ver)
            => ver.Major == 0 && ver.Minor == 0 && ver.Build == 0 && ver.Revision == -1;

        /// <summary>
        /// Adds/combines given <see cref="T:IEnumerable&lt;T&gt;"/> object with the specified items.
        /// <para>
        /// If you are adding items to the <c>Project</c> or <c>Dir</c> then you can use the
        /// dedicated methods for that (e.g. `dir.AffFiles(drivers)`).
        /// </para>
        /// </summary>
        /// <typeparam name="T1">The type of the elements of <c>obj</c>.</typeparam>
        /// <typeparam name="T2">The type of the elements of the items being added.</typeparam>
        /// <param name="obj">The instance of the <see cref="T:System.Array"/>.</param>
        /// <param name="items">The items to be added.</param>
        /// <returns>Combined <see cref="T:System.Array"/> object.</returns>
        public static T1[] Combine<T1, T2>(this T1[] obj, params T2[] items)
        {
            return obj.Combine((IEnumerable<T2>)items);
        }

        /// <summary>
        /// Adds/combines given <see cref="T:IEnumerable&lt;T&gt;"/> object with the specified items.
        /// </summary>
        /// <typeparam name="T1">The type of the elements of <c>obj</c>.</typeparam>
        /// <typeparam name="T2">The type of the elements of the items being added.</typeparam>
        /// <param name="obj">The instance of the <see cref="T:System.Array"/>.</param>
        /// <param name="items">The items to be added.</param>
        /// <returns>Combined <see cref="T:System.Array"/> object.</returns>
        public static T1[] Combine<T1, T2>(this T1[] obj, IEnumerable<T2> items)
        {
            if (items != null)
            {
                var retval = new ArrayList();

                if (obj != null)
                    foreach (var i in obj)
                        retval.Add(i);

                foreach (var i in items)
                    retval.Add(i);

                return retval.Cast<T1>().ToArray();
            }
            return obj;
        }

        /// <summary>
        /// Combines given <see cref="T:System.Collections.Generic.List"/> items with items of
        /// another <see cref="T:System.Collections.Generic.List"/>.
        /// </summary>
        /// <typeparam name="T">The type of the elements of <c>obj</c>.</typeparam>
        /// <param name="obj">A <see cref="T:System.Collections.Generic.List"/>.</param>
        /// <param name="items">
        /// Another instance of <see cref="T:System.Collections.Generic.List"/> whose elements are
        /// to be combined with those of <c>obj</c>.
        /// </param>
        /// <returns>A combined <see cref="T:System.Collections.Generic.List"/>.</returns>
        public static List<T> Combine<T>(this List<T> obj, List<T> items)
        {
            if (items?.Count > 0)
                obj.AddRange(items);
            return obj;
        }

        /// <summary>
        /// Fluent version of the <see cref="T:System.String.IsNullOrEmpty"/> for analysing the
        /// string value for being <c>null</c> or empty.
        /// </summary>
        /// <param name="obj">A <see cref="T:System.String"/> whose value to analyse.</param>
        /// <returns>
        /// true if the value parameter is null or an empty string (""); otherwise, false.
        /// </returns>
        public static bool IsNullOrEmpty(this string obj)
        {
            return string.IsNullOrEmpty(obj);
        }

        /// <summary>
        /// Determines whether the specified <see
        /// cref="T:Microsoft.Deployment.WindowsInstaller.Session"/> is active.
        /// <para>
        /// It is useful for checking if the session is terminated (e.g. in deferred custom actions).
        /// </para>
        /// </summary>
        /// <param name="session">The session.</param>
        /// <returns></returns>
        public static bool IsActive(this Session session)
        {
            if (session.GetAttachedValue("active") != null)
                return session.GetAttachedValue<bool>("active");

            //if (!session.IsClosed) //unfortunately isClosed is always false even for the deferred actions
            try
            {
                var test = session.Components; //it will throw for the deferred action
                var text = session["INSTALLDIR"];
                session.SetAttachedValue("active", true);
                return true;
            }
            catch
            {
                session.SetAttachedValue("active", false);
                return false;
            }
        }

        public static bool IsDisconnected(this Session session)
        {
            if (session.GetAttachedValue("disconnected") != null)
                return session.GetAttachedValue<bool>("disconnected");

            try
            {
                // it will not throw for the deferred action
                // but will for the completely disconnected (fake) session that is created with
                // CreateDisconnectedSession()
                var test = session.CustomActionData.Count;
                session.SetAttachedValue("disconnected", false);
                return false;
            }
            catch
            {
                session.SetAttachedValue("disconnected", true);
                return true;
            }
        }

        /// <summary>
        /// Extracts the application data from the session object. Can be used in the deferred actions.
        /// </summary>
        /// <param name="session">The session.</param>
        /// <returns></returns>
        public static SetupEventArgs.AppData ExtractAppData(this Session session)
            => new AppData().InitFrom(session.Property("WIXSHARP_RUNTIME_DATA"));

        /// <summary>
        /// Saves AppData to the session objcet.
        /// </summary>
        /// <param name="data">The data.</param>
        /// <param name="session">The session.</param>
        public static void SaveTo(this SetupEventArgs.AppData data, Session session)
            => session["WIXSHARP_RUNTIME_DATA"] = data.ToString();

        /// <summary>
        /// Returns an instance of <see cref="SetupEventArgs"/> initialized from the <see cref="Session"/> data.
        /// <para>This method can only be called from immediate actions as it requires alive session object.</para>
        /// </summary>
        /// <param name="session">The session.</param>
        /// <returns></returns>
        public static SetupEventArgs ToEventArgs(this Session session)
        {
            ManagedProject.Init(session);
            return ManagedProject.Convert(session);
        }

        /// <summary>
        /// Determines whether the product associated with the session is installed.
        /// <para>
        /// This method will fail to retrieve the correct value if called from the deferred custom
        /// action and the session properties that it depends on are not preserved with
        /// 'UsesProperties' or 'DefaultUsesProperties'.
        /// </para>
        /// </summary>
        /// <param name="session">The session.</param>
        /// <returns></returns>
        public static bool IsInstalled(this Session session)
        {
            return session.Property("Installed").IsNotEmpty();
        }

        /// <summary>
        /// Gets a value indicating whether Authored UI and wizard dialog boxes suppressed.
        /// </summary>
        /// <value><c>true</c> if UI is suppressed; otherwise, <c>false</c>.</value>
        public static bool IsUISupressed(this Session session)
        {
            return session.UILevel() <= 4;
        }

        /// <summary>
        /// Returns path to the msi file being executed.
        /// </summary>
        /// <param name="session">The session.</param>
        /// <returns></returns>
        public static string MsiFile(this Session session)
        {
            return session.Property("OriginalDatabase");
        }

        /// <summary>
        /// Gets the UIlevel.
        /// <para>UILevel &gt; 4 lead to displaying modal dialogs. See https://msdn.microsoft.com/en-us/library/aa369487(v=vs.85).aspx.</para>
        /// </summary>
        /// <value>The UI level.</value>
        public static int UILevel(this Session session)
        {
            return session.Property("UILevel").ToInt(-1);
        }

        /// <summary>
        /// Gets a value indicating whether the MSI is running in "installing" mode.
        /// <para>
        /// This method will fail to retrieve the correct value if called from the deferred custom
        /// action and the session properties that it depends on are not preserved with
        /// 'UsesProperties' or 'DefaultUsesProperties'.
        /// </para>
        /// </summary>
        /// <value><c>true</c> if installing; otherwise, <c>false</c>.</value>
        static public bool IsInstalling(this Session session)
        {
            return !session.IsInstalled() && !session.IsUninstalling();
        }

        static internal bool IsCancelRequestedFromUI(this Session session)
        {
            string upgradeCode = session.Property("UpgradeCode");

            using (var m = new System.Threading.Mutex(true, "WIXSHARP_UI_CANCEL_REQUEST." + upgradeCode, out bool createdNew))
            {
                return (!createdNew);
            }
        }

        /// <summary>
        /// <para>
        /// Gets the main window of the <c>msiexec.exe</c> process that has 'MainWindowTitle'
        /// containing the name of the product being installed.
        /// </para>
        /// This method is a convenient way to display message box from a custom action with
        /// properly specified parent window.
        /// </summary>
        /// <param name="session">The session.</param>
        /// <returns></returns>
        public static IWin32Window GetMainWindow(this Session session) =>
            Tasks.GetMainWindow("msiexec", p => p.MainWindowTitle.Contains(session.Property("ProductName")));

        /// <summary>
        /// Gets a value indicating whether the MSI is running in "repair" mode.
        /// <para>
        /// This method will fail to retrieve the correct value if called from the deferred custom
        /// action and the session properties that it depends on are not preserved with
        /// 'UsesProperties' or 'DefaultUsesProperties'.
        /// </para>
        /// </summary>
        static public bool IsRepairing(this Session session)
        {
            //unfortunately experiments do not confirm the property values as they are in MSI documentation (below) for repairing scenario
            //so implementation is based on the experimental findings instead
            //bool p_Installed = session.Property("Installed").IsNotEmpty();
            //bool p_REINSTALL = session.Property("REINSTALL").IsNotEmpty();
            //bool p_UPGRADINGPRODUCTCODE = session.Property("UPGRADINGPRODUCTCODE").IsNotEmpty();

            return session.IsInstalled() && !session.Property("REMOVE").SameAs("ALL", true);
        }

        /// <summary>
        /// Gets a value indicating whether the product is being upgraded.
        /// <para>
        /// This method will fail to retrieve the correct value if called from the deferred custom
        /// action and the session properties (that it depends on) are not preserved with
        /// 'UsesProperties' or 'DefaultUsesProperties'.
        /// </para>
        /// <para>
        /// This method relies on "UPGRADINGPRODUCTCODE" property, which is not set by MSI until
        /// previous version is uninstalled. Thus it may not be the most practical way of detecting
        /// upgrades. Use AppSearch.GetProductVersionFromUpgradeCode as a more reliable alternative.
        /// </para>
        /// </summary>
        [Obsolete("This extension method is based on the MSI property 'UPGRADINGPRODUCTCODE' and it does not detect previous installations reliable. " +
                  "Thus use property 'IsUpgradingInstalledVersion' instead.")]
        static public bool IsUpgrading(this Session session)
        {
            return session.IsModifying() && session.Property("UPGRADINGPRODUCTCODE").IsNotEmpty();
        }

        /// <summary>
        /// Gets a value indicating whether this instance is upgrading installed version of the product.
        /// <para>
        /// This extension method is implemented on custom WixSharp algorithm that involves
        /// detection of the previously installed version of the product from the internal custom
        /// action (before `AppSearch`). Thus this algorithm is more reliable than traditional
        /// UPGRADINGPRODUCTCODE based algorithm.
        /// </para>
        /// <para>
        /// Because the custom action is specific to ManagedProject you can only use this extension
        /// from ManagedProject setups.
        /// </para>
        /// </summary>
        /// <value><c>true</c> if this instance is upgrading installed version; otherwise, <c>false</c>.</value>
        static public bool IsUpgradingInstalledVersion(this Session session)
        {
            return session.IsInstalling() && !session.IsModifying() && session.Property("FOUNDPREVIOUSVERSION").IsNotEmpty();
        }

        /// <summary>
        /// Determines whether the MSI is running in "modifying" mode.
        /// <para>
        /// This method will fail to retrieve the correct value if called from the deferred custom
        /// action and the session properties that it depends on are not preserved with
        /// 'UsesProperties' or 'DefaultUsesProperties'.
        /// </para>
        /// </summary>
        /// <param name="session">The session.</param>
        /// <returns></returns>
        static public bool IsModifying(this Session session)
        {
            return session.IsInstalled() && !session.Property("REINSTALL").SameAs("All", true);
        }

        /// <summary>
        /// Determines whether the MSI is running in "uninstalling" mode.
        /// <para>
        /// This method will fail to retrieve the correct value if called from the deferred custom
        /// action and the session properties that it depends on are not preserved with
        /// 'UsesProperties' or 'DefaultUsesProperties'.
        /// </para>
        /// </summary>
        /// <param name="session">The session.</param>
        /// <returns></returns>
        static public bool IsUninstalling(this Session session)
        {
            return session.Property("REMOVE").SameAs("All", true);
        }

        /// <summary>
        /// Gets the log file.
        /// </summary>
        /// <param name="session">The session.</param>
        /// <returns></returns>
        static public string GetLogFile(this Session session)
            => session.Property("MsiLogFileLocation");

        // static bool IsGlobalAssembly(this Assembly assembly) { try { if (assembly.Location.)
        // return session.Property("REMOVE").SameAs("All", true); } catch { return false; } }

        /// <summary>
        /// Returns the first string `value1` if it is not empty. Otherwise returns `value2`.
        /// </summary>
        /// <param name="value1">The value1.</param>
        /// <param name="value2">The value2.</param>
        /// <returns></returns>
        static public string IfEmptyUse(this string value1, string value2)
            => value1.IsEmpty() ? value2 : value1;

        /// <summary>
        /// Determines whether the feature is selected in the feature tree of the Features dialog
        /// and will be installed. The "selected" state of the feature is determined by analysing
        /// the `ADDLOCAL` session property, which has the required information either feature
        /// selected via UI or via msiexec.exe CLI arguments. If none of this selections is made by
        /// the user the method will return the default state of the feature (from session property "ADDFEATURES").
        /// </summary>
        /// <param name="session">The session.</param>
        /// <param name="featureName">Name of the feature.</param>
        /// <returns></returns>
        static public bool IsFeatureEnabled(this Session session, string featureName)
        {
            string defaultAddedFeatures = session.Property("ADDFEATURES");
            string requestedAddedFeatures = session.Property("ADDLOCAL");

            return requestedAddedFeatures.IfEmptyUse(defaultAddedFeatures)
                .Split(',')
                .Any(x => x.SameAs(featureName));
        }

        /// <summary>
        /// Sets `ADDLOCAL` session property to the coma-delimited values of the all features
        /// currently installed. This method is useful if you want to initialize current session
        /// with the specific features enabled according the previous installation if found.
        /// Applicable only for Major Upgrade scenarios.
        /// <para>
        /// Managed UI does this automatically but if you suppress UI then you may want to do this
        /// manually. Then this method is to do the heavy lifting.
        /// </para>
        /// </summary>
        /// <param name="session"></param>
        public static void InitFeaturesFromCurrentInstallation(this Session session)
        {
            var upgradeCode = session["UpgradeCode"];
            var installedPackage = ProductInstallation.GetRelatedProducts(upgradeCode).FirstOrDefault();
            if (installedPackage != null)
            {
                var installedFeatures = installedPackage.Features
                                                        .Where(x => x.State == InstallState.Local)
                                                        .Select(x => x.FeatureName)
                                                        .JoinBy(",");

                session["ADDLOCAL"] = installedFeatures;
            }
        }

        /// <summary>
        /// Builds an MSI condition expression for the given <see cref="WixSharp.Feature"/>, which
        /// evaluates as <c>true</c> if the feature is being installed.
        /// </summary>
        /// <param name="feature">The feature.</param>
        /// <returns></returns>
        public static Condition BeingInstall(this Feature feature)
        {
            return new Condition($"((!{feature.Id} = 2) AND (&{feature.Id} = 3))");
        }

        /// <summary>
        /// Creates a condition that checks if the feature shall be installed regardless of whether
        /// it already is intalled
        /// </summary>
        /// <param name="feature">The feature to check</param>
        public static Condition ShallInstall(this Feature feature) =>
            new Condition($"&{feature.Id}=3");

        /// <summary>
        /// Creates a condition that checks if the feature shall be uninstalled regardless of
        /// whether it already is intalled
        /// </summary>
        /// <param name="feature">The feature to check</param>
        public static Condition ShallUninstall(this Feature feature) =>
            new Condition($"&{feature.Id}=2");

        /// <summary>
        /// Agregate all <see cref="Feature"/> items.
        /// </summary>
        /// <param name="feature">The feature.</param>
        /// <returns></returns>
        public static Feature[] ToItems(this Feature feature)
        {
            if (feature == null)
                return new Feature[0];

            if (feature is FeatureSet feature_set)
                return feature_set.Items;
            else
                return new Feature[] { feature };
        }

        /// <summary>
        /// Builds an MSI condition expression for the given <see cref="WixSharp.Feature"/>, which
        /// evaluates as <c>true</c> if the feature is being uninstalled.
        /// </summary>
        /// <param name="feature">The feature.</param>
        /// <returns></returns>
        public static Condition BeingUninstall(this Feature feature)
        {
            return new Condition($"((&{feature.Id} = 2) AND (!{feature.Id} = 3))");
        }

        /// <summary>
        /// Determines whether this is basic UI level.
        /// </summary>
        /// <param name="level">The level.</param>
        /// <returns></returns>
        public static bool IsBasic(this InstallUIOptions level)
        {
            return (level & InstallUIOptions.Full) != InstallUIOptions.Full;
        }

        /////////////////////////////////////////////////////////////

        /// <summary>
        /// Returns the value of the named property of the specified <see
        /// cref="T:Microsoft.Deployment.WindowsInstaller.Session"/> object.
        /// <para>
        /// It can be uses as a generic way of accessing the properties as it redirects
        /// (transparently) access to the <see
        /// cref="T:Microsoft.Deployment.WindowsInstaller.Session.CustomActionData"/> if the session
        /// is terminated (e.g. in deferred custom actions).
        /// </para>
        /// </summary>
        /// <param name="session">The session.</param>
        /// <param name="name">The name.</param>
        /// <returns></returns>
        public static string Property(this Session session, string name)
        {
            if (session.IsActive())
            {
                return session[name];
            }
            else if (session.IsDisconnected())
            {
                Dictionary<string, string> extraProps = session.GetAttachedProperties();
                return extraProps.ContainsKey(name) ? extraProps[name] : "";
            }
            else
            {
                return (session.CustomActionData.ContainsKey(name) ? session.CustomActionData[name] : "");
            }
        }

        public static Dictionary<string, string> GetAttachedProperties(this Session session)
        {
            var extraProps = session.GetAttachedValue<Dictionary<string, string>>("SerializableProperties");
            if (extraProps == null)
                session.SetAttachedValue("SerializableProperties", extraProps = new Dictionary<string, string>());
            return extraProps;
        }

        public static void SetProperty(this Session session, string name, string value)
        {
            if (session.IsActive())
            {
                session[name] = value;
            }
            else if (session.IsDisconnected())
            {
                var extraProps = session.GetAttachedProperties();
                if (extraProps.ContainsKey(name))
                    extraProps[name] = value;
                else
                    extraProps.Add(name, value);
            }
            else
            {
                if (session.CustomActionData.ContainsKey(name))
                    session.CustomActionData[name] = value;
                else
                    session.CustomActionData.Add(name, value);
            }
        }

        /// <summary>
        /// Determines whether the specified session is cancelled.
        /// </summary>
        /// <param name="session">The session.</param>
        /// <returns><c>true</c> if the specified session is cancelled; otherwise, <c>false</c>.</returns>
        public static bool IsCancelled(this Session session)
        {
            try
            {
                session.Message(WixToolset.Dtf.WindowsInstaller.InstallMessage.ActionData, new Record());
            }
            catch (InstallCanceledException)
            {
                return true;
            }
            catch (Exception)
            {
                return false;
            }
            return false;
        }

        /// <summary>
        /// Determines whether the specified session is canceled.
        /// <para>
        /// It is identical to <see cref="WixSharp.Extensions.IsCancelled(Session)" /> except it does
        /// not throw/handle internal exception This helps if it is preferred to keep MSI log clean
        /// from any messages triggered by handled exceptions.
        /// </para><para>
        /// Though this method relies on <see cref="WixToolset.Dtf.WindowsInstaller" />
        /// internal (non-public) implementation thus is not warrantied to stay unchanged in the
        /// future WiX releases.
        /// </para>
        /// </summary>
        /// <param name="session">The session.</param>
        /// <returns>
        ///   <c>true</c> if the specified session is canceled; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsCancelledRaw(this Session session)
        {
            // does not throw but will become broken if WiX team changes the implementation
            long ActionData = 0x09000000;
            var RemotableNativeMethods = typeof(Session).Assembly
                                                .GetTypes()
                                                .FirstOrDefault(x => x.Name == "RemotableNativeMethods");

            var MsiProcessMessage = RemotableNativeMethods.GetMethods(BindingFlags.Static | BindingFlags.NonPublic)
                                                          .FirstOrDefault(x => x.Name == "MsiProcessMessage");

            var ret = (int)MsiProcessMessage.Invoke(null, new object[]
                                                    {
                                                        (int)session.Handle,
                                                        (uint)ActionData,
                                                        (int)new Record().Handle
                                                    });

            return (ret == (int)MessageResult.Cancel);
        }

        //============================

        /// <summary>
        /// Queries MSI database directly for the table 'Property' value. This method is
        /// particularly useful for the stages when WiX session object is not fully initialized. For
        /// example properties are not discovered yet during EmbeddedUI loading event.
        /// </summary>
        /// <param name="session">The session.</param>
        /// <param name="name">The name.</param>
        /// <returns></returns>
        public static string QueryProperty(this Session session, string name)
        {
            return (string)session.Database.ExecuteScalar($"SELECT `Value` FROM `Property` WHERE `Property` = '{name}'");
        }

        /// <summary>
        /// Lookups the installed version.
        /// </summary>
        /// <param name="session">The session.</param>
        /// <returns></returns>
        public static Version LookupInstalledVersion(this Session session)
        {
            return AppSearch.GetProductVersionFromUpgradeCode(session.QueryUpgradeCode());
        }

        /// <summary>
        /// Queries the upgrade code.
        /// </summary>
        /// <param name="session">The session.</param>
        /// <returns></returns>
        public static string QueryUpgradeCode(this Session session)
        {
            return session.QueryProperty("UpgradeCode");
        }

        /// <summary>
        /// Queries the product version.
        /// </summary>
        /// <param name="session">The session.</param>
        /// <returns></returns>
        public static Version QueryProductVersion(this Session session)
        {
            return new Version(session.QueryProperty("ProductVersion"));
        }

        internal static AppData InitFrom(this AppData data, Session session)
        {
            return data.InitFrom(session.Property("WIXSHARP_RUNTIME_DATA"));
        }

        internal static bool AbortOnError(this Session session)
        {
            string abortOnError = session.Property("WIXSHARP_ABORT_ON_ERROR");
            if (abortOnError.Any())
                return abortOnError == "True";
            else
                return true;
        }

        /// <summary>
        /// Saves the binary (from the Binary table) into the file.
        /// </summary>
        /// <param name="session">The session.</param>
        /// <param name="binary">The binary.</param>
        /// <param name="file">The file.</param>
        public static void SaveBinary(this Session session, string binary, string file)
        {
            //If binary is accessed the way as below it will raise "stream handle is not valid" exception
            //object result = session.Database.ExecuteScalar("select Data from Binary where Name = 'Fake_CRT.msi'");

            //however View approach is OK
            using (var sql = session.Database.OpenView("select Data from Binary where Name = '" + binary + "'"))
            {
                sql.Execute();

                using (var record = sql.Fetch())
                {
                    record.GetStream(1, file);
                }
            }
        }

        /// <summary>
        /// Tries the read the binary (from the Binary table) into the byte array.
        /// </summary>
        /// <param name="session">The session.</param>
        /// <param name="binary">The binary.</param>
        /// <returns></returns>
        public static byte[] TryReadBinary(this Session session, string binary)
        {
            try
            {
                return ReadBinary(session, binary);
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Read the binary (from the Binary table) into the byte array.
        /// </summary>
        /// <param name="session">The session.</param>
        /// <param name="binary">The binary.</param>
        /// <returns></returns>
        public static byte[] ReadBinary(this Session session, string binary)
        {
            return GetEmbeddedData(session, binary);
        }

        /// <summary>
        /// A simple generic wrapper around MSI View open operation. It retrieves all view data and
        /// returns it as a collection of dictionaries (set of named values).
        /// </summary>
        /// <param name="session">The session.</param>
        /// <param name="sqlText">The SQL text.</param>
        /// <returns></returns>
        public static List<Dictionary<string, object>> OpenView(this Session session, string sqlText)
        {
            var table = new List<Dictionary<string, object>>();

            using (var sql = session.Database.OpenView(sqlText))
            {
                sql.Execute();

                Record record;
                while ((record = sql.Fetch()) != null)
                    using (record)
                    {
                        var row = new Dictionary<string, object>();
                        foreach (var col in sql.Columns)
                            row[col.Name] = record[col.Name];

                        table.Add(row);
                    }
            }
            return table;
        }

        /// <summary>
        /// Extracts the bitmap embedded into MSI (into Binary table).
        /// </summary>
        /// <param name="session">The session.</param>
        /// <param name="binary">The name on resource in the Binary table.</param>
        /// <returns></returns>
        public static Bitmap GetEmbeddedBitmap(this Session session, string binary)
        {
            try
            {
                using (var sql = session.Database.OpenView("select Data from Binary where Name = '" + binary + "'"))
                {
                    sql.Execute();

                    using (var record = sql.Fetch())
                    {
                        if (record == null)
                            return null;

                        var stream = record.GetStream(1);

                        // It is important to keep the stream open even after the bitmap is created
                        // otherwise the bitmap will be disposed as soon as the stream is closed
                        // See:
                        //  https://stackoverflow.com/questions/1053052/a-generic-error-occurred-in-gdi-jpeg-image-to-memorystream
                        //  https://github.com/oleg-shilo/wixsharp/issues/1490
                        var ms = new IO.MemoryStream();
                        int Length = 256;
                        var buffer = new Byte[Length];
                        int bytesRead = stream.Read(buffer, 0, Length);
                        while (bytesRead > 0)
                        {
                            ms.Write(buffer, 0, bytesRead);
                            bytesRead = stream.Read(buffer, 0, Length);
                        }
                        ms.Seek(0, IO.SeekOrigin.Begin);

                        return (Bitmap)Bitmap.FromStream(ms);
                    }
                }
            }
            catch { }
            return null;
        }

        /// <summary>
        /// Extracts the string embedded into MSI (into Binary table).
        /// </summary>
        /// <param name="session">The session.</param>
        /// <param name="binary">The name on resource in the Binary table.</param>
        /// <returns></returns>
        public static string GetEmbeddedString(this Session session, string binary)
        {
            return GetEmbeddedData(session, binary).GetString();
        }

        /// <summary>
        /// Extracts the data embedded into MSI (into Binary table).
        /// </summary>
        /// <param name="session">The session.</param>
        /// <param name="binary">The name on resource in the Binary table.</param>
        /// <returns></returns>
        public static byte[] GetEmbeddedData(this Session session, string binary)
        {
            //If binary is accessed this way it will raise "stream handle is not valid" exception
            //object result = session.Database.ExecuteScalar("select Data from Binary where Name = 'Fake_CRT.msi'");
            //Stream s = (Stream)result;
            //using (FileStream fs = new FileStream(@"....\Wix# Samples\Simplified Bootstrapper\Fake CRT1.msi", FileMode.Create))
            //{
            //    int Length = 256;
            //    var buffer = new Byte[Length];
            //    int bytesRead = s.Read(buffer, 0, Length);
            //    while (bytesRead > 0)
            //    {
            //        fs.Write(buffer, 0, bytesRead);
            //        bytesRead = s.Read(buffer, 0, Length);
            //    }
            //}

            //however View approach is OK
            try
            {
                using (var sql = session.Database.OpenView("select Data from Binary where Name = '" + binary + "'"))
                {
                    sql.Execute();

                    using (var record = sql.Fetch())
                    {
                        if (record == null)
                            return null;

                        using (var stream = record.GetStream(1))
                        using (var ms = new IO.MemoryStream())
                        {
                            int Length = 256;
                            var buffer = new Byte[Length];
                            int bytesRead = stream.Read(buffer, 0, Length);
                            while (bytesRead > 0)
                            {
                                ms.Write(buffer, 0, bytesRead);
                                bytesRead = stream.Read(buffer, 0, Length);
                            }
                            ms.Seek(0, IO.SeekOrigin.Begin);
                            return ms.ToArray();
                        }
                    }
                }
            }
            catch { }
            return null;
        }

        /// <summary>
        /// Handles the errors in the specified action being executed. The all exceptions are caught
        /// and logged to the msi log.
        /// </summary>
        /// <param name="session">The session.</param>
        /// <param name="action">The action.</param>
        /// <returns>
        /// <see cref="T:Microsoft.Deployment.WindowsInstaller.ActionResult.Success"/> if no errors
        /// detected, otherwise it returns <see cref="T:Microsoft.Deployment.WindowsInstaller.ActionResult.Failure"/>.
        /// </returns>
        public static ActionResult HandleErrors(this Session session, System.Action action)
        {
            try
            {
                action();
            }
            catch (Exception e)
            {
                session.Log(e.ToString());
                return ActionResult.Failure;
            }
            return ActionResult.Success;
        }

        /// <summary>
        /// To a collection into WixObject that can be passed in the Project constructor.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="items">The items.</param>
        /// <returns></returns>
        public static WixObject ToWObject<T>(this IEnumerable<T> items) where T : WixObject
        {
            return new WixItems(items.Cast<WixObject>());
        }

        /// <summary>
        /// Gets a value of Description attribute from enum
        /// </summary>
        /// <param name="value">Enum value</param>
        /// <returns>Value of Description attribute or default</returns>
        public static string GetDescription(this Enum value)
        {
            var description = GetAttribute<DescriptionAttribute>(value);
            return description?.Description;
        }

        /// <summary>
        /// Gets an attribute from Enum value
        /// </summary>
        /// <param name="value">Enum value</param>
        /// <typeparam name="TAttribute">Attribute type</typeparam>
        /// <returns>Instance of specified attribute type or default</returns>
        public static TAttribute GetAttribute<TAttribute>(this Enum value)
            where TAttribute : Attribute
        {
            var type = value.GetType();
            var name = Enum.GetName(type, value);
            return type.GetField(name)
                .GetCustomAttributes(false)
                .OfType<TAttribute>()
                .SingleOrDefault();
        }
    }

    /// <summary>
    /// </summary>
    public static class LocalizationExtensions
    {
        /// <summary>
        /// Produces multilanguage MSI with embedded transformations based on <paramref name="localizations" /> collection. If this msi is executed on the OS, which language
        /// matches one of the embedded transformations, this transformation will be automatically
        /// triggered and effectively switch the setup UI language. Builds the localized msi.
        /// </summary>
        /// <param name="project">Wix# project.</param>
        /// <param name="defaultLocalization">Use your OS language as default localization. This will ensure that the all
        /// transformations are embedded in such a way that the produced msi can switch to any
        /// alternative language both automatically and manually.</param>
        /// <param name="torchPath">The torch path.</param>
        /// <param name="localizations">Collection of localizations. At least one localization is expected.</param>
        /// <returns>
        /// Path to the built MSI file.
        /// </returns>
        /// <exception cref="System.ArgumentNullException">
        /// project
        /// or
        /// defaultLocalization
        /// or
        /// localizations
        /// </exception>
        /// <exception cref="System.ArgumentException">At least one localization expected - localizations</exception>
        /// <exception cref="System.InvalidOperationException">Signing the file '{msiFilePath}' failed. Return code: {signingReturnCode}</exception>
        /// <example>
        /// The following is an example of building single .msi file with two localizations. With
        /// one of then based on a custom localization file.
        /// <p>
        /// During installation, language is automatically selected based on user's operating system
        /// region settings.
        /// </p><code>
        /// project.BuildMultilanguageMsi(
        /// new ProjectLocalization("en-US"),
        /// new ProjectLocalization("sk-SK", "WixUI_sk-SK.wxl"));
        /// </code></example>
        public static string BuildMultilanguageMsi(this Project project, ProjectLocalization defaultLocalization, string torchPath, params ProjectLocalization[] localizations)
        {
            if (project is null)
                throw new ArgumentNullException(nameof(project));

            if (defaultLocalization is null)
                throw new ArgumentNullException(nameof(defaultLocalization));

            if (localizations is null)
                throw new ArgumentNullException(nameof(localizations));

            if (!localizations.Any())
                throw new ArgumentException("At least one localization expected", nameof(localizations));

            defaultLocalization.BindTo(project);

            var msiFilePath = project.BuildMsi();
            var preserveTempFiles = project.PreserveTempFiles;

            foreach (var localization in localizations)
            {
                if (localization is null)
                    continue;

                localization.BindTo(project);

                Compiler.OutputWriteLine($"> Building msi for {localization.Language}...");
                var localizedMsiFilePath = project.BuildMsi(localization.Language);
                var localizedMstFilePath = localizedMsiFilePath.PathChangeExtension(".mst");

                Compiler.OutputWriteLine("> Preparing language transformations...");
                Process.Start(torchPath, $"-p -t language \"{msiFilePath}\" \"{localizedMsiFilePath}\" -out \"{localizedMstFilePath}\"")
                       .WaitForExit();

                msiFilePath.EmbedTransform(localizedMstFilePath);

                if (!preserveTempFiles)
                {
                    localizedMsiFilePath.DeleteIfExists();
                    localizedMstFilePath.DeleteIfExists();
                }
            }

            var langCodeIds = $"{defaultLocalization.LanguageCodeId}," + localizations.Select(i => i.LanguageCodeId.ToString()).JoinBy(",");
            msiFilePath.SetPackageLanguages(langCodeIds);

            if (project.DigitalSignature != null)
            {
                // sign the MSI after embedding transforms again (as it won't contain publisher info
                // in UAC dialog)
                var signingReturnCode = project.DigitalSignature.Apply(msiFilePath);
                if (signingReturnCode != 0)
                    throw new InvalidOperationException($"Signing the file '{msiFilePath}' failed. Return code: {signingReturnCode}");
            }

            Compiler.OutputWriteLine($"> Multi-language setup {msiFilePath} is completed.");
            return msiFilePath;
        }

        /// <summary>
        /// Builds the localized msi.
        /// <para>
        /// This method builds the msi with the default language support according
        /// `project.Language` setting. The additional languages can also be embedded into the
        /// resulting msi during the build.
        /// </para><para>
        /// Invoking specific language UI is triggered either automatically based on the OS default
        /// language or by passing special MSI properties arguments to the <c>msiexec.exe</c>:
        /// </para>
        /// <list type="bullet">
        ///   <item><description><i>English</i><para><c>msiexec /i setup.msi</c></para></description></item>
        ///   <item><description><i>German</i><para><c>msiexec /i setup.msi TRANSFORMS=:1031</c></para></description>
        ///   </item><item><description><i>Ukrainian</i><para><c>msiexec /i setup.msi TRANSFORMS=:1058</c></para></description></item>
        /// </list>
        /// </summary>
        /// <param name="project">The project.</param>
        /// <param name="languages">The additional languages to embed. The value is a string of the coma-separated
        /// language identifiers (e.g. `en-US,uk-UA,de-DE`). The first identifier will be used as the default language
        /// of the msi to build. Any value of <see cref="WixSharp.WixProject.Language"/> wil be ignored.</param>
        /// <param name="path">The path.</param>
        /// <returns>
        /// Path to the built MSI file.
        /// </returns>
        /// <example>
        /// The following is an example of building <c>English</c> msi, which can also support
        /// <c>German</c> and <c>Ukrainian</c> UI.
        /// <code>
        /// var project =
        ///     new ManagedProject("My Product",
        ///         new Dir(@"%ProgramFiles%\My Company\My Product",
        ///             new File("readme.txt")));
        ///
        /// project.GUID = new Guid("6f330b47-2577-43ad-9095-1861bb258777");
        /// project.BuildMultilanguageMsiFor(en-US,uk-UA,de-DE);
        /// </code></example>
        static public string BuildMultilanguageMsiFor(this WixSharp.Project project, string languages, string path = null)
        {
            project.VerifyLanguage();

            var originalLanguages = project.Language;
            try
            {
                var allLanguages = languages.Split(',', ';').Select(x => x.Trim());
                var additionalLanguages = allLanguages.Skip(1);

                Compiler.OutputWriteLine("> Building msi with the default language...");
                project.Language = allLanguages.FirstOrDefault();
                string productMsi = project.BuildMsi(path);

                Compiler.OutputWriteLine("> Preparing language transformations...");

                foreach (string lang in additionalLanguages)
                {
                    string mstFile = project.BuildLanguageTransform(productMsi, lang);
                    productMsi.EmbedTransform(mstFile);
                }

                productMsi.SetPackageLanguagesFrom(project);

                // -------------------------------
                Compiler.OutputWriteLine($"> Multi-language setup {productMsi} is completed.");
                if (originalLanguages != project.Language)
                    Compiler.OutputWriteLine(
                        $"Warning: The project language `{originalLanguages}` is different to the " +
                        $"first language specified in the transformations list `{languages}`. " +
                        $"The project language value will be ignored and `{allLanguages.FirstOrDefault()}` will be used instead.");

                Compiler.OutputWriteLine($"> {allLanguages.FirstOrDefault()} (default): \"msiexec /i {productMsi.PathGetFileName()}\"");
                foreach (string lang in additionalLanguages)
                    Compiler.OutputWriteLine($"> {lang}          : \"msiexec /i {productMsi.PathGetFileName()} TRANSFORMS=:{new CultureInfo(lang).LCID}\"");
                // -------------------------------

                return productMsi;
            }
            finally
            {
                project.Language = originalLanguages;
            }
        }

        /// <summary>
        /// Determines whether two cultures are compatible based on the same neutral culture (e.g.
        /// en-US is compatible with en-AU).
        /// </summary>
        /// <param name="info1">The info1.</param>
        /// <param name="info2">The info2.</param>
        /// <returns><c>true</c> if [is compatible with] [the specified info2]; otherwise, <c>false</c>.</returns>
        public static bool IsCompatibleWith(this CultureInfo info1, CultureInfo info2)
            => info1.TwoLetterISOLanguageName == info2.TwoLetterISOLanguageName;

        /// <summary>
        /// Verifies the language of the multi-language project being compatible with the language
        /// of the operating system the project is being built on. The compatibility is important in
        /// this case as the wrong choice of the default language can lead to the problems when
        /// building bootstrapper with the language selection. See
        /// "&lt;root&gt;\Source\src\WixSharp.Samples\Wix#
        /// Samples\Bootstrapper\MultiLanguageSupport\setup.cs&gt; sample.
        /// </summary>
        /// <param name="project">The project.</param>
        public static void VerifyLanguage(this Project project)
        {
            if (project.IsMultiLanguage && !project.DefaultLanguage.IsCompatibleWith(CultureInfo.InstalledUICulture))
                Console.WriteLine(
                    $"Warning: Your project default language ({project.DefaultLanguage.IetfLanguageTag}) " +
                    $"does not match the OS language ({CultureInfo.InstalledUICulture.IetfLanguageTag}). " +
                    $"This may lead to unexpected behavior when building multi-language packages (see Wiki-Localization article).\n" +
                    $"Normally you would prefer your project default language to be the same as your OS language.");
        }

        /// <summary>
        /// Builds a language transform (*.mst) file for a given msi file and its `Project`.
        /// <para>
        /// This method is not intended to be used directly (even though it's possible). The
        /// developers are encouraged to use a <see cref="LocalizationExtensions.BuildMultilanguageMsiFor(Project, string, string)" />
        /// instead.
        /// </para>
        /// </summary>
        /// <param name="project">The project.</param>
        /// <param name="originalMsi">The original msi.</param>
        /// <param name="language">The language.</param>
        /// <param name="localizationFile">The localization file.</param>
        /// <returns></returns>
        public static string BuildLanguageTransform(this Project project, string originalMsi, string language, string localizationFile = "")
        {
            var originalLng = project.Language;
            var originalLocalizationFile = project.LocalizationFile;
            try
            {
                project.Language = language;
                project.LocalizationFile = localizationFile;

                string localizedMsi = project.BuildMsi(language).PathGetFullPath();
                string langMst = localizedMsi.PathChangeExtension(".mst");

                "wix.exe".Run($"msi transform -p -t language \"{originalMsi}\" \"{localizedMsi}\" -out \"{langMst}\"");

                return langMst;
            }
            finally
            {
                project.LocalizationFile = originalLocalizationFile;
                project.Language = originalLng;
            }
        }

        static int Run(this string exe, string args)
            => new ExternalTool { ExePath = exe, Arguments = args }.ConsoleRun();
    }

    /// <summary>
    /// </summary>
    static class SystemExtensions
    {
        /// <summary>
        /// Converts semantic version to <see cref="System.Version"/>.
        /// </summary>
        /// <param name="semanticVersion">The semantic version.</param>
        /// <returns></returns>
        public static Version SemanticVersionToVersion(this string semanticVersion)
        {
            // need to handle cases like this: 1.0.0-beta.1, 5.0.0+41e11442
            var parts = semanticVersion.Split('.');
            int major = 0, minor = 0, build = 0;

            if (parts.Length > 0) int.TryParse(parts[0], out major);
            if (parts.Length > 1) int.TryParse(parts[1], out minor);
            if (parts.Length > 2) int.TryParse(new string(parts[2].TakeWhile(char.IsDigit).ToArray()), out build);

            return new Version(major, minor, build);
        }
    }

    /// <summary>
    /// Extension methods for <see cref="System.Diagnostics.Process"/>
    /// </summary>
    public static class ProcessExtensions
    {
        /// <summary>
        /// Starts a process resource by specifying the name of an application and a set of
        /// command-line arguments, and associates the resource with a new
        /// System.Diagnostics.Process component.
        /// </summary>
        /// <param name="fileName">Gets or sets the application or document to start.</param>
        /// <param name="args">
        /// Gets or sets the set of command-line arguments to use when starting the application.
        /// </param>
        /// <returns>
        /// A new <see cref="System.Diagnostics.Process"/> component that is associated with the
        /// process, or null, if no process resource is started (for example, if an existing process
        /// is reused).
        /// </returns>
        public static Process StartElevated(this string fileName, string args = "")
        {
            bool alreadyAdmin = new WindowsPrincipal(WindowsIdentity.GetCurrent()).IsInRole(WindowsBuiltInRole.Administrator);

            return Process.Start(new ProcessStartInfo
            {
                WorkingDirectory = fileName.PathGetFullPath().PathGetDirName(),
                FileName = fileName,
                Arguments = args,
                UseShellExecute = true,
                Verb = alreadyAdmin ? "" : "runas"
            });
        }

        /// <summary>
        /// Starts a process resource by specifying the name of an application and a set of
        /// command-line arguments, and associates the resource with a new
        /// System.Diagnostics.Process component.
        /// </summary>
        /// <param name="fileName">Gets or sets the application or document to start.</param>
        /// <param name="args">
        /// Gets or sets the set of command-line arguments to use when starting the application.
        /// </param>
        /// <returns>
        /// A new <see cref="System.Diagnostics.Process"/> component that is associated with the
        /// process, or null, if no process resource is started (for example, if an existing process
        /// is reused).
        /// </returns>
        public static Process Start(this string fileName, string args = "")
        {
            return Process.Start(new ProcessStartInfo
            {
                WorkingDirectory = fileName.PathGetFullPath().PathGetDirName(),
                FileName = fileName,
                Arguments = args,
            });
        }

        /// <summary>
        /// Instructs the <see cref="System.Diagnostics.Process"/> component to wait indefinitely
        /// for the associated process to exit.
        /// </summary>
        /// <param name="process">
        /// An instance of <see cref="System.Diagnostics.Process"/> component that is running.
        /// </param>
        /// <returns>
        /// An instance of <see cref="System.Diagnostics.Process"/> component that is running.
        /// </returns>
        public static Process Wait(this Process process)
        {
            process.WaitForExit();
            return process;
        }

        /// <summary>
        /// Deconstructs the collection into the tuple.
        /// <para>Beautiful solution of: https://stackoverflow.com/questions/49190830/is-it-possible-for-string-split-to-return-tuple
        /// </para>
        /// </summary>
        /// <example>
        /// <code>
        /// var line = "a=b";
        /// var (first, second, _) = line.Split('=');
        /// var (first, (second, _)) = line.Split('=');
        /// var(first, (second, (third, _))) = line.Split('=');
        /// var(first, rest) = line.Split('=');
        /// </code>
        /// </example>
        /// <typeparam name="T"></typeparam>
        /// <param name="list">The list.</param>
        /// <param name="first">The first.</param>
        /// <param name="rest">The rest.</param>
        public static void Deconstruct<T>(this IList<T> list, out T first, out IList<T> rest)
        {
            first = list.Count > 0 ? list[0] : default(T); // or throw
            rest = list.Skip(1).ToList();
        }

        /// <summary>
        /// Deconstructs the collection into the tuple.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list">The list.</param>
        /// <param name="first">The first.</param>
        /// <param name="second">The second.</param>
        /// <param name="rest">The rest.</param>
        public static void Deconstruct<T>(this IList<T> list, out T first, out T second, out IList<T> rest)
        {
            first = list.Count > 0 ? list[0] : default(T); // or throw
            second = list.Count > 1 ? list[1] : default(T); // or throw
            rest = list.Skip(2).ToList();
        }
    }
}