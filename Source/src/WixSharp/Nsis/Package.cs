namespace WixSharp.Nsis
{
    public abstract class Package
    {
        public string FileName { get; set; }

        public string OptionName { get; set; }

        public string Arguments { get; set; }

        // Gets or sets a value indicating whether to start the process in a new window.
        public bool CreateNoWindow { get; set; }

        // Gets or sets a value indicating whether to use the operating system shell to start the process.
        public bool UseShellExecute { get; set; }
    }

    public class PrimaryPackage : Package
    {
    }

    public class PrerequisitePackage : Package
    {
        public string RegKeyValue { get; set; }

        public bool PostVerify { get; set; } = true;
    }
}
