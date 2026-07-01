$ErrorActionPreference = "Stop"

$root = Split-Path -Parent $MyInvocation.MyCommand.Path
& (Join-Path $root "build.ps1")
Start-Process -FilePath (Join-Path $root "bin\OmniWorld.exe") -WorkingDirectory $root
