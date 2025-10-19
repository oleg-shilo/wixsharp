New-SelfSignedCertificate -Type CodeSigningCert -Subject "CN=TempTestCert" -CertStoreLocation "Cert:\CurrentUser\My"
$pwd = ConvertTo-SecureString -String "password123" -Force -AsPlainText
# replace the <THUMBPRINT>
# Export-PfxCertificate -Cert "Cert:\CurrentUser\My\1077877339E145962379519332433F1E346A2824" -FilePath "C:\temp\TempTestCert.pfx" -Password $pwd
# create "C:\temp\""
Export-PfxCertificate -Cert "Cert:\CurrentUser\My\<THUMBPRINT>" -FilePath "C:\temp\TempTestCert.pfx" -Password $pwd

signtool sign /fd SHA256 /a /f "C:\temp\TempTestCert.pfx" /p password123 "D:\dev\wixsharp4\Source\src\WixSharp.Samples\Wix# Samples\SigningBundle\MyProductBundleSetup.exe"