using System.Collections.Generic;
using System.Xml.Linq;

namespace WixSharp.Controls
{
    /// <summary>
    /// Defines generic WiX Control.  
    /// </summary>
    public partial class Control : WixEntity
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Control"/> class.
        /// </summary>
        public Control()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Control"/> class.
        /// </summary>
        /// <param name="id">The <c>Control</c> id.</param>
        public Control(Id id)
        {
            base.Id = id;
        }

        /// <summary>
        /// The type of the control. Could be one of the following: Billboard, Bitmap, CheckBox, ComboBox, DirectoryCombo, DirectoryList, Edit, GroupBox, Icon, Line, ListBox, ListView, MaskedEdit, PathEdit, ProgressBar, PushButton, RadioButtonGroup, ScrollableText, SelectionTree, Text, VolumeCostList, VolumeSelectCombo.
        /// Use <see cref="WixSharp.Controls.ControlType"></see> constants to define the type of the control.
        /// </summary>
        public string Type;

        /// <summary>
        /// Horizontal coordinate of the upper-left corner of the rectangular boundary of the control. This must be a non-negative number.
        /// </summary>
        public int X;

        /// <summary>
        /// Width of the rectangular boundary of the control. This must be a non-negative number.
        /// </summary>
        public int Y;

        /// <summary>
        /// Vertical coordinate of the upper-left corner of the rectangular boundary of the control. This must be a non-negative number.
        /// </summary>
        public int Height;

        /// <summary>
        /// Width of the rectangular boundary of the control. This must be a non-negative number.
        /// </summary>
        public int Width;

        /// <summary>
        /// A localizable string used to set the initial text contained in a control.
        /// This attribute can contain a formatted string that is processed at install time to insert the values of properties using [PropertyName] syntax. Also supported are environment variables, file installation paths, and component installation directories.
        /// </summary>
        public string Text;

        /// <summary>
        /// Sets whether the control is hidden.
        /// </summary>
        public bool Hidden;

        /// <summary>
        /// Sets whether the control is disabled.
        /// </summary>
        public bool Disabled;

        /// <summary>
        /// The name of a defined property to be linked to this control. This column is required for active controls.
        /// </summary>
        public string Property;

        /// <summary>
        /// The raw XML string to be embedded into WiX <c>Control</c> element.
        /// </summary>
        public string EmbeddedXML;

        /// <summary>
        /// The string used for the Tooltip.
        /// </summary>
        public string Tooltip;

        /// <summary>
        /// Conditions for the control.
        /// </summary>
        public List<WixControlCondition> Conditions = new List<WixControlCondition>();

        /// <summary>
        /// The actions.
        /// </summary>
        public List<ControlActionData> Actions = new List<ControlActionData>();

        /// <summary>
        /// Converts the <see cref="T:WixSharp.Control"/> instance into WiX <see cref="T:System.Xml.Linq.XElement"/>.
        /// </summary>
        /// <returns><see cref="T:System.Xml.Linq.XElement"/> instance.</returns>
        public virtual XElement ToXElement()
        {
            var control =
                new XElement("Control",
                    new XAttribute("Id", this.Id),
                    new XAttribute("Width", this.Width),
                    new XAttribute("Height", this.Height),
                    new XAttribute("X", this.X),
                    new XAttribute("Y", this.Y),
                    new XAttribute("Type", this.Type))
                    .AddAttributes(this.Attributes);

            if (!Property.IsEmpty()) control.Add(new XAttribute("Property", this.Property));

            if (Hidden) control.Add(new XAttribute("Hidden", this.Hidden.ToYesNo()));

            if (Disabled) control.Add(new XAttribute("Disabled", this.Disabled.ToYesNo()));

            if (!Tooltip.IsEmpty()) control.Add(new XAttribute("ToolTip", this.Tooltip));
            if (!Text.IsEmpty()) control.Add(new XElement("Text", this.Text));

            if (!EmbeddedXML.IsEmpty())
            {
                foreach (var element in XDocument.Parse("<root>" + EmbeddedXML + "</root>").Root.Elements())
                    control.Add(element);
            }

            int index = 0;
            foreach (ControlActionData data in Actions)
            {
                index++;
                var publishElement = control.AddElement(new XElement("Publish",
                                                            new XAttribute("Value", data.Value),
                                                            new XAttribute("Order", index),
                                                            data.Condition));
                if (!data.Event.IsEmpty())
                    publishElement.Add(new XAttribute("Event", data.Event));

                if (!data.Property.IsEmpty())
                    publishElement.Add(new XAttribute("Property", data.Property));
            }

            foreach (WixControlCondition condition in Conditions)
            {
                control.AddElement(new XElement("Condition",
                                       new XAttribute("Action", condition.Action.ToString()),
                                       condition.Value));
            }

            return control;
        }
    }
}
