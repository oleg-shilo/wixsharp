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
    /// Defines <see cref="DirectoryShortcut"/> to be installed. <para><see cref="DirectoryShortcut"/> is a specialized version of the <see cref="Shortcut"/> designed
    /// for using <see cref="Shortcut"/> as a <see cref="Dir"/> nested element it belongs to.</para>
    /// </summary>
    ///
    /// <example>
    /// The following is an example of installing "Samples Folder" shortcut.
    /// <code>
    /// var project =
    ///     new Project("My Product",
    ///
    ///         new Dir(@"%ProgramFiles%\My Company\My Product",
    ///             new DirctoryShortcut("Samples Folder", "[Samples]")),
    ///
    ///         ...
    ///
    /// Compiler.BuildMsi(project);
    /// </code>
    /// </example>
    public partial class DirectoryShortcut : ExeFileShortcut
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DirectoryShortcut"/> class.
        /// </summary>
        public DirectoryShortcut()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DirectoryShortcut"/> class with properties/fields initialized with specified parameters.
        /// <para>This constructor should be used to instantiate shortcuts which belong to the <see cref="Dir"/> element.</para>
        /// <para>The shortcut will be installed in the directory defined by the parent <see cref="Dir"/> element.</para>
        /// </summary>
        /// <param name="name">The name of the shortcut to be installed.</param>
        /// <param name="target">The path to the executable the shortcut is associated with.</param>
        public DirectoryShortcut(string name, string target)
        {
            Name = name;
            Target = target;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DirectoryShortcut"/> class with properties/fields initialized with specified parameters.
        /// <para>This constructor should be used to instantiate shortcuts which belong to the <see cref="Dir"/> element.</para>
        /// <para>The shortcut will be installed in the directory defined by the parent <see cref="Dir"/> element.</para>
        /// </summary>
        /// <param name="feature"><see cref="Feature"></see> the shortcut should be included in.</param>
        /// <param name="name">The name of the shortcut to be installed.</param>
        /// <param name="target">The path to the executable the shortcut is associated with.</param>
        public DirectoryShortcut(Feature feature, string name, string target)
        {
            Name = name;
            Target = target;
            Feature = feature;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DirectoryShortcut"/> class with properties/fields initialized with specified parameters.
        /// <para>This constructor should be used to instantiate shortcuts which belong to the <see cref="Dir"/> element.</para>
        /// <para>The shortcut will be installed in the directory defined by the parent <see cref="Dir"/> element.</para>
        /// </summary>
        /// <param name="id">The explicit <see cref="Id"></see> to be associated with <see cref="DirectoryShortcut"/> instance.</param>
        /// <param name="name">The name of the shortcut to be installed.</param>
        /// <param name="target">The path to the executable the shortcut is associated with.</param>
        public DirectoryShortcut(Id id, string name, string target)
        {
            Id = id.Value;
            Name = name;
            Target = target;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DirectoryShortcut"/> class with properties/fields initialized with specified parameters.
        /// <para>This constructor should be used to instantiate shortcuts which belong to the <see cref="Dir"/> element.</para>
        /// <para>The shortcut will be installed in the directory defined by the parent <see cref="Dir"/> element</para>
        /// </summary>
        /// <param name="id">The explicit <see cref="Id"></see> to be associated with <see cref="DirectoryShortcut"/> instance.</param>
        /// <param name="feature"><see cref="Feature"></see> the shortcut should be included in.</param>
        /// <param name="name">The name of the shortcut to be installed.</param>
        /// <param name="target">The path to the executable the shortcut is associated with.</param>
        public DirectoryShortcut(Id id, Feature feature, string name, string target)
        {
            Id = id.Value;
            Name = name;
            Target = target;
            Feature = feature;
        }
    }
}