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
using IO = System.IO;
using System.Collections.Generic;

namespace WixSharp
{
    /// <summary>
    /// Defines FileType association to be created for the file extension and the installed file/application (parent  <see cref="T:WixSharp.File"/>).
    /// </summary>
    ///<example>The following is an example of associating file extension ".my" with installed application MyViewer.exe.
    ///<code>
    /// var project = 
    ///     new Project("My Product",
    ///         new Dir(@"%ProgramFiles%\My Company\My Product",
    ///             new Dir(@"Notepad",
    ///                 new File("MyViewer.exe",
    ///                     new FileAssociation("my", "application/my", "open", "\"%1\"")))));  
    ///         ...
    ///         
    /// Compiler.BuildMsi(project);
    /// </code>
    /// </example>
    public partial class FileAssociation : WixEntity
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FileAssociation"/> class.
        /// </summary>
        /// <param name="extension">The file extension, like "doc" or "xml". Do not include the preceding period.</param>
        /// <param name="contentType">The MIME type that is to be written (e.g. <c>application/notepad)</c>.</param>
        /// <param name="command">The localized text displayed on the context menu (e.g. <c>Open</c>).</param>
        /// <param name="commandArguments">Value for the command arguments (e.g. <c>"%1"</c>).</param>
        public FileAssociation(string extension, string contentType, string command, string commandArguments)
        {
            this.Name = extension;
            this.Command = command;
            this.Arguments = commandArguments;
            this.Description = extension + " file";
            this.ContentType = contentType;
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="FileAssociation"/> class.
        /// </summary>
        /// <param name="extension">The file extension, like "doc" or "xml". Do not include the preceding period.</param>
        public FileAssociation(string extension)
        {
            this.Name = extension;
            this.Description = extension + " file";
            this.ContentType = "application/" + extension;
        }

        /// <summary>
        /// This is simply the file extension, like "doc" or "xml". Do not include the preceding period.
        /// </summary>
        /// <value>The extension.</value>
        public string Extension { get { return Name; } set { Name = value; } }
        /// <summary>
        /// The localized text displayed on the context menu. The default is <c>Open</c>.
        /// </summary>
        public string Command = "Open";
        /// <summary>
        /// Value for the command arguments.  The default is <c>"%1"</c>.
        /// </summary>
        public string Arguments = "\"%1\"";
        /// <summary>
        /// The sequence of the commands. Only <see cref="FileAssociation"/> for which the SequenceNo is specified are 
        /// used to prepare an ordered list for the default value of the shell key. 
        /// The <see cref="FileAssociation"/> with the lowest value in this column becomes the default <see cref="FileAssociation"/>. 
        /// Used only for Advertised <see cref="FileAssociation"/>.
        /// </summary>
        public int SequenceNo = 1;
        /// <summary>
        /// Optional localizable description of the <see cref="FileAssociation"/>.
        /// </summary>
        public string Description = "";
        /// <summary>
        /// Reference to <c>Icon</c> element to be used to install the file association <c>DefaultIcon</c>. If the value is not set, Wix# compiler will use the <c>Id</c> of the <c>File</c> element of the parent component.<see cref="FileAssociation"/>.
        /// <para>Set this value to <c>null</c> if you do not want to install <c>DefaultIcon</c> at all.</para>
        /// </summary>
        public string Icon = "";
        /// <summary>
        /// The zero-based index of the icon associated with this ProgId. The default value is <c>0</c>. 
        /// </summary>
        public int IconIndex = 0;

        /// <summary>
        /// The MIME type that is to be written. The default is <c>application/[extension]</c>.
        /// </summary>
        public string ContentType;
        /// <summary>
        /// Whether this extension is to be advertised. The default is <c>false</c>.
        /// </summary>
        public bool Advertise = false;
    }
}
