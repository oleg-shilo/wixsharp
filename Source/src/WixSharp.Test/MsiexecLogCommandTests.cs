using WixSharp.Msiexec;
using Xunit;

namespace WixSharp.Test
{
    public class MsiexecLogCommandTests
    {
        [Theory]
        [InlineData(null)]
        [InlineData("")]
        public void Generate_WhenLogFilePathNullEmpty_ShouldReturnEmptyString(string log)
        {
            // Act
            var result = MsiexecLogCommand.Generate(log);

            // Assert
            Assert.Equal(result, string.Empty);
        }

        [Fact]
        public void Generate_WhenNoneFlags_ShouldCreateCommandWithoutFlags()
        {
            // Arrange
            var path = "some/path";

            // Act
            // ReSharper disable once RedundantArgumentDefaultValue
            var command = MsiexecLogCommand.Generate(path, MsiexecLogSwitches.None);

            // Assert
            Assert.Equal(command, $" /L \"{path}\"");
        }

        [Theory]
        [InlineData(MsiexecLogSwitches.A, "A")]
        [InlineData(MsiexecLogSwitches.Append | MsiexecLogSwitches.I, "I+")]
        [InlineData(MsiexecLogSwitches.Star | MsiexecLogSwitches.V, "V*")]
        [InlineData(MsiexecLogSwitches.C | MsiexecLogSwitches.P | MsiexecLogSwitches.R | MsiexecLogSwitches.U | MsiexecLogSwitches.FlushEachLine, "RUCP!")]
        [InlineData(MsiexecLogSwitches.I | MsiexecLogSwitches.W | MsiexecLogSwitches.A | MsiexecLogSwitches.M | MsiexecLogSwitches.X, "IWAMX")]
        public void Generate_WhenFlags_ShouldCreateCommandWithFlags(MsiexecLogSwitches flags, string stringFlags)
        {
            // Arrange
            var path = "some/path";

            // Act
            // ReSharper disable once RedundantArgumentDefaultValue
            var command = MsiexecLogCommand.Generate(path, flags);

            // Assert
            Assert.Equal(command, $" /L{stringFlags} \"{path}\"");
        }
    }
}