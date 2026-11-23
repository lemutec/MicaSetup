#Requires -Version 5.1
<#
.SYNOPSIS
  Publish the MicaSetup tools, stage .\Build, pack the default template, create the NuGet package,
  and rename micasetup.7z to MicaSetup_v*.7z so it matches the installer.

.DESCRIPTION
  Run from any directory; the script switches to its own folder first. Native tools
  (dotnet, 7z, makemica) stop the script on a non-zero exit code. A missing .snupkg
  is ignored, matching the old copy step when symbols were not produced.

.PARAMETER NoPause
  Skip the trailing key prompt. The prompt is also skipped when CI or GITHUB_ACTIONS is set.
#>
param(
    [switch]$NoPause
)

$ErrorActionPreference = 'Stop'

Set-Location $PSScriptRoot

# Avoid mojibake: Windows PowerShell defaults to the system ANSI/OEM code page (e.g. GBK),
# while `dotnet` CLI localized output is UTF-8.
$script:Utf8NoBom = [System.Text.UTF8Encoding]::new($false)
try {
    [Console]::InputEncoding = $script:Utf8NoBom
    [Console]::OutputEncoding = $script:Utf8NoBom
}
catch {
    # Host may not expose a real console (redirected I/O).
}
$OutputEncoding = $script:Utf8NoBom
& chcp.com 65001 > $null
$env:DOTNET_CLI_UI_LANGUAGE = 'en'

function Invoke-Native {
    param(
        [Parameter(Mandatory = $true)]
        [string]$FilePath,

        [Parameter(Mandatory = $true)]
        [string[]]$ArgumentList
    )

    & $FilePath @ArgumentList
    if ($LASTEXITCODE -ne 0) {
        $rendered = $ArgumentList -join ' '
        throw "Command failed (exit $LASTEXITCODE): $FilePath $rendered"
    }
}

function Remove-PathIfExists {
    param([Parameter(Mandatory = $true)][string]$Path)

    if (-not (Test-Path -LiteralPath $Path)) {
        return
    }

    $item = Get-Item -LiteralPath $Path -Force
    if ($item.PSIsContainer) {
        Remove-Item -LiteralPath $Path -Recurse -Force
    }
    else {
        Remove-Item -LiteralPath $Path -Force
    }
}

$exitCode = 0
try {
    Write-Host '[build]'

    $publishProjects = @(
        'MicaSetup.Tools\CheckVer\CheckVer.csproj'
        'MicaSetup.Tools\FetchVer\FetchVer.csproj'
        'MicaSetup.Tools\MakeIcon\MakeIcon.csproj'
        'MicaSetup.Tools\MakeIcon.Cli\MakeIcon.Cli.csproj'
        'MicaSetup.Tools\MakeMica\MakeMica.csproj'
        'MicaSetup.Tools\MakeMica.Cli\MakeMica.Cli.csproj'
        # MakeMui is under development and not in the tree yet
        # 'MicaSetup.Tools\MakeMui\MakeMui.csproj'
        'MicaSetup.Tools\MICA\MICA.csproj'
    )

    foreach ($project in $publishProjects) {
        Invoke-Native -FilePath 'dotnet' -ArgumentList @(
            'publish', $project, '-c', 'Release', '-p:PublishProfile=FolderProfile'
        )
    }

    Write-Host '[copy]'

    $buildDir = Join-Path $PSScriptRoot 'Build'
    Remove-PathIfExists -Path $buildDir
    New-Item -ItemType Directory -Path (Join-Path $buildDir 'bin') -Force | Out-Null

    $toolsRoot = Join-Path $PSScriptRoot 'MicaSetup.Tools'

    # Destination names are the on-disk names shipped in the tools package.
    $publishedExes = @(
        @{ Project = 'MICA';         Source = 'MICA.exe';         Dest = 'MICA.exe' }
        @{ Project = 'CheckVer';     Source = 'CheckVer.exe';     Dest = 'checkver.exe' }
        @{ Project = 'FetchVer';     Source = 'FetchVer.exe';     Dest = 'fetchver.exe' }
        @{ Project = 'MakeMica.Cli'; Source = 'MakeMica.Cli.exe'; Dest = 'makemica.exe' }
        @{ Project = 'MakeIcon';     Source = 'MakeIcon.exe';     Dest = 'makeiconw.exe' }
        @{ Project = 'MakeIcon.Cli'; Source = 'MakeIcon.Cli.exe'; Dest = 'makeicon.exe' }
    )

    # MakeMica GUI is published above but not staged:
    # MakeMica.exe would have been renamed to makemicaw.exe.

    foreach ($exe in $publishedExes) {
        $source = Join-Path $toolsRoot "$($exe.Project)\bin\Release\publish\$($exe.Source)"
        Copy-Item -LiteralPath $source -Destination (Join-Path $buildDir $exe.Dest) -Force
    }

    $sevenZipDir = Join-Path $toolsRoot '7-Zip'
    Copy-Item -LiteralPath (Join-Path $sevenZipDir '7z.dll') -Destination (Join-Path $buildDir 'bin\7z.dll') -Force
    Copy-Item -LiteralPath (Join-Path $sevenZipDir '7z.exe') -Destination (Join-Path $buildDir 'bin\7z.exe') -Force

    Write-Host '[create]'

    Remove-PathIfExists -Path (Join-Path $PSScriptRoot 'MicaSetup\obj')
    Remove-PathIfExists -Path (Join-Path $PSScriptRoot 'MicaSetup\bin')
    Remove-PathIfExists -Path (Join-Path $PSScriptRoot 'MicaSetup\MicaSetup.csproj.user')
    Remove-PathIfExists -Path (Join-Path $PSScriptRoot 'MicaSetup\Resources\Setups\publish.7z')
    Remove-PathIfExists -Path (Join-Path $PSScriptRoot 'MicaSetup\Resources\Setups\publish.cer')
    Remove-PathIfExists -Path (Join-Path $PSScriptRoot 'MicaSetup\Resources\Setups\Uninst.exe')

    # Quote the wildcard so PowerShell does not expand it before 7-Zip sees it.
    $sevenZip = Join-Path $sevenZipDir '7z.exe'
    Invoke-Native -FilePath $sevenZip -ArgumentList @(
        'a', 'default.7z', '.\MicaSetup\*', '-t7z', '-mx=5', '-mf=BCJ2', '-r', '-y'
    )

    $templateDir = Join-Path $buildDir 'template'
    New-Item -ItemType Directory -Path $templateDir -Force | Out-Null
    Move-Item -LiteralPath (Join-Path $PSScriptRoot 'default.7z') -Destination (Join-Path $templateDir 'default.7z') -Force

    Write-Host '[nuget]'

    # Package.nuspec packs files from .\Build\, so pack after copy/create.
    Invoke-Native -FilePath 'dotnet' -ArgumentList @(
        'pack', 'MicaSetup.Tools\MicaSetup.Tools\MicaSetup.Tools.csproj', '-c', 'Release', '-p:PublishProfile=FolderProfile'
    )

    $packedDir = Join-Path $toolsRoot 'MicaSetup.Tools\bin\Release'
    $nupkgs = @(Get-ChildItem -LiteralPath $packedDir -Filter '*.nupkg' -File)
    if ($nupkgs.Count -eq 0) {
        throw "No .nupkg was produced in $packedDir"
    }
    Copy-Item -LiteralPath $nupkgs.FullName -Destination $PSScriptRoot -Force

    $snupkgs = @(Get-ChildItem -LiteralPath $packedDir -Filter '*.snupkg' -File -ErrorAction SilentlyContinue)
    if ($snupkgs.Count -gt 0) {
        Copy-Item -LiteralPath $snupkgs.FullName -Destination $PSScriptRoot -Force
    }

    Write-Host '[pack]'

    Invoke-Native -FilePath $sevenZip -ArgumentList @(
        'a', 'micasetup.7z', '.\Build\*', '-t7z', '-mx=5', '-mf=BCJ2', '-r', '-y'
    )

    # makemica reads .\micasetup.7z, then writes .\MicaSetup_v${Version}.exe.
    $packStarted = Get-Date
    Invoke-Native -FilePath (Join-Path $buildDir 'makemica.exe') -ArgumentList @('micasetup.json')

    $installers = @(Get-ChildItem -LiteralPath $PSScriptRoot -File | Where-Object {
            $_.Name -match '^MicaSetup_v\d+(\.\d+)+\.exe$' -and
            $_.LastWriteTime -ge $packStarted.AddSeconds(-5)
        })
    if ($installers.Count -ne 1) {
        throw "Expected makemica to produce one MicaSetup_v*.exe, found $($installers.Count)."
    }

    $archiveName = "$($installers[0].BaseName).7z"
    $archivePath = Join-Path $PSScriptRoot $archiveName
    if (Test-Path -LiteralPath $archivePath) {
        Remove-Item -LiteralPath $archivePath -Force
    }
    Move-Item -LiteralPath (Join-Path $PSScriptRoot 'micasetup.7z') -Destination $archivePath
    Write-Host "Archive: $archiveName"
}
catch {
    Write-Host $_.Exception.Message -ForegroundColor Yellow
    if ($null -ne $LASTEXITCODE -and $LASTEXITCODE -ne 0) {
        $exitCode = $LASTEXITCODE
    }
    else {
        $exitCode = 1
    }
}

$skipPause = $NoPause -or ($env:GITHUB_ACTIONS -eq 'true') -or ($env:CI -eq 'true')
if (-not $skipPause) {
    Write-Host ''
    Write-Host 'Press any key to exit...'
    try {
        [void][Console]::ReadKey($true)
    }
    catch {
        # Ignore when console input is unavailable (e.g. redirected I/O).
    }
}

exit $exitCode
