#region Licence...
/*
The MIT License (MIT)

Copyright (c) 2014 Oleg Shilo

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
namespace WixSharp
{
    /// <summary>
    /// Defines assembly file to be installed.
    /// <para>
    /// This class is essentially the same as <see cref="File"></see> except it has extra member <c>RegisterInGAC</c>
    /// to define if the assembly file needs to be registered in GAC during the installation.
    /// </para>
    /// </summary>
    /// <example>The following is an example of installing <c>MyLibrary.dll</c> assembly and registering it in GAC.
    ///<code>
    /// var project = new Project()
    /// {
    ///     Name = "CustomActionTest",
    ///     UI = WUI.WixUI_ProgressOnly,
    ///     
    ///     Dirs = new[]
    ///     { 
    ///         new Dir(@"%ProgramFiles%\My Company\My Product",
    ///             new Assembly(@"MyLibrary.dll", true))
    ///     }
    /// };
    /// 
    /// Compiler.BuildMsi(project);
    /// </code>
    /// </example>
    public partial class Assembly : File
    {
        /// <summary>
        /// Default constructor. Creates instance of the <see cref="Assembly"></see> class.
        /// </summary>
        public Assembly() { }
        /// <summary>
        /// Creates instance of the <see cref="Assembly"></see> class with properties initialized with specified parameters.
        /// </summary>
        /// <param name="feature"><see cref="Feature"></see> the assembly file should be included in.</param>
        /// <param name="sourcePath">Relative path to the assembly file to be taken for building the MSI.</param>
        /// <param name="registerInGAC">Defines if the assembly file needs to be registered in GAC during the installation. </param>
        /// <param name="items">Optional <see cref="FileShortcut"/> parameters defining shortcuts to the assembly file to be created during the installation.</param>
        public Assembly(Feature feature, string sourcePath, bool registerInGAC, params FileShortcut[] items)
            : base(feature, sourcePath, items)
        {
            RegisterInGAC = registerInGAC;
        }
        /// <summary>
        /// Creates instance of the <see cref="Assembly"></see> class with properties initialized with specified parameters.
        /// </summary>
        /// <param name="sourcePath">Relative path to the assembly file to be taken for building the MSI.</param>
        /// <param name="registerInGAC">Defines if the assembly file needs to be registered in GAC during the installation. </param>
        /// <param name="items">Optional <see cref="FileShortcut"/> parameters defining shortcuts to the assembly file to be created during the installation.</param>
        public Assembly(string sourcePath, bool registerInGAC, params FileShortcut[] items)
            : base(sourcePath, items)
        {
            RegisterInGAC = registerInGAC;
        }
        /// <summary>
        /// Creates instance of the <see cref="Assembly"></see> class with properties initialized with specified parameters.
        /// </summary>
        /// <param name="id">The explicit <see cref="Id"></see> to be associated with <see cref="Assembly"/> instance.</param>
        /// <param name="feature"><see cref="Feature"></see> the assembly file should be included in.</param>
        /// <param name="sourcePath">Relative path to the assembly file to be taken for building the MSI.</param>
        /// <param name="registerInGAC">Defines if the assembly file needs to be registered in GAC during the installation. </param>
        /// <param name="items">Optional <see cref="FileShortcut"/> parameters defining shortcuts to the assembly file to be created during the installation.</param>
        public Assembly(Id id, Feature feature, string sourcePath, bool registerInGAC, params FileShortcut[] items)
            : base(id, feature, sourcePath, items)
        {
            RegisterInGAC = registerInGAC;
        }
        /// <summary>
        /// Creates instance of the <see cref="Assembly"></see> class with properties initialized with specified parameters.
        /// </summary>
        /// <param name="id">The explicit <see cref="Id"></see> to be associated with <see cref="Assembly"/> instance.</param>
        /// <param name="sourcePath">Relative path to the assembly file to be taken for building the MSI.</param>
        /// <param name="registerInGAC">Defines if the assembly file needs to be registered in GAC during the installation. </param>
        /// <param name="items">Optional <see cref="FileShortcut"/> parameters defining shortcuts to the assembly file to be created during the installation.</param>
        public Assembly(Id id, string sourcePath, bool registerInGAC, params FileShortcut[] items)
            : base(id, sourcePath, items)
        {
            RegisterInGAC = registerInGAC;
        }
        /// <summary>
        /// Creates instance of the <see cref="Assembly"></see> class with properties initialized with specified parameters.
        /// </summary>
        /// <param name="feature"><see cref="Feature"></see> the assembly file should be included in.</param>
        /// <param name="sourcePath">Relative path to the assembly file to be taken for building the MSI.</param>
        /// <param name="registerInGAC">Defines if the assembly file needs to be registered in GAC during the installation. </param>
        /// <param name="processorArchitecture">Specifies the architecture for this assembly. </param>
        /// <param name="items">Optional <see cref="FileShortcut"/> parameters defining shortcuts to the assembly file to be created during the installation.</param>
        public Assembly(Feature feature, string sourcePath, bool registerInGAC, ProcessorArchitecture processorArchitecture, params FileShortcut[] items)
            : base(feature, sourcePath, items)
        {
            RegisterInGAC = registerInGAC;
            ProcessorArchitecture = processorArchitecture;
        }
        /// <summary>
        /// Creates instance of the <see cref="Assembly"></see> class with properties initialized with specified parameters.
        /// </summary>
        /// <param name="sourcePath">Relative path to the assembly file to be taken for building the MSI.</param>
        /// <param name="registerInGAC">Defines if the assembly file needs to be registered in GAC during the installation. </param>
        /// <param name="processorArchitecture">Specifies the architecture for this assembly. </param>
        /// <param name="items">Optional <see cref="FileShortcut"/> parameters defining shortcuts to the assembly file to be created during the installation.</param>
        public Assembly(string sourcePath, bool registerInGAC, ProcessorArchitecture processorArchitecture, params FileShortcut[] items)
            : base(sourcePath, items)
        {
            RegisterInGAC = registerInGAC;
            ProcessorArchitecture = processorArchitecture;
        }
        /// <summary>
        /// Creates instance of the <see cref="Assembly"></see> class with properties initialized with specified parameters.
        /// </summary>
        /// <param name="id">The explicit <see cref="Id"></see> to be associated with <see cref="Assembly"/> instance.</param>
        /// <param name="feature"><see cref="Feature"></see> the assembly file should be included in.</param>
        /// <param name="sourcePath">Relative path to the assembly file to be taken for building the MSI.</param>
        /// <param name="registerInGAC">Defines if the assembly file needs to be registered in GAC during the installation. </param>
        /// <param name="processorArchitecture">Specifies the architecture for this assembly. </param>
        /// <param name="items">Optional <see cref="FileShortcut"/> parameters defining shortcuts to the assembly file to be created during the installation.</param>
        public Assembly(Id id, Feature feature, string sourcePath, bool registerInGAC, ProcessorArchitecture processorArchitecture, params FileShortcut[] items)
            : base(id, feature, sourcePath, items)
        {
            RegisterInGAC = registerInGAC;
            ProcessorArchitecture = processorArchitecture;
        }
        /// <summary>
        /// Creates instance of the <see cref="Assembly"></see> class with properties initialized with specified parameters.
        /// </summary>
        /// <param name="id">The explicit <see cref="Id"></see> to be associated with <see cref="Assembly"/> instance.</param>
        /// <param name="sourcePath">Relative path to the assembly file to be taken for building the MSI.</param>
        /// <param name="registerInGAC">Defines if the assembly file needs to be registered in GAC during the installation. </param>
        /// <param name="processorArchitecture">Specifies the architecture for this assembly. </param>
        /// <param name="items">Optional <see cref="FileShortcut"/> parameters defining shortcuts to the assembly file to be created during the installation.</param>
        public Assembly(Id id, string sourcePath, bool registerInGAC, ProcessorArchitecture processorArchitecture, params FileShortcut[] items)
            : base(id, sourcePath, items)
        {
            RegisterInGAC = registerInGAC;
            ProcessorArchitecture = processorArchitecture;
        }
        /// <summary>
        /// Defines if the assembly file needs to be registered in GAC during the installation. 
        /// </summary>
        public bool RegisterInGAC = false;
        /// <summary>
        /// Specifies the architecture for this assembly.
        /// </summary>
        public ProcessorArchitecture ProcessorArchitecture = ProcessorArchitecture.msil;
    }
}
