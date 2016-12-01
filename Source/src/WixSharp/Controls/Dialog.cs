#region Licence...

//-----------------------------------------------------------------------------
// Date:	03/04/12	Time: 19:00
// Module:	WixSharp.cs
// Version: 0.1.20
//
// This module contains the definition of the Wix# classes.
//
// Written by Oleg Shilo (oshilo@gmail.com)
// Copyright (c) 2008-2012. All rights reserved.
//
// Redistribution and use of this code in source and binary forms, with or without
// modification, are permitted provided that the following conditions are met:
// 1. Redistributions of source code must retain the above copyright notice,
//	this list of conditions and the following disclaimer.
// 2. Neither the name of an author nor the names of the contributors may be used
//	to endorse or promote products derived from this software without specific
//	prior written permission.
//
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS
// "AS IS" AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT
// LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR
// A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT
// OWNER OR CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL,
// SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED
// TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR
// PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF
// LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING
// NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS
// SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
//
//	Caution: Bugs are expected!
//----------------------------------------------

#endregion Licence...

using System.Xml.Linq;

namespace WixSharp.Controls
{
    /// <summary>
    /// Defines MSI Dialog. It represents WiX <c>Dialog</c> element.
    /// </summary>
    public partial class Dialog : WixEntity
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Dialog"/> class.
        /// </summary>
        public Dialog()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Dialog"/> class.
        /// </summary>
        /// <param name="id">The id.</param>
        public Dialog(Id id)
        {
            base.Id = id;
        }

        /// <summary>
        /// The height of the dialog box in dialog units.
        /// </summary>
        public int Height;

        /// <summary>
        /// The width of the dialog box in dialog units.
        /// </summary>
        public int Width;

        /// <summary>
        /// The title of the dialog box.
        /// </summary>
        public string Title;

        /// <summary>
        /// Collection of the contained nested <see cref="Control"/>s (UI elements).
        /// </summary>
        public Control[] Controls = new Control[0];

        /// <summary>
        /// Converts the <see cref="T:WixSharp.Dialog"/> instance into WiX <see cref="T:System.Xml.Linq.XElement"/>.
        /// </summary>
        /// <returns><see cref="T:System.Xml.Linq.XElement"/> instance.</returns>
        public virtual XElement ToXElement()
        {
            var dialog =
                new XElement("Dialog",
                    new XAttribute("Id", this.Id),
                    new XAttribute("Width", this.Width),
                    new XAttribute("Height", this.Height),
                    new XAttribute("Title", this.Title))
                    .AddAttributes(this.Attributes);

            foreach (Control item in Controls)
                dialog.Add(item.ToXElement());

            return dialog;
        }

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return this.Id;
        }

        /// <summary>
        /// Performs an implicit conversion from <see cref="Dialog"/> to <see cref="System.String"/>.
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <returns>
        /// The result of the conversion.
        /// </returns>
        public static implicit operator string(Dialog obj)
        {
            return obj.ToString();
        }
    }
}