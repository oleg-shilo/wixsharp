#region Licence...
/*
The MIT License (MIT)
Copyright (c) 2015 Oleg Shilo
Permission is hereby granted, 
free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:
The above copyright notice and this permission notice shall be included in
all copies or substantial portions of the Software.
THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
THE SOFTWARE.
*/
#endregion
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Text.RegularExpressions;

namespace WixSharp
{
    internal static class RegFileImporter
    {
        public static bool SkipUnknownTypes = false;

        static public RegValue[] ImportFrom(string regFile)
        {
            var result = new List<RegValue>();

            string content = System.IO.File.ReadAllText(regFile);

            var parser = new RegParser();

            char[] delimiter = new[] { '\\' };

            foreach (KeyValuePair<string, Dictionary<string, string>> entry in parser.Parse(content))
                foreach (KeyValuePair<string, string> item in entry.Value)
                {
                    string path = entry.Key;

                    var regval = new RegValue();
                    regval.Root = GetHive(path);
                    regval.Key = path.Split(delimiter, 2).Last();
                    regval.Name = item.Key;
                    regval.Value = Deserialize(item.Value, parser.Encoding);

                    if (regval.Value != null)
                        result.Add(regval);
                }

            return result.ToArray();
        }

        /*
         hex: REG_BINARY
         hex(0): REG_NONE
         hex(1): REG_SZ (WiX string)
         hex(2): REG_EXPAND_SZ (Wix expandable)
         hex(3): REG_BINARY (WiX binary)
         hex(4): REG_DWORD (WiX integer)
         hex(5): REG_DWORD_BIG_ENDIAN ; invalid type ?
         hex(6): REG_LINK
         hex(7): REG_MULTI_SZ (WiX multiString)
         hex(8): REG_RESOURCE_LIST
         hex(9): REG_FULL_RESOURCE_DESCRIPTOR
         hex(a): REG_RESOURCE_REQUIREMENTS_LIST
         hex(b): REG_QWORD
         */
        public static object Deserialize(string text, Encoding encoding)
        {
            //Note all string are encoded as Encoding.Unicode (UTF16LE) 
            //http://en.wikipedia.org/wiki/Windows_Registry
            string rawValue = "";

            Func<string, bool> isPreffix = preffix =>
                {
                    if (text.StartsWith(preffix, StringComparison.OrdinalIgnoreCase))
                    {
                        rawValue = text.Substring(preffix.Length);
                        return true;
                    }
                    else
                        return false;
                };

            //WiX 'integer'
            if (isPreffix("hex(4):") || isPreffix("dword:"))
                return Convert.ToInt32(rawValue, 16);

            //WiX 'multiString'
            if (isPreffix("hex(7):"))
            {
                byte[] data = rawValue.Unescape().DecodeFromRegHex();
                return encoding.GetString(data).TrimEnd('\0').Replace("\0", "\r\n");
            }

            //WiX 'expandable'
            if (isPreffix("hex(2):"))
            {
                byte[] data = rawValue.Unescape().DecodeFromRegHex();
                return encoding.GetString(data).TrimEnd('\0');
            }

            //WiX 'binary'
            if (isPreffix("hex:"))
            {
                byte[] data = rawValue.Unescape().DecodeFromRegHex();
                return data;
            }

            //WiX 'string'
            if (isPreffix("hex(1):"))
                return rawValue.Unescape();

            if (isPreffix("\""))
            {
                var strValue = rawValue.Substring(0, rawValue.Length-1); //trim a single " char
                return Regex.Unescape(strValue);
            }

            if (SkipUnknownTypes)
                return null;
            else
                throw new Exception("Cannot deserialize RegFile value: '" + text + "'");
        }

        public static byte[] DecodeFromRegHex(this string obj)
        {
            var data = new List<byte>();
            for (int i = 0; !string.IsNullOrEmpty(obj) && i < obj.Length; )
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

        static string Unescape(this string text)
        {
            //Strip  'continue' char and merge string 
            return Regex.Replace(text, "\\\\\r\n[ ]*", string.Empty);
        }

        static RegistryHive GetHive(this string skey)
        {
            string tmpLine = skey.Trim();

            if (tmpLine.StartsWith("HKEY_LOCAL_MACHINE\\"))
                return RegistryHive.LocalMachine;

            if (tmpLine.StartsWith("HKEY_CLASSES_ROOT\\"))
                return RegistryHive.ClassesRoot;

            if (tmpLine.StartsWith("HKEY_USERS\\"))
                return RegistryHive.Users;

            if (tmpLine.StartsWith("HKEY_CURRENT_CONFIG\\"))
                return RegistryHive.CurrentConfig;

            if (tmpLine.StartsWith("HKEY_CURRENT_USER\\"))
                return RegistryHive.CurrentUser;

            throw new Exception("Cannot extract hive from key path: " + skey);
        }
    }

    /// <summary>
    /// Based on Henryk Filipowicz work http://www.codeproject.com/Articles/125573/Registry-Export-File-reg-Parser
    /// Licensed under The Code Project Open License (CPOL)
    /// </summary>
    internal class RegParser
    {
        public Encoding Encoding;
        public Dictionary<string, Dictionary<string, string>> Entries;

        /// <summary>
        /// Parses the reg file for reg keys and reg values
        /// </summary>
        /// <returns>A Dictionary with reg keys as Dictionary keys and a Dictionary of (valuename, valuedata)</returns>
        public Dictionary<string, Dictionary<string, string>> Parse(string content)
        {
            Encoding = Encoding.GetEncoding(this.GetEncoding(content));

            var retValue = new Dictionary<string, Dictionary<string, string>>();

            //Get registry keys and values content string
            Dictionary<string, string> dictKeys = NormalizeDictionary("^[\t ]*\\[.+\\][\r\n]+", content, true);

            //Get registry values for a given key
            foreach (KeyValuePair<string, string> item in dictKeys)
            {
                Dictionary<string, string> dictValues = NormalizeDictionary("^[\t ]*(\".+\"|@)=", item.Value, false);
                retValue.Add(item.Key, dictValues);
            }
            return Entries = retValue;
        }

        /// <summary>
        /// Creates a flat Dictionary using given search pattern
        /// </summary>
        /// <param name="searchPattern">The search pattern</param>
        /// <param name="content">The content string to be parsed</param>
        /// <param name="stripeBraces">Flag for striping braces (true for reg keys, false for reg values)</param>
        /// <returns>A Dictionary with retrieved keys and remaining content</returns>
        Dictionary<string, string> NormalizeDictionary(string searchPattern, string content, bool stripeBraces)
        {
            MatchCollection matches = Regex.Matches(content, searchPattern, RegexOptions.Multiline);

            Int32 startIndex = 0;
            Int32 lengthIndex = 0;
            var dictKeys = new Dictionary<string, string>();

            foreach (Match match in matches)
            {
                try
                {
                    //Retrieve key
                    string sKey = match.Value;
                    while (sKey.EndsWith("\r\n"))
                        sKey = sKey.Substring(0, sKey.Length - 2);
                    if (sKey.EndsWith("=")) sKey = sKey.Substring(0, sKey.Length - 1);
                    if (stripeBraces) sKey = StripeBraces(sKey);
                    if (sKey == "@")
                        sKey = "";
                    else
                        sKey = StripeLeadingChars(sKey, "\"");

                    //Retrieve value
                    startIndex = match.Index + match.Length;
                    Match nextMatch = match.NextMatch();
                    lengthIndex = ((nextMatch.Success) ? nextMatch.Index : content.Length) - startIndex;
                    string sValue = content.Substring(startIndex, lengthIndex);
                    //Removing the ending CR
                    while (sValue.EndsWith("\r\n"))
                        sValue = sValue.Substring(0, sValue.Length - 2);
                    dictKeys.Add(sKey, sValue);
                }
                catch (Exception ex)
                {
                    throw new Exception(string.Format("Exception thrown on processing string {0}", match.Value), ex);
                }
            }
            return dictKeys;
        }

        /// <summary>
        /// Removes the leading and ending characters from the given string
        /// </summary>
        /// <param name="sLine">given string</param>
        /// <param name="leadChar">The lead character.</param>
        /// <returns>
        /// edited string
        /// </returns>
        string StripeLeadingChars(string sLine, string leadChar)
        {
            string tmpvalue = sLine.Trim();
            if (tmpvalue.StartsWith(leadChar) & tmpvalue.EndsWith(leadChar))
            {
                return tmpvalue.Substring(1, tmpvalue.Length - 2);
            }
            return tmpvalue;
        }

        /// <summary>
        /// Removes the leading and ending parenthesis from the given string
        /// </summary>
        /// <param name="sLine">given string</param>
        /// <returns>edited string</returns>
        /// <remarks></remarks>
        string StripeBraces(string sLine)
        {
            string tmpvalue = sLine.Trim();
            if (tmpvalue.StartsWith("[") & tmpvalue.EndsWith("]"))
            {
                return tmpvalue.Substring(1, tmpvalue.Length - 2);
            }
            return tmpvalue;
        }

        /// <summary>
        /// Retrieves the encoding of the reg file, checking the word "REGEDIT4"
        /// </summary>
        /// <returns></returns>
        string GetEncoding(string content)
        {
            if (Regex.IsMatch(content, "([ ]*(\r\n)*)REGEDIT4", RegexOptions.IgnoreCase | RegexOptions.Singleline))
                return "ANSI";
            else
                return "Unicode"; //The original code had a mistake. It is not UTF8 but UTF16LE (Encoding.Unicode) 
        }
    }
}
