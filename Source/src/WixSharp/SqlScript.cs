using System;

namespace WixSharp
{
    /// <summary>
    /// Represents WixSqlExtension element SqlScript
    /// </summary>
    public class SqlScript : GenericNestedEntity, IGenericEntity
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

        #region Wix SqlScript attributes

        /// <summary>
        /// Maps to the BinaryKey property of SqlScript
        /// </summary>
        [Xml]
        public string BinaryKey; //required

        /// <summary>
        /// Maps to the ContinueOnError property of SqlScript
        /// </summary>
        [Xml]
        public bool? ContinueOnError;

        /// <summary>
        /// Maps to the ExecuteOnInstall property of SqlScript
        /// </summary>
        [Xml]
        public bool? ExecuteOnInstall; //partially required

        /// <summary>
        /// Maps to the ExecuteOnReinstall property of SqlScript
        /// </summary>
        [Xml]
        public bool? ExecuteOnReinstall; //partially required

        /// <summary>
        /// Maps to the ExecuteOnUninstall property of SqlScript
        /// </summary>
        [Xml]
        public bool? ExecuteOnUninstall; //partially required

        /// <summary>
        /// Maps to the RollbackOnInstall property of SqlScript
        /// </summary>
        [Xml]
        public bool? RollbackOnInstall; //partially required

        /// <summary>
        /// Maps to the RollbackOnReinstall property of SqlScript
        /// </summary>
        [Xml]
        public bool? RollbackOnReinstall; //partially required

        /// <summary>
        /// Maps to the RollbackOnUninstall property of SqlScript
        /// </summary>
        [Xml]
        public bool? RollbackOnUninstall; //partially required

        /// <summary>
        /// Maps to the Sequence property of SqlScript
        /// </summary>
        [Xml]
        public int? Sequence;

        /// <summary>
        /// Maps to the SqlDb property of SqlScript. This property is to be inferred from the containing SqlDatabase element.
        /// </summary>
        [Xml]
        internal string SqlDb; //required if and only if not under a SqlDatabase.

        /// <summary>
        /// Maps to the User property of SqlScript
        /// </summary>
        [Xml]
        public string User;

        #endregion Wix SqlScript attributes

        /// <summary>
        /// Adds itself as an XML content into the WiX source being generated from the <see cref="T:WixSharp.Project" />.
        /// See 'Wix#/samples/Extensions' sample for the details on how to implement this interface correctly.
        /// </summary>
        /// <param name="context">The context.</param>
        public void Process(ProcessingContext context)
        {
            base.Process(context, "SqlScript");
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