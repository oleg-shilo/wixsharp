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
using System.Collections.Generic;
using System.Linq;
using WixSharp.CommonTasks;
using IO = System.IO;

#pragma warning disable CA1416 // Validate platform compatibility

namespace WixSharp
{
    /// <summary>
    /// Icon used for Shortcut, ProgId, or Class elements (but not UI controls)
    /// </summary>
    /// <seealso cref="WixSharp.WixEntity" />
    /// <seealso cref="WixSharp.IGenericEntity" />
    public class IconFile : WixEntity, IGenericEntity
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="IconFile"/> class.
        /// </summary>
        public IconFile()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="IconFile"/> class.
        /// </summary>
        /// <param name="sourcePath">The source path.</param>
        public IconFile(string sourcePath)
        {
            this.SourceFile = sourcePath;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="IconFile"/> class.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <param name="sourcePath">The source path.</param>
        public IconFile(Id id, string sourcePath)
        {
            this.Id = id.Value;
            this.SourceFile = sourcePath;
        }

        /// <summary>
        /// Gets or sets the <c>Id</c> value of the <see cref="WixEntity" />.
        /// <para>This value is used as a <c>Id</c> for the corresponding WiX XML element.</para><para>If the <see cref="Id" /> value is not specified explicitly by the user the Wix# compiler
        /// generates it automatically insuring its uniqueness.</para><remarks>
        /// Note: The ID auto-generation is triggered on the first access (evaluation) and in order to make the id
        /// allocation deterministic the compiler resets ID generator just before the build starts. However if you
        /// accessing any auto-id before the Build*() is called you can it interferes with the ID auto generation and eventually
        /// lead to the WiX ID duplications. To prevent this from happening either:"
        /// <para> - Avoid evaluating the auto-generated IDs values before the call to Build*()</para><para> - Set the IDs (to be evaluated) explicitly</para><para> - Prevent resetting auto-ID generator by setting WixEntity.DoNotResetIdGenerator to true";</para></remarks>
        /// </summary>
        /// <value>
        /// The id.
        /// </value>
        [WixSharp.Xml]
        new public string Id { get => base.Id; set => base.Id = value; }

        /// <summary>
        /// The path to the icon file.
        /// </summary>
        [WixSharp.Xml]
        public string SourceFile;

        /// <summary>
        /// Adds itself as an XML content into the WiX source being generated from the <see cref="WixSharp.Project" />.
        /// See 'Wix#/samples/Extensions' sample for the details on how to implement this interface correctly.
        /// </summary>
        /// <param name="context">The context.</param>
        public void Process(ProcessingContext context)
        {
            context.XParent.Add(this.ToXElement("Icon"));
        }
    }
}