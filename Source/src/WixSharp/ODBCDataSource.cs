using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace WixSharp
{
    /// <summary>
    /// Represents an ODBCDataSource to be registered.
    /// 
    ///<example>The following is an example of using ODBCDataSource.
    ///<code>
    /// var project = 
    ///     new Project("My Product",
    ///     
    ///         new Dir(@"%ProgramFiles%\My Company\My Product",
    ///         ...
    ///         
    ///             new ODBCDataSource("DsnName", "SQL Server", true, true,
    ///                 new Property("Database", "MyDb"),
    ///                 new Property("Server", "MyServer")),
    ///                 
    ///         ...
    ///         
    /// Compiler.BuildMsi(project);
    /// </code>
    /// </example>
    /// </summary>
    public class ODBCDataSource : WixEntity
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ODBCDataSource"/> class.
        /// </summary>
        public ODBCDataSource()
        {
        }

        /// <summary>
        /// Creates instance of the <see cref="ODBCDataSource"></see> class with properties initialized with specified parameters.
        /// </summary>
        /// <param name="name">The name with which the odbc datasource is registered</param>
        /// <param name="driverName">The odbc driver named</param>
        /// <param name="keyPath">a boolean value indicating if the element is a KeyPath or not</param>
        /// <param name="perMachine">a boolean value to set machine or user level registration of the data source</param>
        /// <param name="items">Optional parameters defining properties for dsn registration.
        /// These are driver specific</param>
        /// 
        public ODBCDataSource(string name, string driverName, bool keyPath, bool perMachine, params WixEntity[] items)
        {
            Name = name;
            DriverName = driverName;
            KeyPath = keyPath;
            PerMachineRegistration = perMachine;

            AddItems(items);
        }

        /// <summary>
        /// Creates instance of the <see cref="ODBCDataSource"></see> class with properties initialized with specified parameters.
        /// </summary>
        /// <param name="feature">The feature the datasource will be added to</param>
        /// <param name="name">The name with which the odbc datasource is registered</param>
        /// <param name="driverName">The odbc driver named</param>
        /// <param name="keyPath">a boolean value indicating if the element is a KeyPath or not</param>
        /// <param name="perMachine">a boolean value to set machine or user level registration of the data source</param>
        /// <param name="items">Optional parameters defining properties for dsn registration.
        /// These are driver specific</param>
        /// 
        public ODBCDataSource(Feature feature, string name, string driverName, bool keyPath, bool perMachine, params WixEntity[] items)
        {
            Feature = feature;
            Name = name;
            DriverName = driverName;
            KeyPath = keyPath;
            PerMachineRegistration = perMachine;

            AddItems(items);
        }

        /// <summary>
        /// <see cref="Feature"></see> the file belongs to.
        /// </summary>
        public Feature Feature;

        /// <summary>
        /// String containing ODBC driver Name
        /// </summary>
        public string DriverName { get; set; }

        /// <summary>
        /// if true this is translated to 'yes' else 'no'
        ///  Set 'yes' to force this file to be key path for parent Component
        /// </summary>
        public bool KeyPath { get; set; }

        /// <summary>
        /// Scope for which the data source should be registered. This attribute's value must be one of the following:
        /// if true Data source is registered per machine
        /// else Data source is registered per user.
        /// </summary>
        public bool PerMachineRegistration { get; set; }

        /// <summary>
        /// Collection of WiX/MSI <see cref="Property"/> objects to be created during the installed.
        /// </summary>
        public Property[] Properties = new Property[0];


        void AddItems(WixEntity[] items)
        {
            var props = new List<Property>();

            foreach (WixEntity item in items)
            {
                if (item is Property)
                    props.Add(item as Property);
                else
                    throw new Exception(item.GetType().Name + " is not expected to be a child of WixSharp.ODBCDataSource");
            }


            Properties = props.ToArray();
        }
    }
}
