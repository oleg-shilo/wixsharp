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

#pragma warning disable 1591

namespace WixSharp
{
    /// <summary>
    /// Serializer for the MSI session data.
    /// </summary>
    public static class SerializingExtensions
    {
        /// <summary>
        ///
        /// </summary>
        public class Property
        {
            public string Key { get; set; }
            public string Value { get; set; }
        }

        public static Session DeserializeAndUpdateFrom(this Session session, string data)
        {
            using (TextReader reader = new StringReader(data))
            {
                List<Property> propList = (List<Property>)new XmlSerializer(typeof(List<Property>)).Deserialize(reader);
                foreach (Property prop in propList)
                {
                    session.SetProperty(prop.Key, prop.Value);
                }
            }

            return session;
        }

        internal static string Serialize(this Session session, params string[] extraProperties)
        {
            // connected session does not offer any mechanism for exploring properties.
            // On needs to know exactly properties names.
            // Thus we are using a fixed list of properties to serialize.
            // Though the deferred session will have some data in the session.CustomActionData,
            // which can be explored.
            var properties = new List<string>(new[]
            {
                "WIXBUNDLEORIGINALSOURCE",
                "FOUNDPREVIOUSVERSION",
                "WIXSHARP_RUNTIME_DATA",
                "MsiLogFileLocation",
                "ADDFEATURES",
            });
            properties.AddRange(ManagedProject.SessionSerializableProperties);
            properties.AddRange(extraProperties);

            properties = properties.Distinct().ToList();

            if (!session.IsDisconnected())
            {
                // and session.Property(..) will be able to extract data for any session internal
                // stores: session[..], session.CustomActionData[..], session.GetAttachedProperties()[..]
                properties.AddRange(session.CustomActionData.Keys);
            }

            // aggregate connected session properties if any
            List<Property> propList = properties
                .Select(x => new Property { Key = x, Value = session.Property(x) })
                .Where(x => x.Value.IsNotEmpty())
                .ToList();

            // aggregate disconnected session properties if any
            propList.AddRange(
                session.GetAttachedProperties()
                    .Select(x => new Property { Key = x.Key, Value = x.Value })
                    .Where(x => x.Value.IsNotEmpty())
                    .ToList());

            var serializer = new XmlSerializer(propList.GetType());
            using (var writer = new StringWriter())
            {
                serializer.Serialize(writer, propList);
                string xmlString = writer.ToString();
                return xmlString;
            }
        }

        /// <summary>
        /// Decodes hexadecimal string representation into the byte array.
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <returns></returns>
        public static byte[] DecodeFromHex(this string obj)
        {
            var data = new List<byte>();
            for (int i = 0; !string.IsNullOrEmpty(obj) && i < obj.Length;)
            {
                if (obj[i] == ',')
                {
                    i++;
                    continue;
                }
                data.Add(byte.Parse(obj.Substring(i, 2), System.Globalization.NumberStyles.HexNumber));
                i += 2;
            }
            return data.ToArray();
        }

        /// <summary>
        /// Encodes byte array into its hexadecimal string representation.
        /// </summary>
        /// <param name="data">The data.</param>
        /// <returns></returns>
        public static string EncodeToHex(this byte[] data)
        {
            return BitConverter.ToString(data).Replace("-", string.Empty);
        }

        /// <summary>
        /// Converts bytes into text according the specified Encoding..
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <param name="encoding">The encoding.</param>
        /// <returns></returns>
        public static string GetString(this byte[] obj, Encoding encoding = null)
        {
            if (obj == null) return null;
            if (encoding == null)
                return Encoding.Default.GetString(obj);
            else
                return encoding.GetString(obj);
        }

        /// <summary>
        /// Gets the bytes of the text according the specified Encoding.
        /// </summary>
        /// <param name="obj">The text.</param>
        /// <param name="encoding">The encoding.</param>
        /// <returns></returns>
        public static byte[] GetBytes(this string obj, Encoding encoding = null)
        {
            if (encoding == null)
                return Encoding.Default.GetBytes(obj);
            else
                return encoding.GetBytes(obj);
        }

        /// <summary>
        /// Converts a string in Base64 encoding.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <returns></returns>
        public static string Base64Encode(this string text)
            => Convert.ToBase64String((text ?? "").GetBytes());

        /// <summary>
        /// Decodes Base64 data into a string..
        /// </summary>
        /// <param name="data">The data.</param>
        /// <returns></returns>
        public static string Base64Decode(this string data)
            => Convert.FromBase64String(data ?? "").GetString();
    }
}