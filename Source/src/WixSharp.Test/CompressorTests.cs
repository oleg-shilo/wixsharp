using WixSharp.Nsis;
using Xunit;

namespace WixSharp.Test
{
    public class CompressorTests
    {
        [Fact]
        public void ToString_WhenSolid_ShouldAddSolidSwitch()
        {
            // Arrange
            var compressor = new Compressor(Compressor.Method.Bzip2, Compressor.Options.Solid);

            // Act 
            var command = compressor.ToString();

            // Assert
            Assert.Contains("/SOLID", command);
        }

        [Fact]
        public void ToString_WhenFinal_ShouldAddFinalSwitch()
        {
            // Arrange
            var compressor = new Compressor(Compressor.Method.Bzip2, Compressor.Options.Final);

            // Act 
            var command = compressor.ToString();

            // Assert
            Assert.Contains("/FINAL", command);
        }

        [Fact]
        public void ToString_WhenNone_ShouldNotAddSwitches()
        {
            // Arrange
            var compressor = new Compressor(Compressor.Method.Bzip2, Compressor.Options.None);

            // Act 
            var command = compressor.ToString();

            // Assert
            Assert.DoesNotContain("/SOLID", command);
            Assert.DoesNotContain("/FINAL", command);
        }

        [Fact]
        public void ToString_WhenSolidFinal_ShouldAddSolidFinalSwitches()
        {
            // Arrange
            var compressor =
                new Compressor(Compressor.Method.Bzip2, Compressor.Options.Solid | Compressor.Options.Final);

            // Act 
            var command = compressor.ToString();

            // Assert
            Assert.Contains("/SOLID /FINAL", command);
        }

        [Theory]
        [InlineData(Compressor.Method.Bzip2)]
        [InlineData(Compressor.Method.Lzma)]
        [InlineData(Compressor.Method.Zlib)]
        public void ToString_ShouldAddChosenCompressionMethod(Compressor.Method method)
        {
            // Arrange
            var compressor = new Compressor(method, Compressor.Options.None);

            // Act
            var command = compressor.ToString();

            // Assert
            Assert.Contains(method.GetDescription(), command);
        }

        [Theory]
        [InlineData(Compressor.Method.Bzip2, Compressor.Options.Solid | Compressor.Options.Final, "SetCompressor /SOLID /FINAL bzip2")]
        [InlineData(Compressor.Method.Bzip2, Compressor.Options.Solid, "SetCompressor /SOLID bzip2")]
        [InlineData(Compressor.Method.Bzip2, Compressor.Options.Final, "SetCompressor /FINAL bzip2")]
        [InlineData(Compressor.Method.Lzma, Compressor.Options.None, "SetCompressor lzma")]
        [InlineData(Compressor.Method.Zlib, Compressor.Options.Solid | Compressor.Options.Final, "SetCompressor /SOLID /FINAL zlib")]
        public void ToString_ShouldCreateValidCommands( Compressor.Method method, Compressor.Options options,
           string commandToCompare)
        {
            // Arrange
            var compressor = new Compressor(method, options);

            // Act
            var command = compressor.ToString();

            // Assert
            Assert.Equal(command, commandToCompare);
        }
    }
}