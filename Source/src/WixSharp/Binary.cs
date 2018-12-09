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

#endregion Licence...

using System;
using System.Diagnostics;
using System.Linq;

namespace WixSharp
{
    /// <summary>
    /// Defines binary file to be embedded into MSI (<c>Binary</c> table).
    /// <para>
    /// You can use this class to embed any file (e.g. exe, dll, image) to be used during the installation.
    /// Note that none of the binary files are installed on the target system. They are just available at installation time
    /// for using <c>CustomActions</c>.
    /// </para>
    /// </summary>
    ///
    /// <example>The following is an example of embedding <c>CRTSetup.msi</c> file into MSI for further use in
    /// <c>InstallCRTAction</c>  <see cref="ManagedAction"/>.
    /// <code>
    /// var project = new Project("MyProduct",
    ///
    ///         new Dir(@"%ProgramFiles%\My Company\My Product",
    ///             new File("readme.txt")),
    ///             ...
    ///
    ///         new Binary("CRTSetup.msi"),
    ///         new ManagedAction("InstallCRTAction",
    ///         ...
    ///
    /// </code>
    /// </example>
    public partial class Binary : WixEntity
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Binary"/> class.
        /// </summary>
        public Binary() { }

        /// <summary>
        /// Initializes a new instance of the <see cref="Binary"/> class with properties initialized with specified parameters.
        /// </summary>
        /// <param name="sourcePath">Relative path to the file to be taken for building the MSI.</param>
        public Binary(string sourcePath)
        {
            Name = ResolvePath(sourcePath);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Binary"/> class.
        /// </summary>
        /// <param name="id">The explicit <see cref="Id"></see> to be associated with <see cref="Binary"/> instance.</param>
        /// <param name="sourcePath">Relative path to the file to be taken for building the MSI.</param>
        public Binary(Id id, string sourcePath)
        {
            Id = id.Value;
            Name = ResolvePath(sourcePath);
        }

        string ResolvePath(string path)
        {
            if (path == "%this%")
            {
                var callingAssembly = new StackTrace().GetFrames()
                                                      .Select(f => f.GetMethod().DeclaringType.Assembly)
                                                      .First(a => a != this.GetType().Assembly)
                                                      .Location;
                return callingAssembly;
            }
            else
                return path;
        }

        /// <summary>
        /// The flag that indicates if the binary file is a .NET assembly.
        /// </summary>
        public bool IsAssembly = false;
    }

    /// <summary>
    /// Defines assembly file to be embedded into MSI (<c>Binary</c> table).
    /// <para>
    /// You can use this class to embed any .NET assembly to be used during the installation.
    /// Note that none of the binary files are installed on the target system. They are just available at installation time
    /// for using <c>CustomActions</c>.
    /// </para>
    /// </summary>
    ///
    /// <example>The following is an example of embedding <c>CRTSetup.msi</c> file into MSI for further use in
    /// <c>InstallCRTAction</c>  <see cref="ManagedAction"/>.
    /// <code>
    /// var project = new Project("MyProduct",
    ///
    ///         new Dir(@"%ProgramFiles%\My Company\My Product",
    ///             new File(@"readme.txt")),
    ///             ...
    ///
    ///         new EmbeddedAssembly("config.exe") { RefAssemblies = new [] { "utils.dll" }},
    ///         new ManagedAction("InstallCRTAction",
    ///         ...
    ///
    /// </code>
    /// </example>
    public partial class EmbeddedAssembly : Binary
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EmbeddedAssembly"/> class.
        /// </summary>
        public EmbeddedAssembly() { }

        /// <summary>
        /// Initializes a new instance of the <see cref="EmbeddedAssembly"/> class with properties initialized with specified parameters.
        /// </summary>
        /// <param name="sourcePath">Relative path to the assembly file to be taken for building the MSI.</param>
        public EmbeddedAssembly(string sourcePath) : base(sourcePath)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EmbeddedAssembly"/> class.
        /// </summary>
        /// <param name="id">The explicit <see cref="Id"></see> to be associated with <see cref="EmbeddedAssembly"/> instance.</param>
        /// <param name="sourcePath">Relative path to the assembly file to be taken for building the MSI.</param>
        public EmbeddedAssembly(Id id, string sourcePath) : base(id, sourcePath)
        {
        }

        /// <summary>
        /// The referenced assemblies the EmbeddedAssembly depends on.
        /// </summary>
        public string[] RefAssemblies = new string[0];
    }
}