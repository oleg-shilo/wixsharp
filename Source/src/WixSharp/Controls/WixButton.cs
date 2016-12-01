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
    /// Defines <see cref="T:System.Windows.Forms" /> button control for generating WiX Button element.
    /// <para>
    /// The <see cref="T:WixSharp.WixButton" /> can be used with the <see cref="T:System.Windows.Forms" /> designer to define custom
    /// dialog layouts. The <see cref="T:WixSharp.Compiler" /> uses <see cref="T:WixSharp.WixButton" /> instance at compile time
    /// to generate WiX Button element on the base of this instance properties.
    /// </para>
    /// </summary>
    [Designer(typeof(WixButtonDesigner))]
    public class WixButton : Button, IWixControl, IWixInteractiveControl
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
        /// Initializes a new instance of the <see cref="WixButton"/> class.
        /// </summary>
        public WixButton()
        {
            Conditions = new List<WixControlCondition>();
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
        /// Occurs when <see cref="T:WixSharp.Compiler" /> triggers it in order to generate WiX XML
        /// for the control action(s).
        /// </summary>
        new public event ClickHandler Click;

        /// <summary>
        /// Generates a <see cref="T:System.Windows.Forms.Control.Click" /> event for a button.
        /// </summary>
        /// <remarks>Because WixSharp UI controls are used only at compile time to generate WiX actions XML the events are raised/triggered by
        /// the <see cref="T:WixSharp.Compiler"/> but not by the user at runtime (deployment time).
        /// </remarks>
        new public void PerformClick()
        {
            if (Click != null)
                Click();
        }

        /// <summary>
        /// Gets or sets the <c>Control</c> Id.
        /// </summary>
        /// <value>
        /// The Id.
        /// </value>
        public string Id { get; set; }

        /// <summary>
        /// Gets or sets the <c>Control</c> tooltip.
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
        public virtual Wix.Controls.Control ToWControl()
        {
            return this.ConvertToWControl(ControlType.PushButton);
        }
    }

   
}
