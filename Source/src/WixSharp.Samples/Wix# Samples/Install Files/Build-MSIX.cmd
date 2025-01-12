rem $cert = New-SelfSignedCertificate -Type CodeSigningCert -Subject "CN=MyApp" -KeyExportPolicy Exportable -CertStoreLocation Cert:\CurrentUser\My -NotAfter (Get-Date).AddYears(5)
rem Export-PfxCertificate -Cert $cert -FilePath ".\MyApp.pfx" -Password (ConvertTo-SecureString -String "MyPassword" -Force -AsPlainText)
MsixPackagingTool.exe create-package --template .\MyProduct_1.0.0.0_x64__yw5rxyn2chjm8_template.xml -v
pause
