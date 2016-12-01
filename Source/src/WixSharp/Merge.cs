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
using System.Linq;
using System.Collections.Generic;
using System;

namespace WixSharp
{
    /// <summary>
    /// Defines MergeModule to be embedded into MSI.
    /// <para>
    /// Note that WiX syntax allows Merge element to belong to the Product element. However the documentation (WiX v3.0.4917.0) explicitly requires 
    /// it to be nested inside of the Directory element (even if there is no neither technical nor logical reason for this). Thus Wix# simply follows
    /// the WiX convention. 
    /// </para>
    /// </summary>
    /// 
    ///<example>The following is an example of using <c>MyMergeModule.msm</c> file.
    ///<code>
    /// var project = 
    ///    new Project("MyMergeModule",
    ///        new Dir(@"%ProgramFiles%\My Company",
    ///            new Merge("MyMergeModule.msm")));,
    ///                 
    ///         ...
    ///         
    /// Compiler.BuildMsi(project);
    /// </code>
    /// </example>
    public partial class Merge : WixEntity
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Merge"/> class.
        /// </summary>
        public Merge() 
        {
            Name = "MergeModule";
        }
        /// <summary>
        /// Creates instance of the <see cref="Merge"></see> class with properties initialized with specified parameters.
        /// </summary>
        /// <param name="sourceFile">Relative path to the msm file to be taken for building the MSI.</param>
        public Merge(string sourceFile)
        {
            Name = 
            SourceFile = sourceFile;
        }
        /// <summary>
        /// Creates instance of the <see cref="Merge"></see> class with properties initialized with specified parameters.
        /// </summary>
        /// <param name="feature"><see cref="Feature"></see> the msm should be included in.</param>
        /// <param name="sourceFile">Relative path to the msm file to be taken for building the MSI.</param>
        public Merge(Feature feature, string sourceFile)
        {
            Name = 
            SourceFile = sourceFile;
            Feature = feature;
        }
        /// <summary>
        /// Specifies if the files in the merge module should be compressed. 
        /// </summary>
        public bool FileCompression = true;
        /// <summary>
        /// Path to the merge module file.
        /// </summary>
        public string SourceFile;
        /// <summary>
        /// <see cref="Feature"></see> the merge module belongs to.
        /// </summary>
        public Feature Feature;
        ///// <summary>
        ///// Defines the installation <see cref="Condition"/>, which is to be checked during the installation to 
        ///// determine if the file should be installed on the target system.
        ///// </summary>
        //public Condition Condition; //currently WiX does not allow child Condition element but in the future release it most likely will
    }
}
