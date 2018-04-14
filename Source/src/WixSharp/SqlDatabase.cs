using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using WixSharp.CommonTasks;

namespace WixSharp
{
    /// <summary>
    /// Represents WixSqlExtension SqlDatabase element. The resulting XML representation may be rendered
    /// inside of a component element or under the product element. The parent element depends on the
    /// presence of a value for at least 1 property represented by values of SqlDbOption enumeration.
    /// </summary>
    public class SqlDatabase : WixEntity, IGenericEntity
    {
        #region Constructors

        /// <summary>
        /// Creates an instance of the <see cref="SqlDatabase"/> representing the database//>
        /// </summary>
        public SqlDatabase()
        {
        }

        /// <summary>
        /// Creates an instance of the <see cref="SqlDatabase" /> representing the database <paramref name="database" />@<paramref name="server" />
        /// </summary>
        /// <param name="database">The database.</param>
        /// <param name="server">The server.</param>
        /// <param name="children">The items.</param>
        /// <exception cref="System.ArgumentNullException">
        /// database;database is a null reference or empty
        /// or
        /// server;server is a null reference or empty
        /// </exception>
        public SqlDatabase(string database, string server, params IGenericEntity[] children)
        {
            if (string.IsNullOrEmpty(database)) throw new ArgumentNullException("database", "database is a null reference or empty");
            if (string.IsNullOrEmpty(server)) throw new ArgumentNullException("server", "server is a null reference or empty");

            Name = database;
            Database = database;
            Server = server;
            GenericItems = children;
        }

        /// <summary>
        /// Creates an instance of the <see cref="SqlDatabase" /> representing the database <paramref name="database" />@<paramref name="server" />
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <param name="database">The database.</param>
        /// <param name="server">The server.</param>
        /// <param name="children">The items.</param>
        public SqlDatabase(Id id, string database, string server, params IGenericEntity[] children)
            : this(database, server, children)
        {
            Id = id;
        }

        /// <summary>
        /// Creates an instance of the <see cref="SqlDatabase" /> representing the database <paramref name="database" />@<paramref name="server" />
        /// </summary>
        /// <param name="feature">The feature.</param>
        /// <param name="database">The database.</param>
        /// <param name="server">The server.</param>
        /// <param name="children">The items.</param>
        public SqlDatabase(Feature feature, string database, string server, params IGenericEntity[] children)
            : this(database, server, children)
        {
            Feature = feature;
        }

        /// <summary>
        /// Creates an instance of the <see cref="SqlDatabase"/> representing the database <paramref name="database"/>@<paramref name="server"/>
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <param name="feature">The feature.</param>
        /// <param name="database">The database.</param>
        /// <param name="server">The server.</param>
        /// <param name="children">The items.</param>
        public SqlDatabase(Id id, Feature feature, string database, string server, params IGenericEntity[] children)
            : this(database, server, children)
        {
            Id = id;
            Feature = feature;
        }

        /// <summary>
        /// Creates an instance of SqlDatbase representing the database <paramref name="database" />@<paramref name="server" />
        /// </summary>
        /// <param name="database">The database.</param>
        /// <param name="server">The server.</param>
        /// <param name="dbOptions">The database options.</param>
        /// <param name="children">The items.</param>
        public SqlDatabase(string database, string server, SqlDbOption dbOptions, params IGenericEntity[] children)
            : this(database, server, children)
        {
            SetSqlDbOptions(dbOptions);
        }

        /// <summary>
        /// Creates an instance of SqlDatbase representing the database <paramref name="database" />@<paramref name="server" />
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <param name="database">The database.</param>
        /// <param name="server">The server.</param>
        /// <param name="dbOptions">The database options.</param>
        /// <param name="children">The items.</param>
        public SqlDatabase(Id id, string database, string server, SqlDbOption dbOptions, params IGenericEntity[] children)
            : this(database, server, dbOptions, children)
        {
            Id = id;
        }

        /// <summary>
        /// Creates an instance of SqlDatbase representing the database <paramref name="database" />@<paramref name="server" />
        /// </summary>
        /// <param name="feature">The feature.</param>
        /// <param name="database">The database.</param>
        /// <param name="server">The server.</param>
        /// <param name="dbOptions">The database options.</param>
        /// <param name="children">The items.</param>
        public SqlDatabase(Feature feature, string database, string server, SqlDbOption dbOptions, params IGenericEntity[] children)
            : this(database, server, dbOptions, children)
        {
            Feature = feature;
        }

        /// <summary>
        /// Creates an instance of SqlDatbase representing the database <paramref name="database" />@<paramref name="server" />
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <param name="feature">The feature.</param>
        /// <param name="database">The database.</param>
        /// <param name="server">The server.</param>
        /// <param name="dbOptions">The database options.</param>
        /// <param name="children">The items.</param>
        public SqlDatabase(Id id, Feature feature, string database, string server, SqlDbOption dbOptions, params IGenericEntity[] children)
            : this(database, server, dbOptions, children)
        {
            Id = id;
            Feature = feature;
        }

        private void SetSqlDbOptions(SqlDbOption options)
        {
            if ((options & SqlDbOption.CreateOnInstall) == SqlDbOption.CreateOnInstall) CreateOnInstall = true;
            if ((options & SqlDbOption.CreateOnReinstall) == SqlDbOption.CreateOnReinstall) CreateOnReinstall = true;
            if ((options & SqlDbOption.CreateOnUninstall) == SqlDbOption.CreateOnUninstall) CreateOnUninstall = true;
            if ((options & SqlDbOption.DropOnInstall) == SqlDbOption.DropOnInstall) DropOnInstall = true;
            if ((options & SqlDbOption.DropOnReinstall) == SqlDbOption.DropOnReinstall) DropOnReinstall = true;
            if ((options & SqlDbOption.DropOnUninstall) == SqlDbOption.DropOnUninstall) DropOnUninstall = true;
        }

        #endregion Constructors

        /// <summary>
        /// Collection of the nested user defined <see cref="IGenericEntity"/> items.
        /// </summary>
        public IGenericEntity[] GenericItems = new IGenericEntity[0];

        #region Wix SqlDatabase attributes

        /// <summary>
        /// Primary key used to identify this particular entry.
        /// </summary>
        [Xml]
        public new string Id { get { return base.Id; } set { base.Id = value; } }

        /// <summary>
        /// Maps to the ConfirmOverwrite property of SqlDatabase
        /// </summary>
        [Xml]
        public bool? ConfirmOverwrite;

        /// <summary>
        /// Maps to the ContinueOnError property of SqlDatabase
        /// </summary>
        [Xml]
        public bool? ContinueOnError;

        /// <summary>
        /// Maps to the CreateOnInstall property of SqlDatabase
        /// </summary>
        [Xml]
        public bool? CreateOnInstall;

        /// <summary>
        /// Maps to the CreateOnReinstall property of SqlDatabase
        /// </summary>
        [Xml]
        public bool? CreateOnReinstall;

        /// <summary>
        /// Maps to the CreateOnUninstall property of SqlDatabase
        /// </summary>
        [Xml]
        public bool? CreateOnUninstall;

        /// <summary>
        /// Maps to the Database property of SqlDatabase
        /// </summary>
        [Xml]
        public string Database; //required

        /// <summary>
        /// Maps to the DropOnInstall property of SqlDatabase
        /// </summary>
        [Xml]
        public bool? DropOnInstall;

        /// <summary>
        /// Maps to the DropOnReinstall property of SqlDatabase
        /// </summary>
        [Xml]
        public bool? DropOnReinstall;

        /// <summary>
        /// Maps to the DropOnUninstall property of SqlDatabase
        /// </summary>
        [Xml]
        public bool? DropOnUninstall;

        /// <summary>
        /// Maps to the Instance property of SqlDatabase
        /// </summary>
        [Xml]
        public string Instance;

        /// <summary>
        /// Maps to the Server property of SqlDatabase
        /// </summary>
        [Xml]
        public string Server; //required

        /// <summary>
        /// Maps to the User property of SqlDatabase
        /// </summary>
        [Xml]
        public string User;

        #endregion Wix SqlDatabase attributes

        internal bool MustDescendFromComponent
        {
            get
            {
                return CreateOnInstall.HasValue
                       || CreateOnReinstall.HasValue
                       || CreateOnUninstall.HasValue
                       || DropOnInstall.HasValue
                       || DropOnReinstall.HasValue
                       || DropOnUninstall.HasValue;
            }
        }

        /// <summary>
        /// Adds itself as an XML content into the WiX source being generated from the <see cref="WixSharp.Project"/>.
        /// See 'Wix#/samples/Extensions' sample for the details on how to implement this interface correctly.
        /// </summary>
        /// <param name="context">The context.</param>
        public void Process(ProcessingContext context)
        {
            context.Project.Include(WixExtension.Sql);

            if (MustDescendFromComponent)
            {
                XElement component = this.CreateParentComponent();
                XElement sqlDatabase = this.ToXElement(WixExtension.Sql, "SqlDatabase");
                component.Add(sqlDatabase);

                if (GenericItems.Any())
                {
                    var newContext = new ProcessingContext
                    {
                        Project = context.Project,
                        Parent = this,
                        XParent = sqlDatabase,
                        FeatureComponents = context.FeatureComponents,
                    };

                    foreach (IGenericEntity item in GenericItems)
                        item.Process(newContext);
                }

                context.XParent.FindFirst("Component").Parent?.Add(component);
                MapComponentToFeatures(component.Attr("Id"), ActualFeatures, context);
            }
            else
            {
                context.XParent.Add(this.ToXElement(WixExtension.Sql, "SqlDatabase"));
            }
        }
    }

    /// <summary>
    /// Represents Execution options for SqlScript and SqlString elements
    /// </summary>
    /// <remarks>
    /// Attributes represented by this enum will be rendered as having the value 'yes'.
    /// If a value of 'no' is required, set the property directly after construction.
    /// </remarks>
    [Flags]
    public enum ExecuteSql
    {
#pragma warning disable 1591
        None = 0,
        OnInstall = 1,
        OnReinstall = 2,
        OnUninstall = 4
#pragma warning restore 1591
    }

    /// <summary>
    /// Represents Rollback options for SqlScript and SqlString elements
    /// </summary>
    /// <remarks>
    /// Attributes represented by this enum will be rendered as having the value 'yes'.
    /// If a value of 'no' is required, set the property directly after construction.
    /// </remarks>
    [Flags]
    public enum RollbackSql
    {
#pragma warning disable 1591
        None = 0,
        OnInstall = 1,
        OnReinstall = 2,
        OnUninstall = 4
#pragma warning restore 1591
    }

    /// <summary>
    /// Attributes represented by this enum will be rendered as having the value 'yes'.
    /// If a value of 'no' is required, set the property directly after construction.
    /// </summary>
    [Flags]
    public enum SqlDbOption
    {
#pragma warning disable 1591
        None = 0,
        CreateOnInstall = 1,
        CreateOnReinstall = 2,
        CreateOnUninstall = 4,
        DropOnInstall = 8,
        DropOnReinstall = 16,
        DropOnUninstall = 32
#pragma warning restore 1591
    }
}