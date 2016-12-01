using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace WixSharp
{
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

    /// <summary>
    /// Represents WixSqlExtension SqlDatabase element. The resulting XML representation may be rendered
    /// inside of a component element or under the product element. The parent element depends on the
    /// presence of a value for at least 1 property represented by values of SqlDbOption enumeration.
    /// </summary>
    public class SqlDatabase : WixEntity
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
        /// <param name="items">The items.</param>
        /// <exception cref="System.ArgumentNullException">
        /// database;database is a null reference or empty
        /// or
        /// server;server is a null reference or empty
        /// </exception>
        public SqlDatabase(string database, string server, params WixEntity[] items)
        {
            if (string.IsNullOrEmpty(database)) throw new ArgumentNullException("database", "database is a null reference or empty");
            if (string.IsNullOrEmpty(server)) throw new ArgumentNullException("server", "server is a null reference or empty");

            Name = database;
            Database = database;
            Server = server;

            AddItems(items);
        }

        /// <summary>
        /// Creates an instance of the <see cref="SqlDatabase" /> representing the database <paramref name="database" />@<paramref name="server" />
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <param name="database">The database.</param>
        /// <param name="server">The server.</param>
        /// <param name="items">The items.</param>
        public SqlDatabase(Id id, string database, string server, params WixEntity[] items)
            : this(database, server, items)
        {
            Id = id;
        }

        /// <summary>
        /// Creates an instance of the <see cref="SqlDatabase" /> representing the database <paramref name="database" />@<paramref name="server" />
        /// </summary>
        /// <param name="feature">The feature.</param>
        /// <param name="database">The database.</param>
        /// <param name="server">The server.</param>
        /// <param name="items">The items.</param>
        public SqlDatabase(Feature feature, string database, string server, params WixEntity[] items)
            : this(database, server, items)
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
        /// <param name="items">The items.</param>
        public SqlDatabase(Id id, Feature feature, string database, string server, params WixEntity[] items)
            : this(database, server, items)
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
        /// <param name="items">The items.</param>
        public SqlDatabase(string database, string server, SqlDbOption dbOptions, params WixEntity[] items)
            : this(database, server, items)
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
        /// <param name="items">The items.</param>
        public SqlDatabase(Id id, string database, string server, SqlDbOption dbOptions, params WixEntity[] items)
            : this(database, server, dbOptions, items)
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
        /// <param name="items">The items.</param>
        public SqlDatabase(Feature feature, string database, string server, SqlDbOption dbOptions, params WixEntity[] items)
            : this(database, server, dbOptions, items)
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
        /// <param name="items">The items.</param>
        public SqlDatabase(Id id, Feature feature, string database, string server, SqlDbOption dbOptions, params WixEntity[] items)
            : this(database, server, dbOptions, items)
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
        /// <see cref="Feature"></see> the SqlDatabase belongs to.
        /// </summary>
        public Feature Feature { get; set; }

        /// <summary>
        /// The SQL scripts
        /// </summary>
        public SqlScript[] SqlScripts = new SqlScript[0];
        /// <summary>
        /// The SQL strings
        /// </summary>
        public SqlString[] SqlStrings = new SqlString[0];

        #region Wix SqlDatabase attributes

        /// <summary>
        /// Maps to the ConfirmOverwrite property of SqlDatabase
        /// </summary>
        public bool? ConfirmOverwrite { get; set; }

        /// <summary>
        /// Maps to the ContinueOnError property of SqlDatabase
        /// </summary>
        public bool? ContinueOnError { get; set; }

        /// <summary>
        /// Maps to the CreateOnInstall property of SqlDatabase
        /// </summary>
        public bool? CreateOnInstall { get; set; }

        /// <summary>
        /// Maps to the CreateOnReinstall property of SqlDatabase
        /// </summary>
        public bool? CreateOnReinstall { get; set; }

        /// <summary>
        /// Maps to the CreateOnUninstall property of SqlDatabase
        /// </summary>
        public bool? CreateOnUninstall { get; set; }

        /// <summary>
        /// Maps to the Database property of SqlDatabase
        /// </summary>
        public string Database { get; set; } //required

        /// <summary>
        /// Maps to the DropOnInstall property of SqlDatabase
        /// </summary>
        public bool? DropOnInstall { get; set; }

        /// <summary>
        /// Maps to the DropOnReinstall property of SqlDatabase
        /// </summary>
        public bool? DropOnReinstall { get; set; }

        /// <summary>
        /// Maps to the DropOnUninstall property of SqlDatabase
        /// </summary>
        public bool? DropOnUninstall { get; set; }

        /// <summary>
        /// Maps to the Instance property of SqlDatabase
        /// </summary>
        public string Instance { get; set; }

        /// <summary>
        /// Maps to the Server property of SqlDatabase
        /// </summary>
        public string Server { get; set; } //required

        /// <summary>
        /// Maps to the User property of SqlDatabase
        /// </summary>
        public string User { get; set; }

        #endregion Wix SqlDatabase attributes

        private void AddItems(IEnumerable<WixEntity> items)
        {
            SqlStrings = items.OfType<SqlString>().ToArray();
            SqlScripts = items.OfType<SqlScript>().ToArray();

            var unexpectedItem =
                items.Except(SqlStrings)
                     .Except(SqlScripts)
                     .FirstOrDefault();

            if (unexpectedItem != null)
                throw new ApplicationException(
                    string.Format("{0} is unexpected. Only {1} and {2} items can be added to {3}",
                        unexpectedItem,
                        typeof(SqlScript),
                        typeof(SqlString),
                        this.GetType()));
        }

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
    /// Represents WixSqlExtension element SqlString
    /// </summary>
    public class SqlString : WixEntity
    {
        #region Constructors

        /// <summary>
        /// Creates an instance of SqlString
        /// </summary>
        public SqlString()
        {
        }

        private SqlString(string sql)
        {
            if (string.IsNullOrEmpty(sql)) throw new ArgumentNullException("sql", "sql is a null reference or empty");

            Name = "SqlString";
            Sql = sql;
        }

        /// <summary>
        /// Creates an instance of SqlString from <paramref name="sql"/> to be execute according to <paramref name="executeOptions"/>
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="executeOptions"></param>
        public SqlString(string sql, ExecuteSql executeOptions)
            : this(sql)
        {
            SetExecutionOptions(executeOptions);
        }

        /// <summary>
        /// Creates an instance of SqlString from <paramref name="sql"/> to be execute according to <paramref name="executeOptions"/>
        /// </summary>
        /// <param name="id"></param>
        /// <param name="sql"></param>
        /// <param name="executeOptions"></param>
        public SqlString(Id id, string sql, ExecuteSql executeOptions)
            : this(sql, executeOptions)
        {
            Id = id;
        }

        /// <summary>
        /// Creates an instance of SqlString from <paramref name="sql"/> to be execute according to <paramref name="executeOptions"/>
        /// </summary>
        /// <param name="feature"></param>
        /// <param name="sql"></param>
        /// <param name="executeOptions"></param>
        public SqlString(Feature feature, string sql, ExecuteSql executeOptions)
            : this(sql, executeOptions)
        {
            Feature = feature;
        }

        /// <summary>
        /// Creates an instance of SqlString from <paramref name="sql"/> to be execute according to <paramref name="executeOptions"/>
        /// </summary>
        /// <param name="id"></param>
        /// <param name="feature"></param>
        /// <param name="sql"></param>
        /// <param name="executeOptions"></param>
        public SqlString(Id id, Feature feature, string sql, ExecuteSql executeOptions)
            : this(sql, executeOptions)
        {
            Id = id;
            Feature = feature;
        }

        /// <summary>
        /// Creates an instance of SqlString from <paramref name="sql"/> to be rolled-back according to <paramref name="rollbackOptions"/>
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="rollbackOptions"></param>
        public SqlString(string sql, RollbackSql rollbackOptions)
            : this(sql)
        {
            SetRollbackOptions(rollbackOptions);
        }

        /// <summary>
        /// Creates an instance of SqlString from <paramref name="sql"/> to be rolled-back according to <paramref name="rollbackOptions"/>
        /// </summary>
        /// <param name="id"></param>
        /// <param name="sql"></param>
        /// <param name="rollbackOptions"></param>
        public SqlString(Id id, string sql, RollbackSql rollbackOptions)
            : this(sql)
        {
            Id = id;
            SetRollbackOptions(rollbackOptions);
        }

        /// <summary>
        /// Creates an instance of SqlString from <paramref name="sql"/> to be rolled-back according to <paramref name="rollbackOptions"/>
        /// </summary>
        /// <param name="feature"></param>
        /// <param name="sql"></param>
        /// <param name="rollbackOptions"></param>
        public SqlString(Feature feature, string sql, RollbackSql rollbackOptions)
            : this(sql)
        {
            Feature = feature;
            SetRollbackOptions(rollbackOptions);
        }

        /// <summary>
        /// Creates an instance of SqlString from <paramref name="sql"/> to be rolled-back according to <paramref name="rollbackOptions"/>
        /// </summary>
        /// <param name="id"></param>
        /// <param name="feature"></param>
        /// <param name="sql"></param>
        /// <param name="rollbackOptions"></param>
        public SqlString(Id id, Feature feature, string sql, RollbackSql rollbackOptions)
            : this(sql)
        {
            Id = id;
            Feature = feature;
            SetRollbackOptions(rollbackOptions);
        }

        #endregion Constructors

        /// <summary>
        /// <see cref="Feature"></see> the SqlString belongs to.
        /// </summary>
        /// <value>
        /// The feature.
        /// </value>
        public Feature Feature { get; set; }

        #region Wix SqlString attributes

        /// <summary>
        /// Maps to the ContinueOnError property of SqlString
        /// </summary>
        public bool? ContinueOnError { get; set; }

        /// <summary>
        /// Maps to the ExecuteOnInstall property of SqlString
        /// </summary>
        public bool? ExecuteOnInstall { get; set; } //partially required

        /// <summary>
        /// Maps to the ExecuteOnReinstall property of SqlString
        /// </summary>
        public bool? ExecuteOnReinstall { get; set; } //partially required

        /// <summary>
        /// Maps to the ExecuteOnUninstall property of SqlString
        /// </summary>
        public bool? ExecuteOnUninstall { get; set; } //partially required

        /// <summary>
        /// Maps to the RollbackOnInstall property of SqlString
        /// </summary>
        public bool? RollbackOnInstall { get; set; } //partially required

        /// <summary>
        /// Maps to the RollbackOnReinstall property of SqlString
        /// </summary>
        public bool? RollbackOnReinstall { get; set; } //partially required

        /// <summary>
        /// Maps to the RollbackOnUninstall property of SqlString
        /// </summary>
        public bool? RollbackOnUninstall { get; set; } //partially required

        /// <summary>
        /// Maps to the Sequence property of SqlString
        /// </summary>
        public int? Sequence { get; set; }

        /// <summary>
        /// Maps to the Sql property of SqlString
        /// </summary>
        public string Sql { get; set; } //required

        /// <summary>
        /// Maps to the SqlDb property of SqlString. This property is to be inferred from the containing SqlDatabase element.
        /// </summary>
        internal string SqlDb { get; set; } //required when not under a SqlDatabase element

        /// <summary>
        /// Maps to the User property of SqlString
        /// </summary>
        public string User { get; set; }

        #endregion Wix SqlString attributes

        private void SetExecutionOptions(ExecuteSql executeOptions)
        {
            if (executeOptions == ExecuteSql.None) throw new ArgumentException("None is invalid. It has no legal representation in Wix", "executeOptions");

            if ((executeOptions & ExecuteSql.OnInstall) == ExecuteSql.OnInstall) ExecuteOnInstall = true;
            if ((executeOptions & ExecuteSql.OnReinstall) == ExecuteSql.OnReinstall) ExecuteOnReinstall = true;
            if ((executeOptions & ExecuteSql.OnUninstall) == ExecuteSql.OnUninstall) ExecuteOnUninstall = true;
        }

        private void SetRollbackOptions(RollbackSql rollbackOption)
        {
            if (rollbackOption == RollbackSql.None) throw new ArgumentException("None is invalid. It has no legal representation in Wix", "rollbackOption");

            if ((rollbackOption & RollbackSql.OnInstall) == RollbackSql.OnInstall) RollbackOnInstall = true;
            if ((rollbackOption & RollbackSql.OnReinstall) == RollbackSql.OnReinstall) RollbackOnReinstall = true;
            if ((rollbackOption & RollbackSql.OnUninstall) == RollbackSql.OnUninstall) RollbackOnUninstall = true;
        }
    }

    /// <summary>
    /// Represents WixSqlExtension element SqlScript
    /// </summary>
    public class SqlScript : WixEntity
    {
        #region Constructors

        /// <summary>
        /// Creates an instance of SqlScript
        /// </summary>
        public SqlScript()
        {
        }

        private SqlScript(string binaryKey)
        {
            if (string.IsNullOrEmpty(binaryKey)) throw new ArgumentNullException("binaryKey", "binaryKey is a null reference or empty");

            BinaryKey = binaryKey;
            Name = binaryKey;
        }

        /// <summary>
        /// Creates an instance of SqlScript for <paramref name="binaryKey"/> to be execute according to <paramref name="executeOptions"/>
        /// </summary>
        /// <param name="binaryKey"></param>
        /// <param name="executeOptions"></param>
        public SqlScript(string binaryKey, ExecuteSql executeOptions)
            : this(binaryKey)
        {
            SetExecutionOptions(executeOptions);
        }

        /// <summary>
        /// Creates an instance of SqlScript for <paramref name="binaryKey"/> to be execute according to <paramref name="executeOptions"/>
        /// </summary>
        /// <param name="id"></param>
        /// <param name="binaryKey"></param>
        /// <param name="executeOptions"></param>
        public SqlScript(Id id, string binaryKey, ExecuteSql executeOptions)
            : this(binaryKey, executeOptions)
        {
            Id = id;
        }

        /// <summary>
        /// Creates an instance of SqlScript for <paramref name="binaryKey"/> to be execute according to <paramref name="executeOptions"/>
        /// </summary>
        /// <param name="feature"></param>
        /// <param name="binaryKey"></param>
        /// <param name="executeOptions"></param>
        public SqlScript(Feature feature, string binaryKey, ExecuteSql executeOptions)
            : this(binaryKey, executeOptions)
        {
            Feature = feature;
        }

        /// <summary>
        /// Creates an instance of SqlScript for <paramref name="binaryKey"/> to be execute according to <paramref name="executeOptions"/>
        /// </summary>
        /// <param name="id"></param>
        /// <param name="feature"></param>
        /// <param name="binaryKey"></param>
        /// <param name="executeOptions"></param>
        public SqlScript(Id id, Feature feature, string binaryKey, ExecuteSql executeOptions)
            : this(binaryKey, executeOptions)
        {
            Id = id;
            Feature = feature;
        }

        /// <summary>
        /// Creates an instance of SqlScript for <paramref name="binaryKey"/> to be rolled-back according to <paramref name="rollbackOptions"/>
        /// </summary>
        /// <param name="binaryKey"></param>
        /// <param name="rollbackOptions"></param>
        public SqlScript(string binaryKey, RollbackSql rollbackOptions)
            : this(binaryKey)
        {
            SetRollbackOptions(rollbackOptions);
        }

        /// <summary>
        /// Creates an instance of SqlScript for <paramref name="binaryKey"/> to be rolled-back according to <paramref name="rollbackOptions"/>
        /// </summary>
        /// <param name="id"></param>
        /// <param name="binaryKey"></param>
        /// <param name="rollbackOptions"></param>
        public SqlScript(Id id, string binaryKey, RollbackSql rollbackOptions)
            : this(binaryKey, rollbackOptions)
        {
            Id = id;
        }

        /// <summary>
        /// Creates an instance of SqlScript for <paramref name="binaryKey"/> to be rolled-back according to <paramref name="rollbackOptions"/>
        /// </summary>
        /// <param name="feature"></param>
        /// <param name="binaryKey"></param>
        /// <param name="rollbackOptions"></param>
        public SqlScript(Feature feature, string binaryKey, RollbackSql rollbackOptions)
            : this(binaryKey, rollbackOptions)
        {
            Feature = feature;
        }

        /// <summary>
        /// Creates an instance of SqlScript for <paramref name="binaryKey"/> to be rolled-back according to <paramref name="rollbackOptions"/>
        /// </summary>
        /// <param name="id"></param>
        /// <param name="feature"></param>
        /// <param name="binaryKey"></param>
        /// <param name="rollbackOptions"></param>
        public SqlScript(Id id, Feature feature, string binaryKey, RollbackSql rollbackOptions)
            : this(binaryKey, rollbackOptions)
        {
            Id = id;
            Feature = feature;
        }

        #endregion Constructors

        /// <summary>
        /// Gets or sets the feature the SqlScript belongs to.
        /// </summary>
        public Feature Feature { get; set; }

        #region Wix SqlScript attributes

        /// <summary>
        /// Maps to the BinaryKey property of SqlScript
        /// </summary>
        public string BinaryKey { get; set; } //required

        /// <summary>
        /// Maps to the ContinueOnError property of SqlScript
        /// </summary>
        public bool? ContinueOnError { get; set; }

        /// <summary>
        /// Maps to the ExecuteOnInstall property of SqlScript
        /// </summary>
        public bool? ExecuteOnInstall { get; set; } //partially required

        /// <summary>
        /// Maps to the ExecuteOnReinstall property of SqlScript
        /// </summary>
        public bool? ExecuteOnReinstall { get; set; } //partially required

        /// <summary>
        /// Maps to the ExecuteOnUninstall property of SqlScript
        /// </summary>
        public bool? ExecuteOnUninstall { get; set; } //partially required

        /// <summary>
        /// Maps to the RollbackOnInstall property of SqlScript
        /// </summary>
        public bool? RollbackOnInstall { get; set; } //partially required

        /// <summary>
        /// Maps to the RollbackOnReinstall property of SqlScript
        /// </summary>
        public bool? RollbackOnReinstall { get; set; } //partially required

        /// <summary>
        /// Maps to the RollbackOnUninstall property of SqlScript
        /// </summary>
        public bool? RollbackOnUninstall { get; set; } //partially required

        /// <summary>
        /// Maps to the Sequence property of SqlScript
        /// </summary>
        public int? Sequence { get; set; }

        /// <summary>
        /// Maps to the SqlDb property of SqlScript. This property is to be inferred from the containing SqlDatabase element.
        /// </summary>
        internal string SqlDb { get; set; } //required if and only if not under a SqlDatabase.

        /// <summary>
        /// Maps to the User property of SqlScript
        /// </summary>
        public string User { get; set; }

        #endregion Wix SqlScript attributes

        private void SetExecutionOptions(ExecuteSql executeOptions)
        {
            if (executeOptions == ExecuteSql.None) throw new ArgumentException("None is invalid. It has no legal representation in Wix", "executeOptions");

            if ((executeOptions & ExecuteSql.OnInstall) == ExecuteSql.OnInstall) ExecuteOnInstall = true;
            if ((executeOptions & ExecuteSql.OnReinstall) == ExecuteSql.OnReinstall) ExecuteOnReinstall = true;
            if ((executeOptions & ExecuteSql.OnUninstall) == ExecuteSql.OnUninstall) ExecuteOnUninstall = true;
        }

        private void SetRollbackOptions(RollbackSql rollbackOption)
        {
            if (rollbackOption == RollbackSql.None) throw new ArgumentException("None is invalid. It has no legal representation in Wix", "rollbackOption");

            if ((rollbackOption & RollbackSql.OnInstall) == RollbackSql.OnInstall) RollbackOnInstall = true;
            if ((rollbackOption & RollbackSql.OnReinstall) == RollbackSql.OnReinstall) RollbackOnReinstall = true;
            if ((rollbackOption & RollbackSql.OnUninstall) == RollbackSql.OnUninstall) RollbackOnUninstall = true;
        }
    }

    internal static class SqlEx
    {
        static void Do<T>(this T? nullable, Action<T> action) where T : struct
        {
            if (!nullable.HasValue) return;
            action(nullable.Value);
        }

        public static void EmitAttributes(this SqlDatabase sqlDb, XElement sqlDbElement)
        {
            sqlDbElement.SetAttributeValue("Id", sqlDb.Id);
            sqlDbElement.SetAttributeValue("Database", sqlDb.Database);
            sqlDbElement.SetAttributeValue("Server", sqlDb.Server);

            sqlDb.ConfirmOverwrite.Do(b => sqlDbElement.SetAttributeValue("ConfirmOverwrite", b.ToYesNo()));
            sqlDb.ContinueOnError.Do(b => sqlDbElement.SetAttributeValue("ContinueOnError", b.ToYesNo()));
            sqlDb.CreateOnInstall.Do(b => sqlDbElement.SetAttributeValue("CreateOnInstall", b.ToYesNo()));
            sqlDb.CreateOnReinstall.Do(b => sqlDbElement.SetAttributeValue("CreateOnReinstall", b.ToYesNo()));
            sqlDb.CreateOnUninstall.Do(b => sqlDbElement.SetAttributeValue("CreateOnUninstall", b.ToYesNo()));
            sqlDb.DropOnInstall.Do(b => sqlDbElement.SetAttributeValue("DropOnInstall", b.ToYesNo()));
            sqlDb.DropOnReinstall.Do(b => sqlDbElement.SetAttributeValue("DropOnReinstall", b.ToYesNo()));
            sqlDb.DropOnUninstall.Do(b => sqlDbElement.SetAttributeValue("DropOnUninstall", b.ToYesNo()));
            if (!string.IsNullOrEmpty(sqlDb.Instance)) sqlDbElement.SetAttributeValue("Instance", sqlDb.Instance);
            if (!string.IsNullOrEmpty(sqlDb.User)) sqlDbElement.SetAttributeValue("User", sqlDb.User);
        }

        public static void EmitAttributes(this SqlString sqlString, XElement sqlStringElement)
        {
            sqlStringElement.SetAttributeValue("Id", sqlString.Id);
            sqlStringElement.SetAttributeValue("SQL", sqlString.Sql);

            sqlString.ContinueOnError.Do(b => sqlStringElement.SetAttributeValue("ContinueOnError", b.ToYesNo()));
            sqlString.ExecuteOnInstall.Do(b => sqlStringElement.SetAttributeValue("ExecuteOnInstall", b.ToYesNo()));
            sqlString.ExecuteOnReinstall.Do(b => sqlStringElement.SetAttributeValue("ExecuteOnReinstall", b.ToYesNo()));
            sqlString.ExecuteOnUninstall.Do(b => sqlStringElement.SetAttributeValue("ExecuteOnUninstall", b.ToYesNo()));
            sqlString.RollbackOnInstall.Do(b => sqlStringElement.SetAttributeValue("RollbackOnInstall", b.ToYesNo()));
            sqlString.RollbackOnReinstall.Do(b => sqlStringElement.SetAttributeValue("RollbackOnReinstall", b.ToYesNo()));
            sqlString.RollbackOnUninstall.Do(b => sqlStringElement.SetAttributeValue("RollbackOnUninstall", b.ToYesNo()));
            sqlString.Sequence.Do(i => sqlStringElement.SetAttributeValue("Sequence", i));
            if (!string.IsNullOrEmpty(sqlString.SqlDb)) sqlStringElement.SetAttributeValue("SqlDb", sqlString.SqlDb);
            if (!string.IsNullOrEmpty(sqlString.User)) sqlStringElement.SetAttributeValue("User", sqlString.User);
        }

        public static void EmitAttributes(this SqlScript sqlScript, XElement sqlScriptElement)
        {
            sqlScriptElement.SetAttributeValue("Id", sqlScript.Id);
            sqlScriptElement.SetAttributeValue("BinaryKey", sqlScript.BinaryKey);

            sqlScript.ContinueOnError.Do(b => sqlScriptElement.SetAttributeValue("ContinueOnError", b.ToYesNo()));
            sqlScript.ExecuteOnInstall.Do(b => sqlScriptElement.SetAttributeValue("ExecuteOnInstall", b.ToYesNo()));
            sqlScript.ExecuteOnReinstall.Do(b => sqlScriptElement.SetAttributeValue("ExecuteOnReinstall", b.ToYesNo()));
            sqlScript.ExecuteOnUninstall.Do(b => sqlScriptElement.SetAttributeValue("ExecuteOnUninstall", b.ToYesNo()));
            sqlScript.RollbackOnInstall.Do(b => sqlScriptElement.SetAttributeValue("RollbackOnInstall", b.ToYesNo()));
            sqlScript.RollbackOnReinstall.Do(b => sqlScriptElement.SetAttributeValue("RollbackOnReinstall", b.ToYesNo()));
            sqlScript.RollbackOnUninstall.Do(b => sqlScriptElement.SetAttributeValue("RollbackOnUninstall", b.ToYesNo()));
            sqlScript.Sequence.Do(i => sqlScriptElement.SetAttributeValue("Sequence", i));
            if (!string.IsNullOrEmpty(sqlScript.SqlDb)) sqlScriptElement.SetAttributeValue("SqlDb", sqlScript.SqlDb);
            if (!string.IsNullOrEmpty(sqlScript.User)) sqlScriptElement.SetAttributeValue("User", sqlScript.User);
        }
    }
}
