Run create-temp-certificates.ps1 to create a temp certificate.

Now you can sign any file (e.g. a bundle) using the following command:

signtool sign /fd SHA256 /a /f ".\TempTestCert.pfx" /p password123 ".\MyProductBundleSetup.exe"