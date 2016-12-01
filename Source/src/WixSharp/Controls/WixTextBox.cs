using System.ComponentModel;
using System.Windows.Forms;
using Wix = WixSharp;
using System;
using System.Drawing.Design;
using System.ComponentModel.Design;
using System.Drawing;
using System.Collections.Generic;

namespace WixSharp.Controls
{
    /// <summary>
    /// Defines <see cref="T:System.Windows.Forms" /> text box control for generating WiX TextBox element.
    /// <para>
    /// The <see cref="T:WixSharp.WixTextBox" /> can be used with the <see cref="T:System.Windows.Forms" /> designer to define custom
    /// dialog layouts. The <see cref="T:WixSharp.Compiler" /> uses <see cref="T:WixSharp.WixTextBox" /> instance at compile time
    /// to generate WiX textBox element on the base of this instance properties.
    /// </para>
    /// </summary>
    [Designer(typeof(WixTextBoxDesigner))]
    public class WixTextBox : TextBox, IWixControl
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
        /// Initializes a new instance of the <see cref="WixTextBox"/> class.
        /// </summary>
        public WixTextBox()
        {
            this.Conditions = new List<WixControlCondition>();
            this.LocationChanged += (x, y) => { WixLocation = new Point(this.Left.WScale(), this.Top.WScale()); };
            this.SizeChanged += (x, y) => { WixSize = new Size(this.Width.WScale(), this.Height.WScale()); };
        }

        /// <summary>
        /// Gets the size of the <c>Control</c>.
        /// </summary>
        /// <value>
        /// The size of the <c>Control</c>.
        /// </value>
        public Size WixSize { get; private set; }
        /// <summary>
        /// Gets the <c>Control</c> location.
        /// </summary>
        /// <value>
        /// The <c>Control</c> location.
        /// </value>
        public Point WixLocation { get; private set; }
        /// <summary>
        /// Gets or sets the Id.
        /// </summary>
        /// <value>
        /// The Id.
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
        /// Gets or sets the WiX element attributes.
        /// </summary>
        /// <value>
        /// The WiX element attributes.
        /// </value>
        [Editor(typeof(MultilineStringEditor), typeof(UITypeEditor))]
        public string WixAttributes { get; set; }

        /// <summary>
        /// Converts the WinForm control into WiX custom UI control <see cref="T:WixSharp.Control" />.
        /// </summary>
        /// <returns>
        /// Instance of the WixSharp.Control.
        /// </returns>
        /// <exception cref="System.ApplicationException">WixTextBox (' + control.Id + ') must have BoundProperty set to non-empty value.</exception>
        public virtual Wix.Controls.Control ToWControl()
        {
            Wix.Controls.Control control = this.ConvertToWControl(ControlType.Edit);

            if (BoundProperty.IsEmpty())
                throw new ApplicationException("WixTextBox ('" + control.Id + "') must have BoundProperty set to non-empty value.");

            return control;
        }
    }
}
