namespace WixSharp.Controls
{
    public static partial class Extensions
    {
        public static void CopyCommonPropertiesFrom(this WixSharp.Controls.Control destControl, IWixControl srcControl)
        {
            var formControl = (System.Windows.Forms.Control)srcControl;

            if (srcControl.Conditions != null)
                destControl.Conditions.AddRange(srcControl.Conditions);

            destControl.Height = formControl.Size.Height.WScale();
            destControl.Width = formControl.Size.Width.WScale();
            destControl.X = formControl.Left.WScale();
            destControl.Y = formControl.Top.WScale();

            destControl.Disabled = !formControl.Enabled;
            destControl.Hidden = srcControl.Hidden;
            destControl.AttributesDefinition = srcControl.WixAttributes;
            destControl.Property = srcControl.BoundProperty;

            if (srcControl is WixControl)
                destControl.EmbeddedXML = (srcControl as WixControl).EmbeddedXML;

            if (!formControl.Text.IsEmpty())
                destControl.Text = formControl.Text;

            if (!srcControl.Tooltip.IsEmpty())
                destControl.Tooltip = srcControl.Tooltip;

            destControl.Name = formControl.Name.IsNullOrEmpty() ? destControl.Type : formControl.Name;
            if (!srcControl.Id.IsNullOrEmpty())
                destControl.Id = srcControl.Id; //destControl.Id is a calculated property and if not set explicitly it will fell back to the destControl.Name

            if (srcControl is IWixInteractiveControl)
            {
                destControl.Actions.AddRange((srcControl as IWixInteractiveControl).Actions);
            }
        }

        public static WixSharp.Controls.Control ConvertToWControl(this IWixControl srcControl, ControlType controlType)
        {
            var wControl = new WixSharp.Controls.Control { Type = controlType.ToString() };

            wControl.CopyCommonPropertiesFrom(srcControl);

            return wControl;
        }

        /// <summary>
        /// The WiX UI scaling factor.
        /// WiX pixels are not the same as Windows ones. For example Wix.Point(56,17) is equivalent of the Win.Point(75,23)
        /// </summary>
        static public double WixScalingFactor = 0.756;

        /// <summary>
        /// Converts (scales) Windows coordinate value into WiX one.
        /// </summary>
        /// <param name="data">The data.</param>
        /// <returns></returns>
        internal static int WScale(this int data)
        {
            return (int)(data * WixScalingFactor);
        }
    }

    //VS designer property grid does not like if the control designer attribute type is generic so make it a concrete type
#pragma warning disable 1591

    public class WixControlDesigner : WixControlDesigner<WixControl> { }
    public class WixButtonDesigner : WixControlDesigner<WixButton> { }
    public class WixLabelDesigner : WixControlDesigner<WixLabel> { }
    public class WixFormDesigner : WixControlDesigner<WixForm> { }
    public class WixCheckBoxDesigner : WixControlDesigner<WixCheckBox> { }
    public class WixTextBoxDesigner : WixControlDesigner<WixTextBox> { }
#pragma warning restore 1591
}