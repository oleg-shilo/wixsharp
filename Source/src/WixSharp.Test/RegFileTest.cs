using System.Text;
using Xunit;

namespace WixSharp.Test
{
    public class RegFileTest
    {
        [Fact]
        public void Should_Create_RegValue_From_String()
        {
            object value;

            value = RegFileImporter.Deserialize(@"""\""[INSTALLDIR]7zFM.exe\"" \""%1\""""", Encoding.Unicode);
            Assert.IsType<string>(value);
            Assert.Equal(value, @"""[INSTALLDIR]7zFM.exe"" ""%1""");

            value = RegFileImporter.Deserialize("\"test\"", Encoding.Unicode);
            Assert.IsType<string>(value);
            Assert.Equal(value, "test");


            value = RegFileImporter.Deserialize("dword:00000020", Encoding.Unicode);
            Assert.IsType<int>(value);
            Assert.Equal(value, 32);

            value = RegFileImporter.Deserialize("hex(2):25,00,50,00,41,00,54,00,48,00,25,00,00,00", Encoding.Unicode);
            Assert.IsType<string>(value);
            Assert.Equal(value, "%PATH%");

            value = RegFileImporter.Deserialize(@"hex(7):6f,00,6e,00,65,00,00,00,74,00,77,00,6f,00,00,00,74,00,68,\
00,72,00,65,00,65,00,00,00,00,00", Encoding.Unicode);
            Assert.IsType<string>(value);
            Assert.Equal(value, "one\r\ntwo\r\nthree");
        }

    }
}