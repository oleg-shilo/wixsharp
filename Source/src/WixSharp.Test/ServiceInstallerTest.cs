using System.Linq;
using System.Xml.Linq;
using Xunit;

namespace WixSharp.Test
{
    public class ServiceInstallerTest
    {
        Project projectMock = new Project();
        
        //[Fact]
        //public void Should_Emit_FullSetOfMembers()
        //{
        //    var service = new ServiceInstaller
        //    {
        //        Name = "WixSharp.TestSvc",
        //        DisplayName = "WixSharp TestSvc",
        //        Description = "ServiceDescription",
        //        DependsOn = "Dnscache;Dhcp",
        //        Account = "NT AUTHORITY\\LocalService",
        //        Arguments = "a b c",
        //        Password = "Password",
        //        LoadOrderGroup = "LoadOrderGroup",
        //        Vital = true,
        //        StartOn = SvcEvent.Install,
        //        StopOn = SvcEvent.InstallUninstall_Wait,
        //        RemoveOn = SvcEvent.Uninstall_Wait
        //    };

        //    var all = service.ToXml(projectMock).Cast<XElement>().ToArray();

        //    var install = all[0];
        //    Assert.Equal("ServiceInstall", install.Name.LocalName);
        //    Assert.Equal("WixSharp.TestSvc", install.Attribute("Name").Value);
        //    Assert.Equal("WixSharp TestSvc", install.Attribute("DisplayName").Value);
        //    Assert.Equal("ServiceDescription", install.Attribute("Description").Value);
        //    Assert.Equal("NT AUTHORITY\\LocalService", install.Attribute("Account").Value);
        //    Assert.Equal("a b c", install.Attribute("Arguments").Value);
        //    Assert.Equal("Password", install.Attribute("Password").Value);
        //    Assert.Equal("LoadOrderGroup", install.Attribute("LoadOrderGroup").Value);
        //    Assert.Equal("yes", install.Attribute("Vital").Value);
        //    Assert.Equal("ownProcess", install.Attribute("Type").Value);
        //    Assert.Equal("auto", install.Attribute("Start").Value);
        //    Assert.Equal("normal", install.Attribute("ErrorControl").Value);

        //    var dependencies = install.Elements("ServiceDependency").ToArray();
        //    Assert.Equal("Dnscache", dependencies[0].Attribute("Id").Value);
        //    Assert.Equal("Dhcp", dependencies[1].Attribute("Id").Value);

        //    var controll1 = all[1];
        //    Assert.Equal("ServiceControl", controll1.Name.LocalName);
        //    Assert.StartsWith("StopWixSharp.TestSvc", controll1.Attribute("Id").Value); //possible concurrency issues so handle "StopWixSharp.TestSvc.2" as well
        //    Assert.Equal("WixSharp.TestSvc", controll1.Attribute("Name").Value);
        //    Assert.Equal("both", controll1.Attribute("Stop").Value);
        //    Assert.Equal("yes", controll1.Attribute("Wait").Value);

        //    var controll2 = all[2];
        //    Assert.Equal("ServiceControl", controll2.Name.LocalName);
        //    Assert.StartsWith("StartWixSharp.TestSvc", controll2.Attribute("Id").Value);
        //    Assert.Equal("WixSharp.TestSvc", controll2.Attribute("Name").Value);
        //    Assert.Equal("install", controll2.Attribute("Start").Value);
        //    Assert.Equal("no", controll2.Attribute("Wait").Value);

        //    var controll3 = all[3];
        //    Assert.Equal("ServiceControl", controll3.Name.LocalName);
        //    Assert.StartsWith("RemoveWixSharp.TestSvc", controll3.Attribute("Id").Value);
        //    Assert.Equal("WixSharp.TestSvc", controll3.Attribute("Name").Value);
        //    Assert.Equal("uninstall", controll3.Attribute("Remove").Value);
        //    Assert.Equal("yes", controll3.Attribute("Wait").Value);
        //}

        //[Fact]
        //public void Should_Emit_OptionalAutoAttributes()
        //{
        //    var service = new ServiceInstaller
        //    {
        //        Name = "WixSharp.TestSvc"
        //    };

        //    var root = service.ToXml(projectMock).Cast<XElement>().First();

        //    Assert.Equal("WixSharp.TestSvc", root.Attribute("Name").Value);
        //    Assert.Equal("WixSharp.TestSvc", root.Attribute("DisplayName").Value);
        //    Assert.Equal("ownProcess", root.Attribute("Type").Value);
        //    Assert.Equal("auto", root.Attribute("Start").Value);
        //    Assert.Equal("normal", root.Attribute("ErrorControl").Value);
        //}

        //[Fact]
        //public void ShouldNot_Emit_AbsentOptionalAttributes()
        //{
        //    var service = new ServiceInstaller
        //    {
        //        Name = "WixSharp.TestSvc"
        //    };

        //    var root = service.ToXml(projectMock).Cast<XElement>().First();

        //    Assert.False(root.HasAttribute("Account"));
        //    Assert.False(root.HasAttribute("Arguments"));
        //    Assert.False(root.HasAttribute("Password"));
        //    Assert.False(root.HasAttribute("LoadOrderGroup"));
        //    Assert.False(root.HasAttribute("Vital"));
        //}

        //[Fact]
        //public void Should_Handle_NotInitializedAttributes()
        //{
        //    var element = new XElement("ServiceConfig");

        //    int? delay = null;

        //    element.SetAttributeValue("PreShutdownDelay", delay);
        //    Assert.False(element.HasAttribute("PreShutdownDelay"));

        //    delay = 1000;
        //    element.SetAttributeValue("PreShutdownDelay", delay);
        //    Assert.True(element.HasAttribute("PreShutdownDelay"));

        //    ServiceSid serviceSid = null;

        //    element.SetAttributeValue("ServiceSid", serviceSid);
        //    Assert.False(element.HasAttribute("ServiceSid"));

        //    serviceSid = ServiceSid.none;
        //    element.SetAttributeValue("ServiceSid", serviceSid);
        //    Assert.True(element.HasAttribute("ServiceSid", value => value == "none"));

        //    serviceSid = new ServiceSid("0");
        //    element.SetAttributeValue("ServiceSid", serviceSid);
        //    Assert.True(element.HasAttribute("ServiceSid", value => value == "0"));
        //}
    }
}