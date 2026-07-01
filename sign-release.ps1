$ErrorActionPreference = "Stop"

param(
    [Parameter(Mandatory=$true)]
    [string]$CertificateThumbprint,

    [string]$TimestampServer = "http://timestamp.digicert.com"
)

$root = Split-Path -Parent $MyInvocation.MyCommand.Path
$exe = Join-Path $root "release\OmniWorld.exe"

if (-not (Test-Path $exe)) {
    throw "release\OmniWorld.exe was not found. Run build.ps1 and copy the EXE to release first."
}

$cert = Get-ChildItem Cert:\CurrentUser\My | Where-Object {
    $_.Thumbprint -replace '\s', '' -ieq ($CertificateThumbprint -replace '\s', '')
} | Select-Object -First 1

if (-not $cert) {
    throw "Could not find a code-signing certificate with that thumbprint in Cert:\CurrentUser\My."
}

$signature = Set-AuthenticodeSignature -FilePath $exe -Certificate $cert -TimestampServer $TimestampServer

if ($signature.Status -ne "Valid") {
    throw "Signing did not produce a valid signature. Status: $($signature.Status) $($signature.StatusMessage)"
}

Write-Host "Signed release\OmniWorld.exe as $($cert.Subject)"
