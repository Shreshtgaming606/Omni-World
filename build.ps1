$ErrorActionPreference = "Stop"

$root = Split-Path -Parent $MyInvocation.MyCommand.Path
$src = Join-Path $root "src"
$bin = Join-Path $root "bin"

$compiler = Join-Path $env:WINDIR "Microsoft.NET\Framework64\v4.0.30319\csc.exe"
if (-not (Test-Path $compiler)) {
    $compiler = Join-Path $env:WINDIR "Microsoft.NET\Framework\v4.0.30319\csc.exe"
}

if (-not (Test-Path $compiler)) {
    throw "Could not find the .NET Framework C# compiler. Install Visual Studio Build Tools or the .NET SDK, then rerun build.ps1."
}

New-Item -ItemType Directory -Force $bin | Out-Null

$outFile = Join-Path $bin "OmniWorld.exe"
$sources = Get-ChildItem $src -Filter *.cs | Sort-Object Name | ForEach-Object { $_.FullName }

& $compiler `
    /nologo `
    /target:winexe `
    /optimize+ `
    /warn:4 `
    "/out:$outFile" `
    /reference:System.dll `
    /reference:System.Core.dll `
    /reference:System.Drawing.dll `
    /reference:System.Windows.Forms.dll `
    $sources

if ($LASTEXITCODE -ne 0) {
    throw "Build failed with exit code $LASTEXITCODE."
}

Write-Host "Built bin\OmniWorld.exe"
