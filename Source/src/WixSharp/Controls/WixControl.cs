using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing;
using System.Drawing.Design;
using System.Windows.Forms;
using Wix = WixSharp;

namespace WixSharp.Controls
{
    /* Problems:
     * Bitmap source is encoded as text
     * All types are squashed together
     * Actions (event handlers) are encoded as Events
     * Some properties are control type specific
     * WixPixel != WinPixel WiX(56,17) -> Control(75,23) WiX pixels are not the same as Windows ones
     * MSI property type is defined as the control attribute (e.g. Control.CheckBox.Integer)
     * There is no way to support custom controls
     * Very convoluted way of controlling Enabled/Disabled. Through two dedicated properties.
     * Not type safe:  prop "Integer" - YesNoType - Specifies if the property of the control is an integer. Otherwise it is treated as a string
     * Documentation is sometimes just appalling: "NoPrefix	YesNoType	Only valid for Text Controls"
     * Text style (e.g. font) is embedded into text itself as prefix.
     *     The API logic and error reporting is shocking:
     *     "{\DlgTitleFont}" prefix is interpreted as a font instruction but compiler complains "... uses undefined TextStyle DlgTitleFont"
     *     and ...... produces the valid MSI anyway
     *
     *     Though this is what http://wix.tramontana.co.hu/tutorial/user-interface-revisited/a-single-dialog says:
     *         "NoPrefix only controls whether ampersand characters are displayed verbatim or used as shortcut specifies, as usual in the Windows GUI."
     */

    /// <summary>
    /// Defines <see cref="T:System.Windows.Forms" /> generic control for generating WiX Control element.
    /// <para>
    /// The <see cref="T:WixSharp.WixControl" /> can be used with the <see cref="T:System.Windows.Forms" /> designer to define custom
    /// dialog layouts. The <see cref="T:WixSharp.Compiler" /> uses <see cref="T:WixSharp.WixControl" /> instance at compile time
    /// to generate WiX Control element on the base of this instance properties.
    /// </para>
    /// </summary>
    [Designer(typeof(WixControlDesigner))]
    public class WixControl : Panel, IWixControl, IWixInteractiveControl
    {
        List<ControlActionData> actions = new List<ControlActionData>();

        /// <summary>
        /// Gets or sets the control conditions.
        /// </summary>
        /// <value>
        /// The control conditions.
        /// </value>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public List<WixControlCondition> Conditions { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="WixControl"/> class.
        /// </summary>
        public WixControl()
        {
            this.Conditions = new List<WixControlCondition>();
            this.LocationChanged += (x, y) => { WixLocation = new Point(this.Left.WScale(), this.Top.WScale()); };
            this.SizeChanged += (x, y) => { WixSize = new Size(this.Width.WScale(), this.Height.WScale()); };
            BackColor = Color.DarkGray;
        }

        /// <summary>
        /// Occurs when <see cref="T:WixSharp.Compiler" /> triggers it in order to generate WiX XML
        /// for the control action(s).
        /// </summary>
        new public event ClickHandler Click;

        /// <summary>
        /// Gets the actions associated with the WiX <c>Control</c>.
        /// </summary>
        /// <value>
        /// The actions.
        /// </value>
        [Browsable(false)]
        public List<ControlActionData> Actions
        {
            get
            {
                return actions;
            }
        }

        /// <summary>
        /// Triggers <c>Click</c> event.
        /// </summary>
        public void PerformClick()
        {
            if (Click != null)
                Click();
        }

        /// <summary>
        /// Gets the size of the <c>Control</c>.
        /// </summary>
        /// <value>
        /// The size of the <c>Control</c>.
        /// </value>
        public Size WixSize { get; private set; }

        /// <summary>
        /// Gets the WiX <c>Control</c> location.
        /// </summary>
        /// <value>
        /// The location.
        /// </value>
        public Point WixLocation { get; private set; }

        /// <summary>
        /// Gets or sets the <c>Control</c> Id.
        /// </summary>
        /// <value>
        /// The Id.
        /// </value>
        public string Id { get; set; }

        /// <summary>
        /// Gets or sets the text to be displayed by the <c>Control</c>.
        /// </summary>
        /// <value>
        /// The text.
        /// </value>
        public string WixText
        {
            get { return Text; }
            set { Text = value; }
        }

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
        /// Gets or sets the raw XML to be embedded into WiX <c>Control</c> element.
        /// </summary>
        /// <value>
        /// The embedded XML.
        /// </value>
        [Editor(typeof(MultilineStringEditor), typeof(UITypeEditor))]
        public string EmbeddedXML { get; set; }

        /// <summary>
        /// Gets or sets the type of the control.
        /// </summary>
        /// <value>
        /// The type of the control.
        /// </value>
        public ControlType ControlType { get; set; }

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
            return this.ConvertToWControl(this.ControlType);
        }
    }
}
