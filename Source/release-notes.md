## Release v2.15

### Changes:

- Fixed AppSearch.IniFileValue ignoring the encoding parameter
- Document headless WiX source generation on Linux
- Demonstrate bundle signing in sample
- Implemented support for using generic signer for signing the bundles.
- Improveed Burn bootstrapper signing and inspection
- Enabled headless WiX source generation on Linux
- Fixed problem with unnessesary waits between file signing steps (e.g. retries).
- Signing failures are now hangeled by WixSharp as errors - abort the build process. Previously it was the top level builder (e.g. msbuild, IDE) who was handling teh build errors.
- Added signing bundle Application assembly and its payloads: 
   ```C#
   bundle.SignApplicationFiles = true;
   ``` 
- Now, signing tool output is hidden by default. Otherwise it's too crowded in the build output. WixSharp still reportss what is being signed and the signing result (success/error). 
  If prefered seeing the signtool.exe output it can be enabled with:
   ```c#
   Compiler.VerboseSigningOutput = true;
   ```
 - Added a binary analyzer utility that allows quick and reliable detection of the signed state of the bundle engine in a given bundle/bootstrapper file. It can be used for quick verification of bundle signing.
   ```c#
   var bundleFile = bundle.Build("MyProductBundleSetup.exe");
   var info = BurnEngineInspector.InspectBundle(bundleFile);
   
   Console.WriteLine($"BundleSigned: {info.BundleSigned}");
   Console.WriteLine($"EngineSigned: {info.EngineSigned}"); 
   ```
- Signing algorithm for bundles is reworked to allow using generic signers. This makes `DigitalSignatureBootstrapper` prac tically obsolete even though still working:
   ```c#
   bundle.DigitalSignature = new DigitalSignature 
   {
       PfxFilePath = certificateFile,
       Password = "password123",
       Description = "MyProductBundle"
    };
  ``` 
- `GenericSigner` interface is reworked to allow setting teh delegates for:
  - `Apply` - older interface for signing individual files
  - `Sign` - newer interface for signing multiple files in a single step
  ```c#
  project.DigitalSignature = new GenericSigner
  (
      apply: fileToSign =>
      {
          // digitally sign your file (a string) here
          return 0;
      }
  );
  // or 
  project.DigitalSignature = new GenericSigner
  (
      sign: filesToSign =>
      {
          // digitally sign your files (a string[]) here
          return 0;
      }
  );
  ```
  You need to define only one of the delegates. If another delgate is preferred but not available WixSharp will tunnel the call to the other available delegate. 


