using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing;
using System.Drawing.Design;
using System.Windows.Forms;
using Wix = WixSharp;
using System.Collections.Generic;

namespace WixSharp.Controls
{
    /// <summary>
    /// Defines <see cref="T:System.Windows.Forms" /> label control for generating WiX Label element.
    /// <para>
    /// The <see cref="T:WixSharp.WixLabel" /> can be used with the <see cref="T:System.Windows.Forms" /> designer to define custom
    /// dialog layouts. The <see cref="T:WixSharp.Compiler" /> uses <see cref="T:WixSharp.WixLabel" /> instance at compile time
    /// to generate WiX Label element on the base of this instance properties.
    /// </para>
    /// </summary>
    [Designer(typeof(WixLabelDesigner))]
    public class WixLabel : Label, IWixControl
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
        /// Initializes a new instance of the <see cref="WixLabel"/> class.
        /// </summary>
        public WixLabel()
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
        public virtual Wix.Controls.Control ToWControl()
        {
            Wix.Controls.Control retval = this.ConvertToWControl(ControlType.Text);
            //if (NoPrefix)
            //    retval.AttributesDefinition += ";NoPrefix=yes";
            return retval;
        }
    }
}
