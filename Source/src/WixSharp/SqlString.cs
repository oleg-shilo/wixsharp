using System;
using System.Xml.Linq;
using WixSharp.CommonTasks;

namespace WixSharp
{
    /// <summary>
    /// Represents WixSqlExtension element SqlString
    /// </summary>
    public class SqlString : GenericNestedEntity, IGenericEntity
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
            SQL = sql;
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
        /// <param name="sqlDb"></param>
        /// <param name="sql"></param>
        /// <param name="executeOptions"></param>
        public SqlString(string sqlDb, string sql, ExecuteSql executeOptions)
            : this(sql, executeOptions)
        {
            SqlDb = sqlDb;
        }

        /// <summary>
        /// Creates an instance of SqlString from <paramref name="sql"/> to be execute according to <paramref name="executeOptions"/>
        /// </summary>
        /// <param name="sqlDb"></param>
        /// <param name="sql"></param>
        /// <param name="executeOptions"></param>
        public SqlString(SqlDatabase sqlDb, string sql, ExecuteSql executeOptions)
            : this(sql, executeOptions)
        {
            SqlDb = sqlDb.Id;
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
        /// <param name="id"></param>
        /// <param name="sqlDb"></param>
        /// <param name="sql"></param>
        /// <param name="executeOptions"></param>
        public SqlString(Id id, string sqlDb, string sql, ExecuteSql executeOptions)
            : this(sql, executeOptions)
        {
            Id = id;
            SqlDb = sqlDb;
        }

        /// <summary>
        /// Creates an instance of SqlString from <paramref name="sql"/> to be execute according to <paramref name="executeOptions"/>
        /// </summary>
        /// <param name="id"></param>
        /// <param name="sqlDb"></param>
        /// <param name="sql"></param>
        /// <param name="executeOptions"></param>
        public SqlString(Id id, SqlDatabase sqlDb, string sql, ExecuteSql executeOptions)
            : this(sql, executeOptions)
        {
            Id = id;
            SqlDb = sqlDb.Id;
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
        /// <param name="sqlDb"></param>
        /// <param name="feature"></param>
        /// <param name="sql"></param>
        /// <param name="executeOptions"></param>
        public SqlString(string sqlDb, Feature feature, string sql, ExecuteSql executeOptions)
            : this(sql, executeOptions)
        {
            SqlDb = sqlDb;
            Feature = feature;
        }

        /// <summary>
        /// Creates an instance of SqlString from <paramref name="sql"/> to be execute according to <paramref name="executeOptions"/>
        /// </summary>
        /// <param name="sqlDb"></param>
        /// <param name="feature"></param>
        /// <param name="sql"></param>
        /// <param name="executeOptions"></param>
        public SqlString(SqlDatabase sqlDb, Feature feature, string sql, ExecuteSql executeOptions)
            : this(sql, executeOptions)
        {
            SqlDb = sqlDb.Id;
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
        /// Creates an instance of SqlString from <paramref name="sql"/> to be execute according to <paramref name="executeOptions"/>
        /// </summary>
        /// <param name="id"></param>
        /// <param name="sqlDb"></param>
        /// <param name="feature"></param>
        /// <param name="sql"></param>
        /// <param name="executeOptions"></param>
        public SqlString(Id id, string sqlDb, Feature feature, string sql, ExecuteSql executeOptions)
            : this(sql, executeOptions)
        {
            Id = id;
            SqlDb = sqlDb;
            Feature = feature;
        }

        /// <summary>
        /// Creates an instance of SqlString from <paramref name="sql"/> to be execute according to <paramref name="executeOptions"/>
        /// </summary>
        /// <param name="id"></param>
        /// <param name="sqlDb"></param>
        /// <param name="feature"></param>
        /// <param name="sql"></param>
        /// <param name="executeOptions"></param>
        public SqlString(Id id, SqlDatabase sqlDb, Feature feature, string sql, ExecuteSql executeOptions)
            : this(sql, executeOptions)
        {
            Id = id;
            SqlDb = sqlDb.Id;
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
        /// <param name="sqlDb"></param>
        /// <param name="sql"></param>
        /// <param name="rollbackOptions"></param>
        public SqlString(string sqlDb, string sql, RollbackSql rollbackOptions)
            : this(sql, rollbackOptions)
        {
            SqlDb = sqlDb;
        }

        /// <summary>
        /// Creates an instance of SqlString from <paramref name="sql"/> to be rolled-back according to <paramref name="rollbackOptions"/>
        /// </summary>
        /// <param name="sqlDb"></param>
        /// <param name="sql"></param>
        /// <param name="rollbackOptions"></param>
        public SqlString(SqlDatabase sqlDb, string sql, RollbackSql rollbackOptions)
            : this(sql, rollbackOptions)
        {
            SqlDb = sqlDb.Id;
        }

        /// <summary>
        /// Creates an instance of SqlString from <paramref name="sql"/> to be rolled-back according to <paramref name="rollbackOptions"/>
        /// </summary>
        /// <param name="id"></param>
        /// <param name="sql"></param>
        /// <param name="rollbackOptions"></param>
        public SqlString(Id id, string sql, RollbackSql rollbackOptions)
            : this(sql, rollbackOptions)
        {
            Id = id;
        }

        /// <summary>
        /// Creates an instance of SqlString from <paramref name="sql"/> to be rolled-back according to <paramref name="rollbackOptions"/>
        /// </summary>
        /// <param name="id"></param>
        /// <param name="sqlDb"></param>
        /// <param name="sql"></param>
        /// <param name="rollbackOptions"></param>
        public SqlString(Id id, string sqlDb, string sql, RollbackSql rollbackOptions)
            : this(sql, rollbackOptions)
        {
            Id = id;
            SqlDb = sqlDb;
        }

        /// <summary>
        /// Creates an instance of SqlString from <paramref name="sql"/> to be rolled-back according to <paramref name="rollbackOptions"/>
        /// </summary>
        /// <param name="id"></param>
        /// <param name="sqlDb"></param>
        /// <param name="sql"></param>
        /// <param name="rollbackOptions"></param>
        public SqlString(Id id, SqlDatabase sqlDb, string sql, RollbackSql rollbackOptions)
            : this(sql, rollbackOptions)
        {
            Id = id;
            SqlDb = sqlDb.Id;
        }

        /// <summary>
        /// Creates an instance of SqlString from <paramref name="sql"/> to be rolled-back according to <paramref name="rollbackOptions"/>
        /// </summary>
        /// <param name="feature"></param>
        /// <param name="sql"></param>
        /// <param name="rollbackOptions"></param>
        public SqlString(Feature feature, string sql, RollbackSql rollbackOptions)
            : this(sql, rollbackOptions)
        {
            Feature = feature;
        }

        /// <summary>
        /// Creates an instance of SqlString from <paramref name="sql"/> to be rolled-back according to <paramref name="rollbackOptions"/>
        /// </summary>
        /// <param name="sqlDb"></param>
        /// <param name="feature"></param>
        /// <param name="sql"></param>
        /// <param name="rollbackOptions"></param>
        public SqlString(string sqlDb, Feature feature, string sql, RollbackSql rollbackOptions)
            : this(sql, rollbackOptions)
        {
            SqlDb = sqlDb;
            Feature = feature;
        }

        /// <summary>
        /// Creates an instance of SqlString from <paramref name="sql"/> to be rolled-back according to <paramref name="rollbackOptions"/>
        /// </summary>
        /// <param name="sqlDb"></param>
        /// <param name="feature"></param>
        /// <param name="sql"></param>
        /// <param name="rollbackOptions"></param>
        public SqlString(SqlDatabase sqlDb, Feature feature, string sql, RollbackSql rollbackOptions)
            : this(sql, rollbackOptions)
        {
            SqlDb = sqlDb.Id;
            Feature = feature;
        }

        /// <summary>
        /// Creates an instance of SqlString from <paramref name="sql"/> to be rolled-back according to <paramref name="rollbackOptions"/>
        /// </summary>
        /// <param name="id"></param>
        /// <param name="feature"></param>
        /// <param name="sql"></param>
        /// <param name="rollbackOptions"></param>
        public SqlString(Id id, Feature feature, string sql, RollbackSql rollbackOptions)
            : this(sql, rollbackOptions)
        {
            Id = id;
            Feature = feature;
        }

        /// <summary>
        /// Creates an instance of SqlString from <paramref name="sql"/> to be rolled-back according to <paramref name="rollbackOptions"/>
        /// </summary>
        /// <param name="id"></param>
        /// <param name="sqlDb"></param>
        /// <param name="feature"></param>
        /// <param name="sql"></param>
        /// <param name="rollbackOptions"></param>
        public SqlString(Id id, string sqlDb, Feature feature, string sql, RollbackSql rollbackOptions)
            : this(sql, rollbackOptions)
        {
            Id = id;
            SqlDb = sqlDb;
            Feature = feature;
        }

        /// <summary>
        /// Creates an instance of SqlString from <paramref name="sql"/> to be rolled-back according to <paramref name="rollbackOptions"/>
        /// </summary>
        /// <param name="id"></param>
        /// <param name="sqlDb"></param>
        /// <param name="feature"></param>
        /// <param name="sql"></param>
        /// <param name="rollbackOptions"></param>
        public SqlString(Id id, SqlDatabase sqlDb, Feature feature, string sql, RollbackSql rollbackOptions)
            : this(sql, rollbackOptions)
        {
            Id = id;
            SqlDb = sqlDb.Id;
            Feature = feature;
        }

        #endregion Constructors

        #region Wix SqlString attributes

        /// <summary>
        /// Maps to the ContinueOnError property of SqlString
        /// </summary>
        [Xml]
        public bool? ContinueOnError;

        /// <summary>
        /// Maps to the ExecuteOnInstall property of SqlString
        /// </summary>
        [Xml]
        public bool? ExecuteOnInstall; //partially required

        /// <summary>
        /// Maps to the ExecuteOnReinstall property of SqlString
        /// </summary>
        [Xml]
        public bool? ExecuteOnReinstall; //partially required

        /// <summary>
        /// Maps to the ExecuteOnUninstall property of SqlString
        /// </summary>
        [Xml]
        public bool? ExecuteOnUninstall; //partially required

        /// <summary>
        /// Maps to the RollbackOnInstall property of SqlString
        /// </summary>
        [Xml]
        public bool? RollbackOnInstall; //partially required

        /// <summary>
        /// Maps to the RollbackOnReinstall property of SqlString
        /// </summary>
        [Xml]
        public bool? RollbackOnReinstall; //partially required

        /// <summary>
        /// Maps to the RollbackOnUninstall property of SqlString
        /// </summary>
        [Xml]
        public bool? RollbackOnUninstall; //partially required

        /// <summary>
        /// Maps to the Sequence property of SqlString
        /// </summary>
        [Xml]
        public int? Sequence;

        /// <summary>
        /// Maps to the Sql property of SqlString
        /// </summary>
        [Xml]
        public string SQL; //required

        /// <summary>
        /// Maps to the SqlDb property of SqlString. This property is to be inferred from the containing SqlDatabase element.
        /// </summary>
        [Xml]
        internal string SqlDb; //required when not under a SqlDatabase element

        /// <summary>
        /// Maps to the User property of SqlString
        /// </summary>
        [Xml]
        public string User;

        #endregion Wix SqlString attributes

        /// <summary>
        /// Adds itself as an XML content into the WiX source being generated from the <see cref="T:WixSharp.Project" />.
        /// See 'Wix#/samples/Extensions' sample for the details on how to implement this interface correctly.
        /// </summary>
        /// <param name="context">The context.</param>
        public void Process(ProcessingContext context)
        {
            if (SqlDb != null)
            {
                context.Project.IncludeWixExtension(WixExtension.Sql);

                XElement component = this.CreateParentComponent();
                XElement sqlString = this.ToXElement(WixExtension.Sql, "SqlString");
                component.Add(sqlString);

                context.XParent.FindFirst("Component").Parent?.Add(component);
                MapComponentToFeatures(component.Attr("Id"), ActualFeatures, context);
            }
            else
            {
                base.Process(context, "SqlString");
            }
        }

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
}