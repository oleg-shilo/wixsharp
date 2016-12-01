#region Licence...
/*
The MIT License (MIT)

Copyright (c) 2015 Oleg Shilo

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
using System.Linq;
using System.Collections.Generic;
using System;
using System.Xml.Linq;

namespace WixSharp
{

    /// <summary>
    /// Defines file to be installed.
    /// </summary>
    /// 
    ///<example>The following is an example of installing <c>MyApp.exe</c> file.
    ///<code>
    /// var project = 
    ///         new Project("MyProduct",
    ///             new Dir(@"%ProgramFiles%\My Company\My Product",
    ///                 new File("readme.txt")),
    ///             new Dir("%Fonts%",
    ///                 new FontFile("FreeSansBold.ttf")));
    ///         ...
    ///         
    /// project.BuildMsi();
    /// </code>
    /// </example>
    public partial class FontFile : File
    {
        /// <summary>
        /// Returns a <see cref="T:System.String"/> that represents the <see cref="FontFile"/>.
        /// <para>This property is designed to produce a friendlier string representation of the <see cref="FontFile"/>
        /// for debugging purposes.</para>
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.String"/> that represents the <see cref="FontFile"/>.
        /// </returns>
        public new string ToString()
        {
            return IO.Path.GetFileName(Name) + "; " + Name;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FontFile"/> class.
        /// </summary>
        public FontFile()
        {
            TrueType = true;
        }
        /// <summary>
        /// Creates instance of the <see cref="FontFile"></see> class with properties initialized with specified parameters.
        /// </summary>
        /// <param name="feature"><see cref="Feature"></see> the file should be included in.</param>
        /// <param name="sourcePath">Relative path to the file to be taken for building the MSI.</param>
        public FontFile(Feature feature, string sourcePath)
        {
            Name = sourcePath;
            Feature = feature;
            TrueType = true;
        }

        /// <summary>
        /// Gets or sets a value indicating whether the font file is a TrueType font. Default value is <c>true</c>
        /// <para>Causes an entry to be generated for the file in the Font table with no FontTitle specified. This attribute is intended to be used to register the file as a TrueType font.</para>
        /// </summary>
        /// <value><c>true</c> if TrueType font otherwise, <c>false</c>.</value>
        public bool TrueType { get; set; }

        /// <summary>
        /// Gets or sets the font title.
        /// <para>Causes an entry to be generated for the file in the Font 
        /// table with the specified FontTitle. This attribute is intended to be used to register the file as a non-TrueType font.</para>
        /// </summary>
        /// <value>The font title.</value>
        public string FontTitle { get; set; }

        /// <summary>
        /// Creates instance of the <see cref="FontFile"></see> class with properties initialized with specified parameters.
        /// </summary>
        /// <param name="id">The explicit <see cref="Id"></see> to be associated with <see cref="FontFile"/> instance.</param>
        /// <param name="feature"><see cref="Feature"></see> the file should be included in.</param>
        /// <param name="sourcePath">Relative path to the file to be taken for building the MSI.</param>
        public FontFile(Id id, Feature feature, string sourcePath)
        {
            Id = id.Value;
            Name = sourcePath;
            Feature = feature;
            TrueType = true;
        }
        /// <summary>
        /// Creates instance of the <see cref="FontFile"></see> class with properties initialized with specified parameters.
        /// </summary>
        /// <param name="sourcePath">Relative path to the file to be taken for building the MSI.</param>
        public FontFile(string sourcePath)
        {
            Name = sourcePath;
            TrueType = true;
        }
        /// <summary>
        /// Creates instance of the <see cref="FontFile"></see> class with properties initialized with specified parameters.
        /// </summary>
        /// <param name="id">The explicit <see cref="Id"></see> to be associated with <see cref="FontFile"/> instance.</param>
        /// <param name="sourcePath">Relative path to the file to be taken for building the MSI.</param>
        public FontFile(Id id, string sourcePath)
        {
            Id = id.Value;
            Name = sourcePath;
            TrueType = true;
        }
    }
}
