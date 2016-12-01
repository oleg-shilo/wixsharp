using System;
using System.Linq;
using System.Xml.Linq;
using WixSharp.Bootstrapper;
using Xunit;

namespace WixSharp.Test
{
    public class BootstrapperTest
    {
        [Fact]
        public void Should_Process_XmlAttribute()
        {
            var bootstrapper = new Bundle("test_name")
            {
                AboutUrl = "a_url",
                DisableRollback = true,
                IconFile = "icon.ico",
                Version = new Version("1.2.3.4"),
                UpgradeCode = new Guid("00000000-0000-0000-0000-000000000007")
            };

            var xml = bootstrapper.ToXml().Cast<XElement>().First();

            //<Bundle Name="test_name" AboutUrl="a_url" IconSourceFile="icon.ico" UpgradeCode="00000000-0000-0000-0000-000000000007" Version="1.2.3.4" />
            Assert.Equal(5, xml.Attributes().Count());

            Assert.Equal("test_name", xml.ReadAttribute("Name"));
            Assert.Equal("a_url", xml.ReadAttribute("AboutUrl"));
            Assert.Equal("yes", xml.Element("Chain").ReadAttribute("DisableRollback"));
            Assert.Equal("icon.ico", xml.ReadAttribute("IconSourceFile"));
            Assert.Equal("1.2.3.4", xml.ReadAttribute("Version"));
            Assert.Equal("00000000-0000-0000-0000-000000000007", xml.ReadAttribute("UpgradeCode"));

            Assert.Null(xml.Element("Chain").ReadAttribute("DisableSystemRestore")); //bool?
            Assert.Null(xml.ReadAttribute("Copyright"));//string
        }

        [Fact]
        public void Should_Produce_ExePackageXml()
        {
            var entity = new ExePackage(@"Samples\Setup1.exe")
            {
                Id = "package1",
                Name = "Setup1",
                Payloads = new[] { @"Samples\setup1.dll", @"Samples\setup2.dll", },
                InstallCommand = "/q /norestart",
                Permanent = true,
            };

            var xml = entity.ToXml().First().ToString();
            var expected = "<ExePackage Name=\"Setup1\" Id=\"package1\" InstallCommand=\"/q /norestart\" Permanent=\"yes\" SourceFile=\"Samples\\Setup1.exe\">\r\n" +
                           "  <Payload SourceFile=\"Samples\\setup1.dll\" />\r\n" +
                           "  <Payload SourceFile=\"Samples\\setup2.dll\" />\r\n" +
                           "</ExePackage>";

            Assert.Equal(expected, xml);
        }

        [Fact]
        public void Should_Produce_MsiPackageXml()
        {
            var entity = new MsiPackage(@"Samples\SetupA.msi")
            {
                Payloads = new[] { @"Samples\setup1.dll", @"Samples\setup2.dll", },
                Permanent = true,
                MsiProperties = "TRANSFORMS=[CommandArgs];GLOBAL=yes"
            };

            var xml = entity.ToXml().First().ToString();
            var expected = "<MsiPackage Permanent=\"yes\" SourceFile=\"Samples\\SetupA.msi\">\r\n" +
                           "  <Payload SourceFile=\"Samples\\setup1.dll\" />\r\n" +
                           "  <Payload SourceFile=\"Samples\\setup2.dll\" />\r\n" +
                           "  <MsiProperty Name=\"TRANSFORMS\" Value=\"[CommandArgs]\" />\r\n" +
                           "  <MsiProperty Name=\"GLOBAL\" Value=\"yes\" />\r\n" +
                           "  <MsiProperty Name=\"WIXBUNDLEORIGINALSOURCE\" Value=\"[WixBundleOriginalSource]\" />\r\n" +
                           "</MsiPackage>";

            Assert.Equal(expected, xml);
        }

        [Fact]
        public void Should_Produce_BundleXml()
        {
            var bootstrapper =
                new Bundle("My Product",
                    new PackageGroupRef("NetFx40Web"),
                    new ExePackage(@"Samples\Setup1.exe")
                    {
                        Payloads = new[] { @"Samples\setup1.dll" },
                        InstallCommand = "/q /norestart",
                        PerMachine = true
                    },
                    new MsiPackage(@"Samples\SetupB.msi") { Vital = false },
                    new RollbackBoundary(),
                    new MsiPackage(@"Samples\SetupA.msi"));

            bootstrapper.AboutUrl = "https://wixsharp.codeplex.com/";
            bootstrapper.IconFile = "icon.ico";
            bootstrapper.Version = new Version("1.0.0.0");
            bootstrapper.UpgradeCode = new Guid("00000000-0000-0000-0000-000000000007");
            bootstrapper.Application.LicensePath = "readme.rtf";
            bootstrapper.Application.LocalizationFile = "en-us.wxl";
            bootstrapper.Application.LogoFile = "app_logo.png";

            var xml = bootstrapper.ToXml().First().ToString();

            var expected = @"<Bundle Name=""My Product"" AboutUrl=""https://wixsharp.codeplex.com/"" IconSourceFile=""icon.ico"" UpgradeCode=""00000000-0000-0000-0000-000000000007"" Version=""1.0.0.0"">
  <BootstrapperApplicationRef Id=""WixStandardBootstrapperApplication.RtfLicense"">
    <WixStandardBootstrapperApplication LogoFile=""app_logo.png"" LocalizationFile=""en-us.wxl"" LicenseFile=""readme.rtf"" xmlns=""http://schemas.microsoft.com/wix/BalExtension"" />
  </BootstrapperApplicationRef>
  <Chain>
    <PackageGroupRef Id=""NetFx40Web"" />
    <ExePackage InstallCommand=""/q /norestart"" SourceFile=""Samples\Setup1.exe"" PerMachine=""yes"">
      <Payload SourceFile=""Samples\setup1.dll"" />
    </ExePackage>
    <MsiPackage SourceFile=""Samples\SetupB.msi"" Vital=""no"">
      <MsiProperty Name=""WIXBUNDLEORIGINALSOURCE"" Value=""[WixBundleOriginalSource]"" />
    </MsiPackage>
    <RollbackBoundary />
    <MsiPackage SourceFile=""Samples\SetupA.msi"">
      <MsiProperty Name=""WIXBUNDLEORIGINALSOURCE"" Value=""[WixBundleOriginalSource]"" />
    </MsiPackage>
  </Chain>
</Bundle>";
            Assert.Equal(expected, xml);
        }

        [Fact]
        public void Should_Resolve_RtfLicenseType()
        {
            var bootstrapper = new Bundle("My Product");

            bootstrapper.UpgradeCode = new Guid("00000000-0000-0000-0000-000000000007");
            bootstrapper.Application.LicensePath = "readme.rtf";

            var xml = bootstrapper.ToXml().First().ToString();

            var expected = @"<Bundle Name=""My Product"" UpgradeCode=""00000000-0000-0000-0000-000000000007"">
  <BootstrapperApplicationRef Id=""WixStandardBootstrapperApplication.RtfLicense"">
    <WixStandardBootstrapperApplication LicenseFile=""readme.rtf"" xmlns=""http://schemas.microsoft.com/wix/BalExtension"" />
  </BootstrapperApplicationRef>
  <Chain />
</Bundle>";
            Assert.Equal(expected, xml);
        }

        [Fact]
        public void Should_Resolve_LocalHtmlLicenseType()
        {
            var bootstrapper = new Bundle("My Product");

            bootstrapper.UpgradeCode = new Guid("00000000-0000-0000-0000-000000000007");
            bootstrapper.Application.LicensePath = @"Resource\License.html";

            var xml = bootstrapper.ToXml().First().ToString();

            var expected = @"<Bundle Name=""My Product"" UpgradeCode=""00000000-0000-0000-0000-000000000007"">
  <BootstrapperApplicationRef Id=""WixStandardBootstrapperApplication.HyperlinkLicense"">
    <Payload SourceFile=""Resource\License.html"" />
    <WixStandardBootstrapperApplication LicenseUrl=""License.html"" xmlns=""http://schemas.microsoft.com/wix/BalExtension"" />
  </BootstrapperApplicationRef>
  <Chain />
</Bundle>";
            Assert.Equal(expected, xml);
        }

        [Fact]
        public void Should_Resolve_WebLicenseType()
        {
            var bootstrapper = new Bundle("My Product");

            bootstrapper.UpgradeCode = new Guid("00000000-0000-0000-0000-000000000007");
            bootstrapper.Application.LicensePath = "http://opensource.org/licenses/MIT";

            var xml = bootstrapper.ToXml().First().ToString();

            var expected = @"<Bundle Name=""My Product"" UpgradeCode=""00000000-0000-0000-0000-000000000007"">
  <BootstrapperApplicationRef Id=""WixStandardBootstrapperApplication.HyperlinkLicense"">
    <WixStandardBootstrapperApplication LicenseUrl=""http://opensource.org/licenses/MIT"" xmlns=""http://schemas.microsoft.com/wix/BalExtension"" />
  </BootstrapperApplicationRef>
  <Chain />
</Bundle>";
            Assert.Equal(expected, xml);
        }

        [Fact]
        public void Should_Resolve_NoLicenseType()
        {
            var bootstrapper = new Bundle("My Product");

            bootstrapper.UpgradeCode = new Guid("00000000-0000-0000-0000-000000000007");
            bootstrapper.Application.LicensePath = null;

            var xml = bootstrapper.ToXml().First().ToString();

            var expected = @"<Bundle Name=""My Product"" UpgradeCode=""00000000-0000-0000-0000-000000000007"">
  <BootstrapperApplicationRef Id=""WixStandardBootstrapperApplication.HyperlinkLicense"">
    <WixStandardBootstrapperApplication LicenseUrl="""" xmlns=""http://schemas.microsoft.com/wix/BalExtension"" />
  </BootstrapperApplicationRef>
  <Chain />
</Bundle>";
            Assert.Equal(expected, xml);
        }
    }
}
