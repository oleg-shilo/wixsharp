$cert = New-SelfSignedCertificate `
    -Type CodeSigningCert `
    -Subject "CN=TempTestCert" `
    -CertStoreLocation "Cert:\CurrentUser\My" `
    -NotAfter (Get-Date).AddYears(3)

$pwd = ConvertTo-SecureString "password123" -AsPlainText -Force

Export-PfxCertificate -Cert $cert -FilePath ".\TempTestCert.pfx" -Password $pwd   