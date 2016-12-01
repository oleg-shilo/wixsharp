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
using System;
namespace WixSharp
{
    /// <summary>
    /// Defines <see cref="FileShortcut"/> to be installed.
    /// <para>
    /// <see cref="FileShortcut"/> is a specialized version of the <see cref="Shortcut"/> designed 
    /// for using <see cref="Shortcut"/> as a <see cref="File"/> nested element.
    /// </para>
    /// <para>There are different ways of defining shortcuts of the Wix# project: </para>
    /// <para> - It can be specified as a nested element of the <see cref="File"/>. In this case, after 
    /// the installation, the shortcut will point to the file it belongs to.</para>
    /// <para> - Alternatively the <c>Shortcut</c> can be specified as a <see cref="File"/> nested 
    /// element. In this case after the installation the shortcut will point to the file 
    /// will point to the file it belongs to.</para>
    /// </summary>
    /// 
    /// <example>
    /// The following is an example of installing <c>MyApp.exe</c> file with the corresponding 
    /// shortcuts in on <c>Desktop</c> and <c>Program Menu</c>.
    /// <code>
    /// var project = 
    ///     new Project("My Product",
    ///     
    ///         new Dir(@"%ProgramFiles%\My Company\My Product",
    ///         
    ///             new File(binaries, @"AppFiles\MyApp.exe",
    ///                 new FileShortcut("MyApp", @"%ProgramMenu%\My Company\My Product"),
    ///                 new FileShortcut("MyApp", @"%Desktop%")),
    ///                 
    ///         ...
    ///         
    /// Compiler.BuildMsi(project);
    /// </code>
    /// </example>
    public partial class FileShortcut : Shortcut
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FileShortcut"/> class.
        /// </summary>
        public FileShortcut()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FileShortcut"/> class with properties/fields initialized with specified parameters.
        /// <para>This constructor should be used to instantiate shortcuts, which belong to the <see cref="File"/> element.</para>
        /// </summary>
        /// <param name="id">The explicit <see cref="Id"></see> to be associated with <see cref="FileShortcut"/> instance.</param>
        /// <param name="feature"><see cref="Feature"></see> the shortcut should be included in.</param>
        /// <param name="location">The directory where the shortcut should be installed.</param>
        public FileShortcut(Id id, Feature feature, string location)
        {
            Id = id.Value;
            Feature = feature;
            Location = location;
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="FileShortcut"/> class with properties/fields initialized with specified parameters.
        /// <para>This constructor should be used to instantiate shortcuts, which belong to the <see cref="File"/> element.</para>
        /// </summary>
        /// <param name="feature"><see cref="Feature"></see> the shortcut should be included in.</param>
        /// <param name="location">The directory where the shortcut should be installed.</param>
        public FileShortcut(Feature feature, string location)
        {
            Feature = feature;
            Location = location;
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="FileShortcut"/> class with properties/fields initialized with specified parameters.
        /// <para>This constructor should be used to instantiate shortcuts, which belong to the <see cref="File"/> element.</para>
        /// </summary>
        /// <param name="name">The name of the shortcut to be installed.</param>
        /// <param name="location">The directory where the shortcut should be installed.</param>
        public FileShortcut(string name, string location)
        {
            Name = name;
            Location = location;
        }

        /// <summary>
        /// Inherited member, which you should not use as the WiX Shortcut element doesn't support Condition.
        /// </summary>
        public new Condition Condition
        {
            set { throw new ApplicationException("Despite the fact that " + this.GetType() + " has member Condition you should not use as the WiX Shortcut element doesn't support Condition."); }
        }


        /// <summary>
        /// Initializes a new instance of the <see cref="FileShortcut"/> class with properties/fields initialized with specified parameters.
        /// <para>This constructor should be used to instantiate shortcuts, which belong to the <see cref="File"/> element.</para>
        /// </summary>
        /// <param name="id">The explicit <see cref="Id"></see> to be associated with <see cref="FileShortcut"/> instance.</param>
        /// <param name="name">The name of the shortcut to be installed.</param>
        /// <param name="location">The directory where the shortcut should be installed.</param>
        public FileShortcut(Id id, string name, string location)
        {
            Id = id.Value;
            Name = name;
            Location = location;
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="FileShortcut"/> class with properties/fields initialized with specified parameters.
        /// <para>This constructor should be used to instantiate shortcuts, which belong to the <see cref="File"/> element.</para>
        /// </summary>
        /// <param name="feature"><see cref="Feature"></see> the shortcut should be included in.</param>
        /// <param name="name">The name of the shortcut to be installed.</param>
        /// <param name="location">The directory where the shortcut should be installed.</param>
        public FileShortcut(Feature feature, string name, string location)
            : this(feature, location)
        {
            Name = name;
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="FileShortcut"/> class with properties/fields initialized with specified parameters.
        /// <para>This constructor should be used to instantiate shortcuts, which belong to the <see cref="File"/> element.</para>
        /// </summary>
        /// <param name="id">The explicit <see cref="Id"></see> to be associated with <see cref="FileShortcut"/> instance.</param>
        /// <param name="feature"><see cref="Feature"></see> the shortcut should be included in.</param>
        /// <param name="name">The name of the shortcut to be installed.</param>
        /// <param name="location">The directory where the shortcut should be installed.</param>
        public FileShortcut(Id id, Feature feature, string name, string location)
            : this(id, feature, location)
        {
            Name = name;
        }
    }
}
