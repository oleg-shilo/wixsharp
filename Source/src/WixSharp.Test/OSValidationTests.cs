using WixSharp.Nsis.WinVer;
using Xunit;

namespace WixSharp.Test
{
    // ReSharper disable once InconsistentNaming
    public class OSValidationTests
    {
        [Fact]
        public void BuildVersionCheckScriptPart_WhenNoVersions_ShouldReturnEmptyString()
        {
            // Arrange
            var notSup = CreateOsValidation();

            // Act
            var script = notSup.BuildVersionCheckScriptPart();

            // Assert
            Assert.True(script.Equals(string.Empty));
        }
        
        [Fact]
        public void BuildVersionCheckScriptPart_WhenExactVersion_ShouldCreateCorrectScript()
        {
            // Arrange
            var correctScript = @"${If} ${IsWin7}
    ClearErrors
    ${GetOptions} $R0 ""/S"" $0
    ${If} ${Errors}
        !define MB_OK 0x00000000
        !define MB_ICONERROR 0x00000010
        System::Call 'USER32::MessageBox(i $hwndparent, t ""This operating system is not supported.$\nPlease, update your OS."", t ""Error"", i ${MB_OK}|${MB_ICONERROR})i'
    ${EndIf}
    goto end
${EndIf}
";
            var osValidation = CreateOsValidation();
            osValidation.UnsupportedVersions.Add(new WindowsVersion(WindowsVersionNumber._7));

            // Act
            var script = osValidation.BuildVersionCheckScriptPart();

            // Assert
            Assert.Equal(script, correctScript);
        }
        
        [Fact]
        public void BuildVersionCheckScriptPart_WhenExactVersionSp_ShouldCreateCorrectScript()
        {
            // Arrange
            var correctScript = @"${If} ${IsWin8}
    ${AndIf} ${IsServicePack} 1
    ClearErrors
    ${GetOptions} $R0 ""/S"" $0
    ${If} ${Errors}
        !define MB_OK 0x00000000
        !define MB_ICONERROR 0x00000010
        System::Call 'USER32::MessageBox(i $hwndparent, t ""This operating system is not supported.$\nPlease, update your OS."", t ""Error"", i ${MB_OK}|${MB_ICONERROR})i'
    ${EndIf}
    goto end
${EndIf}
";
            var osValidation = CreateOsValidation();
            osValidation.UnsupportedVersions.Add(new WindowsVersion(WindowsVersionNumber._8, 1));

            // Act
            var script = osValidation.BuildVersionCheckScriptPart();

            // Assert
            Assert.Equal(script, correctScript);
        }
        
        [Fact]
        public void BuildVersionCheckScriptPart_WhenMinVersion_ShouldCreateCorrectScript()
        {
            // Arrange
            var correctScript = @"${Unless} ${AtLeastWin2012}
    ClearErrors
    ${GetOptions} $R0 ""/S"" $0
    ${If} ${Errors}
        !define MB_OK 0x00000000
        !define MB_ICONERROR 0x00000010
        System::Call 'USER32::MessageBox(i $hwndparent, t ""This operating system is not supported.$\nPlease, update your OS."", t ""Error"", i ${MB_OK}|${MB_ICONERROR})i'
    ${EndIf}
    goto end
${EndUnless}
";
            var osValidation = CreateOsValidation(); 
            osValidation.MinVersion = WindowsVersionNumber._2012;

            // Act
            var script = osValidation.BuildVersionCheckScriptPart();

            // Assert
            Assert.Equal(script, correctScript);
        }
        
        [Fact]
        public void BuildVersionCheckScriptPart_WhenCombinationOfVersions_ShouldCreateCorrectScript()
        {
            // Arrange
            var correctScript = @"${Unless} ${AtLeastWin7}
${OrIf} ${IsWin8}
${OrIf} ${IsWin2008}
    ${AndIf} ${IsServicePack} 0
    ClearErrors
    ${GetOptions} $R0 ""/S"" $0
    ${If} ${Errors}
        !define MB_OK 0x00000000
        !define MB_ICONERROR 0x00000010
        System::Call 'USER32::MessageBox(i $hwndparent, t ""This operating system is not supported.$\nPlease, update your OS."", t ""Error"", i ${MB_OK}|${MB_ICONERROR})i'
    ${EndIf}
    goto end
${EndUnless}
";
            var osValidation = CreateOsValidation();
            osValidation.MinVersion = WindowsVersionNumber._7;
            osValidation.UnsupportedVersions.Add(new WindowsVersion(WindowsVersionNumber._8));
            osValidation.UnsupportedVersions.Add(new WindowsVersion(WindowsVersionNumber._2008, 0));

            // Act
            var script = osValidation.BuildVersionCheckScriptPart();

            // Assert
            Assert.Equal(script, correctScript);
        }
        
        [Fact]
        public void BuildVersionCheckScriptPart_WhenTerminationFalse_ShouldNotGoToEnd()
        {
            // Arrange
            var correctScript = @"${If} ${IsWin7}
    ClearErrors
    ${GetOptions} $R0 ""/S"" $0
    ${If} ${Errors}
        !define MB_OK 0x00000000
        !define MB_ICONERROR 0x00000010
        System::Call 'USER32::MessageBox(i $hwndparent, t ""This operating system is not supported.$\nPlease, update your OS."", t ""Error"", i ${MB_OK}|${MB_ICONERROR})i'
    ${EndIf}
${EndIf}
";
            var osValidation = CreateOsValidation();
            osValidation.UnsupportedVersions.Add(new WindowsVersion(WindowsVersionNumber._7));
            osValidation.TerminateInstallation = false;

            // Act
            var script = osValidation.BuildVersionCheckScriptPart();

            // Assert
            Assert.Equal(script, correctScript);
        }
        
        [Fact]
        public void BuildVersionCheckScriptPart_WhenCustomErrorMessage_ShouldApplyIt()
        {
            // Arrange
            var customErrorMessage = "Custom error message";
            var correctScript = @"${If} ${IsWin7}
    ClearErrors
    ${GetOptions} $R0 ""/S"" $0
    ${If} ${Errors}
        !define MB_OK 0x00000000
        !define MB_ICONERROR 0x00000010
        System::Call 'USER32::MessageBox(i $hwndparent, t """+customErrorMessage+@""", t ""Error"", i ${MB_OK}|${MB_ICONERROR})i'
    ${EndIf}
    goto end
${EndIf}
";
            var osValidation = CreateOsValidation();
            osValidation.UnsupportedVersions.Add(new WindowsVersion(WindowsVersionNumber._7));
            osValidation.ErrorMessage = customErrorMessage;

            // Act
            var script = osValidation.BuildVersionCheckScriptPart();

            // Assert
            Assert.Equal(script, correctScript);
        }

        private OSValidation CreateOsValidation() => new OSValidation();
    }
}