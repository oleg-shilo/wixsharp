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

using System.Xml.Linq;
namespace WixSharp
{
    /// <summary>
    /// Defines shortcut to be installed. <see cref="Shortcut"/> is not supposed to be instantiated directly.
    /// Derivative class constructors should be used instead (e.g. <see cref="FileShortcut"/>, <see cref="ExeFileShortcut"/>).
    /// </summary>
    public partial class Shortcut : WixEntity
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Shortcut"/> class.
        /// </summary>
        protected Shortcut()
        {
        }

        /// <summary>
        /// The path to the executable the shortcut is associated with.
        /// <para>This value is a processed only for shortcuts which belong to the <see cref="Dir"/> element.</para>
        /// </summary>
        public string Target = "";
        /// <summary>
        /// The shortcut arguments.
        /// </summary>
        public string Arguments = "";
        /// <summary>
        /// The directory where the shortcut should be installed.
        /// <para>This value is a processed only for shortcuts which belong to the <see cref="File"/> element.</para>
        /// </summary>
        public string Location = "";
        /// <summary>
        /// <see cref="Feature"></see> the shortcut belongs to.
        /// </summary>
        public Feature Feature;
        /// <summary>
        /// "Working Directory" for the shortcut to be installed. If <see cref="WorkingDirectory"/> is not specified
        /// it will be set in the installed shortcut to the location of the file the shortcut points to.
        /// </summary>
        public string WorkingDirectory = "";
        /// <summary>
        /// Defines the launch <see cref="Condition"/> which is to be checked during the installation to
        /// determine if the shortcut should be installed.
        /// </summary>
        public Condition Condition;
        /// <summary>
        /// Defines icon file for the shortcut. Relative or absolute path to the source file containing icons (exe, dll or ico).
        /// </summary>
        public string IconFile = "";
        /// <summary>
        /// The zero-based index of the icon associated with this ProgId. The default value is 0.
        /// </summary>
        public int IconIndex = 0;
        /// <summary>
        /// Whether this shortcut is to be advertised. The default is <c>false</c>.
        /// </summary>
        public bool Advertise = false;
    }

    internal static class ShortcutExtensions
    {
        static public void EmitAttributes(this Shortcut shortcut, XElement shortcutElement)
        {
            shortcutElement.AddAttributes(shortcut.Attributes);

            if (shortcut.Arguments == "" && shortcutElement.Attribute("Arguments") != null)
                shortcutElement.Attribute("Arguments").Remove();

            if (shortcut.Advertise)
                shortcutElement.Add(new XAttribute("Advertise", "yes"));

            if (!shortcut.IconFile.IsEmpty())
            {
                shortcutElement.Add(new XAttribute("Icon", shortcut.IconFile)); //note the IconFile will be converted into Icon (ID) in the AutoElements.InjectAutoElementsHandler(...)
                shortcutElement.Add(new XAttribute("IconIndex", shortcut.IconIndex));
            }
        }
    }
}
