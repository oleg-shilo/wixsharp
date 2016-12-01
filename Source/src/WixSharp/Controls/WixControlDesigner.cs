using System.Windows.Forms;
using System.Xml.Linq;
using System.Linq;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms.Design;
using System.Collections.Generic;
using System.Reflection;
using System.IO;
using System;
using System.Text;

namespace WixSharp.Controls
{
    /// <summary>
    /// Implements Form Designer support for the WixSharp MSI UI controls. 
    /// </summary>
    /// <typeparam name="T">WinForm control to be supported by Form Designer.</typeparam>
    public class WixControlDesigner<T> : ControlDesigner
    {
        //do not include Visible as it is not a design time property but a runtime one. 
        //For example if it is set to true it is false just after the instantiation but will 
        //become true when the form becomes visible.
        /// <summary>
        /// The ';' separated list of the <see cref="T:System.Windows.Forms.Control" /> properties 
        /// to be visible in the properties grid by default.
        /// </summary>
        public const string VisibleBaseProperties = "Width;Height;Location;Enabled;Text;Name;Size";

        /// <summary>
        /// Adjusts the set of properties the component exposes through a <see cref="T:System.ComponentModel.TypeDescriptor" />.
        /// </summary>
        /// <param name="properties">An <see cref="T:System.Collections.IDictionary" /> containing the properties for the class of the component.</param>
        protected override void PreFilterProperties(IDictionary properties)
        {
            base.PreFilterProperties(properties);

            var designerProperties = new List<string>(VisibleBaseProperties.Split(';'));

            var hiddenProperties = new List<string>();

            foreach (string key in properties.Keys)
            {
                if ((properties[key] as PropertyDescriptor).ComponentType != typeof(T))
                {
                    if (!designerProperties.Contains(key) && !key.StartsWith("Name_")) //all "(Name)" rows in the PropertyGrid have keys starting with "Name_"
                    {
                        hiddenProperties.Add(key);
                    }
                }
            }

            foreach (string name in hiddenProperties)
                properties.Remove(name);
        }

        /// <summary>
        /// Allows a designer to add to the set of events that it exposes through a <see cref="T:System.ComponentModel.TypeDescriptor" />.
        /// </summary>
        /// <param name="events">The events for the class of the component.</param>
        protected override void PreFilterEvents(IDictionary events)
        {
            base.PreFilterEvents(events);

            var baseClassEvents = new List<object>();

            foreach (var item in events.Keys)
                if ((events[item] as EventDescriptor).ComponentType != typeof(T))
                    baseClassEvents.Add(item);

            foreach (var item in baseClassEvents)
                events.Remove(item);
        }
    }
}
