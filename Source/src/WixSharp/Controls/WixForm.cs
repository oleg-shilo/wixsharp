using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace WixSharp.Controls
{
    /// <summary>
    /// Defines <see cref="T:System.Windows.Forms" /> Form for generating WiX Dialog element.
    /// <para>
    /// The <see cref="T:WixSharp.WixForm" /> can be used with the <see cref="T:System.Windows.Forms" /> designer to define custom
    /// dialog layouts. The <see cref="T:WixSharp.Compiler" /> uses <see cref="T:WixSharp.WixForm" /> instance at compile time
    /// to generate WiX Dialog element on the base of this instance properties.
    /// </para>
    /// </summary>
    [Designer(typeof(WixButtonDesigner))]
    public class WixForm : Form
    {
        /// <summary>
        /// The dialog actions
        /// </summary>
        List<ControlActionData> dialogActions = new List<ControlActionData>();

        /// <summary>
        /// Initializes a new instance of the <see cref="WixForm"/> class.
        /// </summary>
        public WixForm()
        {
            this.SizeChanged += (x, y) => { WixSize = new Size(this.ClientSize.Width.WScale(), this.ClientSize.Height.WScale()); };
        }

        /// <summary>
        /// Gets the size of the <c>Control</c>.
        /// </summary>
        /// <value>
        /// The size of the <c>Control</c>.
        /// </value>
        public Size WixSize { get; private set; }

        /// <summary>
        /// Generates and adds the action to the list of the dialog actions, which is to be used at compile time for 
        /// generating WiX Dialog element actions.
        /// <code>
        /// void wixButton_Click()
        /// {
        ///     this.Do(ControlAction.DoAction, "ClaimLicenceKey");
        /// }
        /// </code>
        /// </summary>
        /// <param name="action">The action type.</param>
        /// <param name="value">The action value.</param>
        /// <param name="property">The property name.</param>
        /// <param name="condition">The action condition.</param>
        public void Do(string action, string value, string property = null, string condition = "1")
        {
            if (!action.IsEmpty())
                dialogActions.Add(new ControlActionData { Event = action, Value = value, Condition = condition });

            if (!property.IsEmpty())
                dialogActions.Add(new ControlActionData { Property = property, Value = value, Condition = condition });
        }

        /// <summary>
        /// Generates and adds the EndDialog action to the list of the dialog actions, which is to be used at compile time for 
        /// generating WiX Dialog element actions.
        /// <code>
        /// void wixButton_Click()
        /// {
        ///     this.EndDialog(EndDialogValue.Return);
        /// }
        /// </code>
        /// </summary>
        /// <param name="value">The action value.</param>
        /// <param name="condition">The action condition.</param>
        public void EndDialog(string value, string condition = "1")
        {
            dialogActions.Add(new ControlActionData { Event = ControlAction.EndDialog, Value = value, Condition = condition });
        }

        /// <summary>
        /// Generates and adds the SetProperty action to the list of the dialog actions, which is to be used at compile time for 
        /// generating WiX Dialog element actions.
        /// </summary>
        /// <param name="name">The property name.</param>
        /// <param name="value">The property value.</param>
        /// <param name="condition">The action condition.</param>
        public void SetPrperty(string name, string value, string condition = "1")
        {
            dialogActions.Add(new ControlActionData { Property = name, Value = value, Condition = condition });
        }

        /// <summary>
        /// Gets or sets the id.
        /// </summary>
        /// <value>
        /// The id.
        /// </value>
        public string Id { get; set; }

        /// <summary>
        /// Gets or sets the WiX element attributes.
        /// </summary>
        /// <value>
        /// The WiX element attributes.
        /// </value>
        public string WixAttributes { get; set; }

        /// <summary>
        /// Converts the WixForm (WinForm) instance into WiX custom UI control WixSharp.<see cref="T:WixSharp.Dialog"/>.
        /// </summary>
        /// <returns>WixSharp.<see cref="T:WixSharp.Dialog"/> instance.</returns>
        public Dialog ToWDialog()
        {
            var wDialog = new Dialog
            {
                Height = this.ClientRectangle.Size.Height.WScale(),
                Width = this.ClientRectangle.Size.Width.WScale(),
                AttributesDefinition = this.WixAttributes,
                Title = this.Text
            };

            foreach (var control in this.Controls)
                if (control is IWixControl)
                {
                    if (control is WixButton)
                    {
                        var button = control as WixButton;

                        this.dialogActions.Clear();

                        button.Actions.Clear();

                        button.PerformClick();
                        button.Actions.AddRange(this.dialogActions);
                    }
                }

            wDialog.Name = this.Name.IsNullOrEmpty() ? "Dialog" : this.Name;

            if (!this.Id.IsNullOrEmpty())
                wDialog.Id = this.Id;

            var wControls = new List<WixSharp.Controls.Control>();

            foreach (var item in this.Controls)
                if (item is IWixControl)
                    wControls.Add((item as IWixControl).ToWControl());

            wDialog.Controls = wControls.ToArray();

            return wDialog;
        }
    }

}
