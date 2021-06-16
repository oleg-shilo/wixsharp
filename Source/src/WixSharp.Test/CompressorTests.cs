using WixSharp.Nsis;
using Xunit;

namespace WixSharp.Test
{
    public class CompressorTests
    {
        [Fact]
        public void ToString_WhenIsSolid_ShouldAddSolidSwitch()
        {
            // Arrange
            var isSolid = true;
            var compressor = new Compressor(isSolid, false, Compressor.CompressionMethod.Bzip2);

            // Act 
            var command = compressor.ToString();

            // Assert
            Assert.Contains("/SOLID",command);
        }
        
        [Fact]
        public void ToString_WhenNotIsSolid_ShouldNotAddSolidSwitch()
        {
            // Arrange
            var isSolid = false;
            var compressor = new Compressor(isSolid, false, Compressor.CompressionMethod.Bzip2);

            // Act 
            var command = compressor.ToString();

            // Assert
            Assert.DoesNotContain("/SOLID",command);
        }
        
        [Fact]
        public void ToString_WhenIsFinal_ShouldAddFinalSwitch()
        {
            // Arrange
            var isFinal = true;
            var compressor = new Compressor(true, isFinal, Compressor.CompressionMethod.Bzip2);

            // Act 
            var command = compressor.ToString();

            // Assert
            Assert.Contains("/FINAL",command);
        }
        
        [Fact]
        public void ToString_WhenNotIsFinal_ShouldNotAddFinalSwitch()
        {
            // Arrange
            var isFinal = false;
            var compressor = new Compressor(true, isFinal, Compressor.CompressionMethod.Bzip2);

            // Act 
            var command = compressor.ToString();

            // Assert
            Assert.DoesNotContain("/FINAL",command);
        }

        [Theory]
        [InlineData(Compressor.CompressionMethod.Bzip2)]
        [InlineData(Compressor.CompressionMethod.Lzma)]
        [InlineData(Compressor.CompressionMethod.Zlib)]
        public void ToString_ShouldAddChosenCompressionMethod(Compressor.CompressionMethod compressionMethod)
        {
            // Arrange
            var compressor = new Compressor(true, true, compressionMethod);

            // Act
            var command = compressor.ToString();

            // Assert
            Assert.Contains(compressionMethod.GetDescription(), command);
        }

        [Theory]
        [InlineData(true, true, Compressor.CompressionMethod.Bzip2, "SetCompressor /SOLID /FINAL bzip2")]
        [InlineData(true, false, Compressor.CompressionMethod.Bzip2, "SetCompressor /SOLID bzip2")]
        [InlineData(false, true, Compressor.CompressionMethod.Bzip2, "SetCompressor /FINAL bzip2")]
        [InlineData(false, false, Compressor.CompressionMethod.Lzma, "SetCompressor lzma")]
        [InlineData(true, true, Compressor.CompressionMethod.Zlib, "SetCompressor /SOLID /FINAL zlib")]
        public void ToString_ShouldCreateValidCommands(bool isSolid, bool isFinal,
            Compressor.CompressionMethod compressionMethod, string commandToCompare)
        {
            // Arrange
            var compressor = new Compressor(isSolid, isFinal, compressionMethod);

            // Act
            var command = compressor.ToString();

            // Assert
            Assert.Equal(command, commandToCompare);
        }
    }
}