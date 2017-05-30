namespace WixSharp
{
    /// <summary>
    /// Container of the parameters of the Digitaly Signing of the Bootstrapper
    /// </summary>
    public class DigitalySignBootstrapper : DigitalySign
    {
        public override int Apply(string fileToSign)
        {
            return CommonTasks.Tasks.DigitalySignBootstrapper(fileToSign, PfxFilePath, TimeUrl.AbsoluteUri, Password,
                PrepareOptionalArguments(), WellKnownLocations);
        }
    }
}