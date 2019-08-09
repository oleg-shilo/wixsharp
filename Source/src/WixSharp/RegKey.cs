namespace WixSharp
{
    public class RegKey
    {
        /// <summary>
        /// <see cref="Feature"></see> the registry value should be included in.
        /// </summary>
        public Feature Feature;

        /// <summary>
        /// The registry hive name.
        /// </summary>
        public RegistryHive Root;

        /// <summary>
        /// The registry key name.
        /// </summary>
        public string Key;

        /// <summary>
        /// Facilitates the installation of packages that include both 32-bit and 64-bit components.
        /// Set this attribute to 'yes' to mark the corresponding RegValue as a 64-bit component.
        /// </summary>
        public bool Win64;

        internal RegValue[] GetValues()
        {
            foreach (var value in _values)
            {
                value.Feature = Feature;
                value.Root = Root;
                value.Key = Key;
                value.Win64 = Win64;
            }
            return _values;
        }

        private readonly RegValue[] _values;

        /// <summary>
        /// Initializes a new instance of the <see cref="RegKey"/> class with properties initialized with specified parameters.
        /// </summary>
        /// <param name="feature"><see cref="Feature"></see> the registry value should be included in.</param>
        /// <param name="root">The registry hive name.</param>
        /// <param name="key">The registry key name.</param>
        /// <param name="values">The registry entry values.</param>
        public RegKey(Feature feature, RegistryHive root, string key, params RegValue[] values)
        {
            Feature = feature;
            Root = root;
            Key = key;
            _values = values;
        }
    }
}
