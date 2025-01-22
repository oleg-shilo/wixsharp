$cert = New-SelfSignedCertificate -Type CodeSigningCert -Subject "CN=MyApp" -KeyExportPolicy Exportable -CertStoreLocation Cert:\CurrentUser\My -NotAfter (Get-Date).AddYears(5)
Export-PfxCertificate -Cert $cert -FilePath ".\MyApp.pfx" -Password (ConvertTo-SecureString -String "MyPassword" -Force -AsPlainText)
pause