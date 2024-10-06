extern alias WixSharpMsi;

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml.Serialization;
using WixToolset.Dtf.WindowsInstaller;
using Xunit;
using WixMsi = WixSharpMsi::WixSharp;

namespace WixSharp.Test
{
    public class SerializationTest
    {
        [Fact]
        public void DisconnectedSession_CanManage_Properties()
        {
            Session session = DisconnectedSession.Create();
            session.SetProperty("propName", "test");

            // property that does not exist
            var actualValue = session.Property("fake propName");
            Assert.Equal("", actualValue);

            // property that does exist
            actualValue = session.Property("propName");
            Assert.Equal("test", actualValue);
        }

        [Fact]
        public void Serialize()
        {
            Session session = DisconnectedSession.Create();

            session.SetProperty("test", "ttt");

            var xml = session.Serialize();

            Assert.Equal(
@"<?xml version=""1.0"" encoding=""utf-16""?>
<ArrayOfProperty xmlns:xsd=""http://www.w3.org/2001/XMLSchema"" xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"">
  <Property>
    <Key>test</Key>
    <Value>ttt</Value>
  </Property>
</ArrayOfProperty>", xml);
        }

        [Fact]
        public void Deserialize()
        {
            Session session = DisconnectedSession.Create();

            var xml = @"<?xml version=""1.0"" encoding=""utf-16""?>
<ArrayOfProperty xmlns:xsd=""http://www.w3.org/2001/XMLSchema"" xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"">
  <Property>
    <Key>test</Key>
    <Value>ttt</Value>
  </Property>
</ArrayOfProperty>";

            session.DeserializeAndUpdateFrom(xml);

            Assert.Equal(@"ttt", session.Property("test"));
        }
    }
}