using System;
using System.Collections.Generic;
using System.Xml.Linq;
using WixSharp.UI.ManagedUI;
using Xunit;

namespace WixSharp.Test
{
    public class ManagedProjectTest
    {
        [Fact]
        public void CanHandle_ResourceEncoding_Test()
        {
            //WixUI_en_us is a WiX source file that is just added to Wix# codebase as resource.
            //This file can easily come with the wrong encoding. So we need to unsure it can be parsed
            //during the installation.
            var xml = Resources.WixUI_en_us.GetString(System.Text.Encoding.UTF8);
            XDocument.Parse(xml);
        }

        [Fact]
        public void Can_Localize_UIString()
        {
            var resources = new Dictionary<string, string> { { "aaa", "AAA" }, { "bbb", "BBB [ccc] DDD" }, { "ccc", "CCC" } };
            Func<string, string> localizer =
                key =>
                {
                    if (resources.ContainsKey(key))
                        return resources[key];
                    else
                        return key;
                };

            var text = "123 [aaa] 321 [bbb]";

            var locText = text.LocalizeWith(localizer);

            Assert.Equal("123 AAA 321 BBB CCC DDD", locText);
        }
    }
}