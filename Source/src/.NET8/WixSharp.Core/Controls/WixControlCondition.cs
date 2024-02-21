using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing;
using System.Drawing.Design;
using System.Windows.Forms;
using Wix = WixSharp;
using System;

namespace WixSharp.Controls
{
    /// <summary>
    /// Defines Form designer compatible <c>Condition</c> associated with the WixControl action.
    /// </summary>
    [Serializable]
    public class WixControlCondition
    {
        /// <summary>
        /// Gets or sets the action.
        /// <para>Used only under Control elements and is required. Allows
        /// specific actions to be applied to a control based on the result of this condition. This attribute's value must be one of the following:</para>
        /// <para>- default</para>
        /// <para>- enable </para>
        /// <para>- disable</para>
        /// <para>- hide</para>
        /// <para>- show</para>
        /// </summary>
        /// <value>
        /// The action.
        /// </value>
        public ConditionAction Action { get; set; }

        /// <summary>
        /// Gets or sets the value for the action condition of the WiX Control element.
        /// </summary>
        /// <value>
        /// The value.
        /// </value>
        public string Value { get; set; }
    }

    /// <summary>
    /// Defines condition action type values for the WiX Control elements. 
    /// </summary>
    public enum ConditionAction
    {
#pragma warning disable 1591
        @default,
        enable,
        disable,
        hide,
        show
#pragma warning restore 1591
    }
}
