using System.Collections.Generic;
using System.Drawing;

namespace WixSharp.Controls
{
    /// <summary>
    /// Delegate for  <see cref="IWixInteractiveControl"/> event <c>Click</c>.
    /// </summary>
    public delegate void ClickHandler();

    /// <summary>
    /// The interface for the WinForm control representing WiX custom UI control exhibiting interactive behavior.
    /// </summary>
    public interface IWixInteractiveControl
    {
        /// <summary>
        /// Gets the actions associated with the WiX <c>Control</c>.
        /// </summary>
        /// <value>
        /// The actions.
        /// </value>
        List<ControlActionData> Actions { get; }

        /// <summary>
        /// Occurs when <see cref="T:WixSharp.Compiler"/> triggers it in order to generate WiX XML
        /// for the control action(s).
        /// </summary>
        event ClickHandler Click;

        /// <summary>
        /// Triggers <c>Click</c> event.
        /// </summary>
        void PerformClick();
    }

    /// <summary>
    /// The interface for the WinForm control representing generic WiX custom UI control.
    /// </summary>
    public interface IWixControl
    {
        /// <summary>
        /// Gets or sets the Id.
        /// </summary>
        /// <value>
        /// The Id.
        /// </value>
        string Id { get; set; }

        /// <summary>
        /// Gets or sets the bound property name.
        /// </summary>
        /// <value>
        /// The bound property name.
        /// </value>
        string BoundProperty { get; set; }

        /// <summary>
        /// Gets or sets the tooltip.
        /// </summary>
        /// <value>
        /// The tooltip.
        /// </value>
        string Tooltip { get; set; }

        /// <summary>
        /// Gets or sets the WiX attributes.
        /// </summary>
        /// <value>
        /// The WiX attributes.
        /// </value>
        string WixAttributes { get; set; }

        /// <summary>
        /// Gets the size of the <c>Control</c>.
        /// </summary>
        /// <value>
        /// The size of the <c>Control</c>.
        /// </value>
        Size WixSize { get; }

        /// <summary>
        /// Gets the <c>Control</c> location.
        /// </summary>
        /// <value>
        /// The <c>Control</c> location.
        /// </value>
        Point WixLocation { get; }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="IWixControl"/> is hidden.
        /// </summary>
        /// <value>
        ///   <c>true</c> if hidden; otherwise, <c>false</c>.
        /// </value>
        bool Hidden { get; set; }

        /// <summary>
        /// Gets or sets the control conditions.
        /// </summary>
        /// <value>
        /// The control conditions.
        /// </value>
        List<WixControlCondition> Conditions { get; set; }

        /// <summary>
        /// Converts the WinForm control into WiX custom UI control <see cref="T:WixSharp.Control"/>.
        /// </summary>
        /// <returns>Instance of the WixSharp.Control.</returns>
        WixSharp.Controls.Control ToWControl();
    }

    /// <summary>
    /// UI Control types supported by wiX/MSI.
    /// </summary>
    public enum ControlType
    {
#pragma warning disable 1591
        Billboard,
        Bitmap,
        CheckBox,
        ComboBox,
        DirectoryCombo,
        DirectoryList,
        Edit,
        GroupBox,
        Icon,
        Line,
        ListBox,
        ListView,
        MaskedEdit,
        PathEdit,
        ProgressBar,
        PushButton,
        RadioButtonGroup,
        ScrollableText,
        SelectionTree,
        Text,
        VolumeCostList,
        VolumeSelectCombo,
#pragma warning restore 1591
    }
}
