## .NET Core support

### Current state

This is how external functionality can be embedded in MSI/WiX currently (WiX4):

... | Functionality           |Host     | Runtime         | Comment
--- | ---                     |---      | ---             | ---
1   | CA                      |MSI      | Native          | native dll
2   | Managed CA (DTF)        |WiX      | .NET Framework  | assembly wrapped into native dll proxy
3   | Managed Project Events  |WixSharp | .NET Framework  | Managed CA defined as CLR event
4   | Custom BA (Burn)        |WIX      | .NET Framework  |
5   | Embedded UI             |WiX      | .NET Framework  | Assembly wrapped into native dll interop
6   | MSI builder             |WixSharp | .NET Framework  | Assembly built by  WixSharp VS project

In MS/WiX vision all 1-6 cases are implemented as individual modules/workflows.
This leads to a very fragmented deployment definition implemented as multiple projects defining the functionality in different programming languages.

WixSharp takes a different and rather elegant approach. Everything can be defined as a single assembly with a very minimalistic user code.

This approach is possible because all the assemblies of MSI/WiX workflows are of the same type. Thus WixSharp can combine them in a single assembly. But if we try to convert it in .NET Core then it becomes tricky.

None of the scenarios from the table above support .NET Core compiled assemblies except 6 `MSI builder` (currently deliberately disabled).

AOT compilation offers an interesting opportunity as the compiled assembly effectively becomes a native dll. Thus some scenarios can be scenario implemented providing the assemblies are AOT-ed by WixSharp before the msi compilation.

... | Functionality           |Host     | Runtime         | Comment
--- | ---                     |---      | ---             | ---
1   | CA                      |MSI      | Native, AOT-asm | supported
2   | ~~Managed CA (DTF)~~    |~~WiX~~  |  ~~.NET Framework~~ | not needed any more
3   | Managed Project Events  |WixSharp | AOT-asm         | Possible if not too much dependencies
4   | Custom BA (Burn)        |WIX      | .NET Framework  | has to stay .NET Framework
5   | Embedded UI             |WiX      | .NET Framework  | has to stay .NET Framework
6   | MSI builder             |WixSharp | AOT-asm         | Needs WixSharp recompilation and that's it.

I have already got scenarios 1 and 6 working.

Remaining scenarios:
3. It is likely possible unless it is too difficult to keep it together.
4. Scenario 4 are completely out as there is no chance to influence WiX team. They are most likely going (or not) to support it eventually. But there is no way to impact the schedule of the delivery of this functionality. I feel like the WiX team is underresourced to do it quickly.
5. The same as above.
   _Though there is a small chance that I will be able to by pass WiX runtime and embed AOT-asm directly in msi. Some heavy investigation is required._

I am currently trying to POC scenario `#3` if it works then I will release a NET8 port of WixSharp. I really dislike the idea of maintaining 3 streams of product WiX3, WiX4 and WiX4-NETCore.
But I feel otherwise we will have to wait for NETCore integration next 5-7 years.