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

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing;
using System.Drawing.Design;
using System.Windows.Forms;
using Wix = WixSharp;

namespace WixSharp.Controls
{
    /// <summary>
    /// Defines <see cref="T:System.Windows.Forms" /> CheckBox control for generating WiX Button element.
    /// <para>
    /// The <see cref="T:WixSharp.WixButton" /> can be used with the <see cref="T:System.Windows.Forms" /> designer to define custom
    /// dialog layouts. The <see cref="T:WixSharp.Compiler" /> uses <see cref="T:WixSharp.WixCheckBox" /> instance at compile time
    /// to generate WiX CheckBox element on the base of this instance properties.
    /// </para>
    /// </summary>
    [Designer(typeof(WixCheckBoxDesigner))]
    public class WixCheckBox : CheckBox, IWixControl
    {
        /// <summary>
        /// Gets or sets the control conditions.
        /// </summary>
        /// <value>
        /// The control conditions.
        /// </value>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public List<WixControlCondition> Conditions { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="WixCheckBox"/> class.
        /// </summary>
        public WixCheckBox()
        {
            this.CheckBoxValue = "1";
            this.Conditions = new List<WixControlCondition>();
            this.LocationChanged += (x, y) => { WixLocation = new Point(this.Left.WScale(), this.Top.WScale()); };
            this.SizeChanged += (x, y) => { WixSize = new Size(this.Width.WScale(), this.Height.WScale()); };
        }

        /// <summary>
        /// Gets the size of the CheckBox.
        /// </summary>
        /// <value>
        /// The size of the CheckBox.
        /// </value>
        public Size WixSize { get; private set; }

        /// <summary>
        /// Gets the CheckBox location.
        /// </summary>
        /// <value>
        /// The CheckBox location.
        /// </value>
        public Point WixLocation { get; private set; }

        /// <summary>
        /// Gets or sets the CheckBox Id.
        /// </summary>
        /// <value>
        /// The CheckBox Id.
        /// </value>
        public string Id { get; set; }

        /// <summary>
        /// Gets or sets the tooltip.
        /// </summary>
        /// <value>
        /// The tooltip.
        /// </value>
        public string Tooltip { get; set; }

        /// <summary>
        /// Gets or sets the CheckBox value.
        /// </summary>
        /// <value>
        /// The CheckBox value.
        /// </value>
        public string CheckBoxValue { get; set; }

        /// <summary>
        /// Gets or sets the bound property name.
        /// </summary>
        /// <value>
        /// The bound property name.
        /// </value>
        public string BoundProperty { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="IWixControl" /> is hidden.
        /// </summary>
        /// <value>
        ///   <c>true</c> if hidden; otherwise, <c>false</c>.
        /// </value>
        public bool Hidden { get; set; }

        /// <summary>
        /// Gets or sets the WiX attributes.
        /// </summary>
        /// <value>
        /// The WiX attributes.
        /// </value>
        [Editor(typeof(MultilineStringEditor), typeof(UITypeEditor))]
        public string WixAttributes { get; set; }

        /// <summary>
        /// Converts the WinForm control into WiX custom UI control <see cref="T:WixSharp.Control" />.
        /// </summary>
        /// <returns>
        /// Instance of the WixSharp.Control.
        /// </returns>
        /// <exception cref="System.ApplicationException">WixCheckBox (' + control.Id + ') must have BoundProperty set to non-empty value.</exception>
        public virtual Wix.Controls.Control ToWControl()
        {
            Wix.Controls.Control control = this.ConvertToWControl(ControlType.CheckBox);

            //It is tempting to allow WiX compiler report the problem. However WiX is not reliable with the error reporting.
            //For example it does it for "CheckBox" but not for "Edit"
            //Note that control.Name is a better identity value than this.Name, which can be empty.
            if (BoundProperty.IsEmpty())
                throw new ApplicationException("WixCheckBox ('" + control.Id + "') must have BoundProperty set to non-empty value.");

            if (!CheckBoxValue.IsEmpty())
                control.AttributesDefinition += ";CheckBoxValue=" + CheckBoxValue;

            return control;
        }
    }
}
