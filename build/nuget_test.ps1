#Requires -Version 5.1
<#
.SYNOPSIS
  Restore all .nupkg / .snupkg files in this script's directory (build/)
  into the local NuGet global packages folder.

.DESCRIPTION
  Drop-in local replacement for "pmc add": no CLI arguments.
  After nuget_pack.ps1 writes packages here, run this script to install
  each .nupkg (extract) / .snupkg (copy) under the resolved global packages
  folder, and write the .sha512 / .nupkg.metadata files NuGet expects.

  Resolution order for the global packages folder:
    1. NUGET_PACKAGES environment variable
    2. globalPackagesFolder / globalPackagesPath in NuGet.Config
       (walk up from cwd, then user, then machine config)
    3. %USERPROFILE%\.nuget\packages
#>
$ErrorActionPreference = 'Stop'

Set-Location $PSScriptRoot

# Avoid mojibake: Windows PowerShell defaults to the system ANSI/OEM code page (e.g. GBK),
# while Unicode banners and tool output are UTF-8.
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

Write-Host @"
███╗   ██╗██╗   ██╗ ██████╗ ███████╗████████╗
████╗  ██║██║   ██║██╔════╝ ██╔════╝╚══██╔══╝
██╔██╗ ██║██║   ██║██║  ███╗█████╗     ██║   
██║╚██╗██║██║   ██║██║   ██║██╔══╝     ██║   
██║ ╚████║╚██████╔╝╚██████╔╝███████╗   ██║   
╚═╝  ╚═══╝ ╚═════╝  ╚═════╝ ╚══════╝   ╚═╝   
"@

# Zip APIs used to open .nupkg/.snupkg and extract package content.
Add-Type -AssemblyName System.IO.Compression
Add-Type -AssemblyName System.IO.Compression.FileSystem

<#
.SYNOPSIS
  Expand "~/" / environment-variable tokens in a configured path.
#>
function Expand-PathToken {
    param([string]$PathValue)

    # NuGet configs often use "~/..." as a shorthand for the user profile.
    if ($PathValue.StartsWith('~/') -or $PathValue.StartsWith('~\')) {
        return Join-Path $env:USERPROFILE $PathValue.Substring(2)
    }

    return [Environment]::ExpandEnvironmentVariables($PathValue)
}

<#
.SYNOPSIS
  Enumerate candidate NuGet.Config paths in NuGet's usual search order.
#>
function Get-NugetConfigPaths {
    $paths = New-Object System.Collections.Generic.List[string]

    # Walk from the current directory up to the drive root.
    $dir = Get-Item -LiteralPath (Get-Location).Path
    while ($null -ne $dir) {
        foreach ($name in @('nuget.config', 'NuGet.Config')) {
            $candidate = Join-Path $dir.FullName $name
            if (Test-Path -LiteralPath $candidate) {
                $paths.Add($candidate)
            }
        }
        $dir = $dir.Parent
    }

    # User-level config: %APPDATA%\NuGet\NuGet.Config
    if (-not [string]::IsNullOrEmpty($env:APPDATA)) {
        $userConfig = Join-Path $env:APPDATA 'NuGet\NuGet.Config'
        if (Test-Path -LiteralPath $userConfig) {
            $paths.Add($userConfig)
        }
    }

    # Machine-level config: %ProgramData%\NuGet\NuGet.Config (Windows)
    if (-not [string]::IsNullOrEmpty($env:ProgramData)) {
        $machineConfig = Join-Path $env:ProgramData 'NuGet\NuGet.Config'
        if (Test-Path -LiteralPath $machineConfig) {
            $paths.Add($machineConfig)
        }
    }

    return $paths
}

<#
.SYNOPSIS
  Read globalPackagesFolder / globalPackagesPath from a NuGet.Config file.
#>
function Read-GlobalPackagesFolderFromConfig {
    param([string]$ConfigPath)

    try {
        [xml]$xml = Get-Content -LiteralPath $ConfigPath -Raw
    }
    catch {
        # Corrupt or unreadable config: ignore and keep falling back.
        return $null
    }

    $adds = @($xml.SelectNodes('//add'))
    foreach ($node in $adds) {
        $key = [string]$node.GetAttribute('key')
        if ($key -eq 'globalPackagesFolder' -or $key -eq 'globalPackagesPath') {
            $value = [string]$node.GetAttribute('value')
            if (-not [string]::IsNullOrWhiteSpace($value)) {
                return $value.Trim()
            }
        }
    }

    return $null
}

<#
.SYNOPSIS
  Resolve the NuGet global packages folder (globalPackagesFolder).
#>
function Resolve-GlobalPackagesFolder {
    # Highest precedence: explicit environment override.
    $fromEnv = $env:NUGET_PACKAGES
    if (-not [string]::IsNullOrWhiteSpace($fromEnv)) {
        return [System.IO.Path]::GetFullPath((Expand-PathToken $fromEnv.Trim()))
    }

    # Next: first matching setting found while walking NuGet.Config locations.
    foreach ($configPath in (Get-NugetConfigPaths)) {
        $folder = Read-GlobalPackagesFolderFromConfig -ConfigPath $configPath
        if (-not [string]::IsNullOrWhiteSpace($folder)) {
            return [System.IO.Path]::GetFullPath((Expand-PathToken $folder))
        }
    }

    # Default NuGet layout on Windows / Unix-like profiles.
    return Join-Path $env:USERPROFILE '.nuget\packages'
}

<#
.SYNOPSIS
  Read package id and version from the embedded .nuspec inside a package zip.
#>
function Read-PackageIdentity {
    param([string]$PackagePath)

    $archive = [System.IO.Compression.ZipFile]::OpenRead($PackagePath)
    try {
        # Prefer a root-level .nuspec (standard nupkg layout).
        $nuspec = $archive.Entries |
            Where-Object {
                $_.FullName.EndsWith('.nuspec', [StringComparison]::OrdinalIgnoreCase) -and
                (-not $_.FullName.Contains('/')) -and
                (-not $_.FullName.Contains('\'))
            } |
            Select-Object -First 1

        # Fallback: any .nuspec entry if the package is oddly laid out.
        if ($null -eq $nuspec) {
            $nuspec = $archive.Entries |
                Where-Object { $_.FullName.EndsWith('.nuspec', [StringComparison]::OrdinalIgnoreCase) } |
                Select-Object -First 1
        }

        if ($null -eq $nuspec) {
            throw "No .nuspec found in the package: $PackagePath"
        }

        $stream = $nuspec.Open()
        try {
            $settings = New-Object System.Xml.XmlReaderSettings
            $settings.IgnoreComments = $true
            $settings.IgnoreWhitespace = $true
            $settings.DtdProcessing = [System.Xml.DtdProcessing]::Prohibit

            $reader = [System.Xml.XmlReader]::Create($stream, $settings)
            try {
                $id = $null
                $version = $null

                while ($reader.Read()) {
                    if ($reader.NodeType -ne [System.Xml.XmlNodeType]::Element -or
                        -not $reader.LocalName.Equals('metadata', [StringComparison]::OrdinalIgnoreCase)) {
                        continue
                    }

                    # Read id/version only from the <metadata> subtree.
                    # ReadElementContentAsString advances the reader; do not
                    # call Read() again on the same iteration or version is skipped.
                    $meta = $reader.ReadSubtree()
                    $moved = $meta.Read()
                    while ($moved) {
                        if ($meta.NodeType -eq [System.Xml.XmlNodeType]::Element) {
                            if ($meta.LocalName.Equals('id', [StringComparison]::OrdinalIgnoreCase)) {
                                $id = $meta.ReadElementContentAsString().Trim()
                                continue
                            }
                            if ($meta.LocalName.Equals('version', [StringComparison]::OrdinalIgnoreCase)) {
                                $version = $meta.ReadElementContentAsString().Trim()
                                continue
                            }
                        }

                        if (($null -ne $id) -and ($null -ne $version)) {
                            break
                        }

                        $moved = $meta.Read()
                    }

                    break
                }

                if ([string]::IsNullOrWhiteSpace($id) -or [string]::IsNullOrWhiteSpace($version)) {
                    throw "Unable to read id/version from .nuspec in: $PackagePath"
                }

                return [pscustomobject]@{
                    Id      = $id
                    Version = $version
                }
            }
            finally {
                $reader.Dispose()
            }
        }
        finally {
            $stream.Dispose()
        }
    }
    finally {
        $archive.Dispose()
    }
}

<#
.SYNOPSIS
  Compute SHA-512 of a file and return it as Base64 (NuGet contentHash form).
#>
function Get-FileSha512Base64 {
    param([string]$FilePath)

    # Same algorithm NuGet uses to validate local packages:
    # hash raw file bytes, then encode as Base64 (not hex).
    $sha = [System.Security.Cryptography.SHA512]::Create()
    try {
        $stream = [System.IO.File]::OpenRead($FilePath)
        try {
            $hashBytes = $sha.ComputeHash($stream)
            return [Convert]::ToBase64String($hashBytes)
        }
        finally {
            $stream.Dispose()
        }
    }
    finally {
        $sha.Dispose()
    }
}

<#
.SYNOPSIS
  Write the sidecar *.nupkg.sha512 / *.snupkg.sha512 checksum file.
#>
function Write-NugetSha512File {
    param(
        [string]$PackageFilePath,
        [string]$HashBase64
    )

    $shaPath = "$PackageFilePath.sha512"
    # NuGet checksum file format: plain Base64 only — no newline, no file name.
    [System.IO.File]::WriteAllText($shaPath, $HashBase64)
    return $shaPath
}

<#
.SYNOPSIS
  Write .nupkg.metadata so NuGet treats the folder as a locally installed package.
#>
function Write-NupkgMetadata {
    param(
        [string]$TargetDir,
        [string]$ContentHash
    )

    $path = Join-Path $TargetDir '.nupkg.metadata'
    # Minimal v2 metadata; "source": "local" marks a hand-installed package.
    $json = @(
        '{'
        '  "version": 2,'
        "  `"contentHash`": `"$ContentHash`","
        '  "source": "local"'
        '}'
    ) -join "`n"
    [System.IO.File]::WriteAllText($path, $json)
}

<#
.SYNOPSIS
  Extract package payload into the version folder, skipping OPC packaging junk.
#>
function Expand-PackageContent {
    param(
        [string]$PackagePath,
        [string]$TargetDir
    )

    # OPC / packaging entries that must not land in the global packages tree.
    $skipRoot = [System.Collections.Generic.HashSet[string]]::new(
        [string[]]@('[Content_Types].xml', '_rels', 'package'),
        [StringComparer]::OrdinalIgnoreCase
    )

    # Guard against zip-slip: every extracted path must stay under TargetDir.
    $targetFull = [System.IO.Path]::GetFullPath($TargetDir).TrimEnd('\', '/')
    $targetPrefix = $targetFull + [System.IO.Path]::DirectorySeparatorChar

    $archive = [System.IO.Compression.ZipFile]::OpenRead($PackagePath)
    try {
        foreach ($entry in $archive.Entries) {
            # Directory entries have an empty Name; skip them.
            if ([string]::IsNullOrEmpty($entry.Name)) {
                continue
            }

            $relative = $entry.FullName.Replace('\', '/')
            $root = ($relative -split '/', 2)[0]
            if ($skipRoot.Contains($root)) {
                continue
            }

            $destPath = [System.IO.Path]::GetFullPath(
                (Join-Path $TargetDir ($relative.Replace('/', [System.IO.Path]::DirectorySeparatorChar)))
            )

            if (-not ($destPath.StartsWith($targetPrefix, [StringComparison]::OrdinalIgnoreCase) -or
                      $destPath.Equals($targetFull, [StringComparison]::OrdinalIgnoreCase))) {
                throw "Illegal package path: $($entry.FullName)"
            }

            $destDir = [System.IO.Path]::GetDirectoryName($destPath)
            if (-not [string]::IsNullOrEmpty($destDir)) {
                [void][System.IO.Directory]::CreateDirectory($destDir)
            }

            [System.IO.Compression.ZipFileExtensions]::ExtractToFile($entry, $destPath, $true)
        }
    }
    finally {
        $archive.Dispose()
    }
}

<#
.SYNOPSIS
  Ensure the extracted .nuspec file name matches "{id}.nuspec" (lowercase id).
#>
function Rename-NuspecIfNeeded {
    param(
        [string]$TargetDir,
        [string]$PackageIdLower
    )

    $expectedName = "$PackageIdLower.nuspec"
    $expectedPath = Join-Path $TargetDir $expectedName
    $nuspecs = @(Get-ChildItem -LiteralPath $TargetDir -Filter '*.nuspec' -File)
    if ($nuspecs.Count -eq 0) {
        return
    }

    $current = $nuspecs[0]
    # Compare the file name itself (paths are case-insensitive on Windows).
    if (-not [string]::Equals($current.Name, $expectedName, [StringComparison]::Ordinal)) {
        Move-Item -LiteralPath $current.FullName -Destination $expectedPath -Force
    }
}

<#
.SYNOPSIS
  Install one .nupkg or .snupkg into the global packages folder layout.
.NOTES
  Layout: {packagesRoot}\{id}\{version}\
  - .nupkg: extract content, copy the package, write checksum + metadata
  - .snupkg: copy the symbol package and write checksum only (do not wipe content)
#>
function Add-PackageToGlobalFolder {
    param(
        [string]$PackagePath,
        [string]$PackagesRoot
    )

    $identity = Read-PackageIdentity -PackagePath $PackagePath
    # NuGet always lowercases id and version folder names on disk.
    $idFolder = $identity.Id.ToLowerInvariant()
    $versionFolder = $identity.Version.ToLowerInvariant()
    $targetDir = Join-Path $PackagesRoot (Join-Path $idFolder $versionFolder)

    [void][System.IO.Directory]::CreateDirectory($targetDir)

    $ext = [System.IO.Path]::GetExtension($PackagePath)
    $isSymbol = $ext.Equals('.snupkg', [StringComparison]::OrdinalIgnoreCase)
    $packageFileName = if ($isSymbol) {
        "$idFolder.$versionFolder.snupkg"
    }
    else {
        "$idFolder.$versionFolder.nupkg"
    }
    $targetPackagePath = Join-Path $targetDir $packageFileName

    if ($isSymbol) {
        # Symbol package: copy .snupkg beside the main package; leave extracted files alone.
        Copy-Item -LiteralPath $PackagePath -Destination $targetPackagePath -Force
    }
    else {
        Expand-PackageContent -PackagePath $PackagePath -TargetDir $targetDir
        Copy-Item -LiteralPath $PackagePath -Destination $targetPackagePath -Force
        Rename-NuspecIfNeeded -TargetDir $targetDir -PackageIdLower $idFolder
    }

    $hash = Get-FileSha512Base64 -FilePath $targetPackagePath
    $shaPath = Write-NugetSha512File -PackageFilePath $targetPackagePath -HashBase64 $hash

    if (-not $isSymbol) {
        Write-NupkgMetadata -TargetDir $targetDir -ContentHash $hash
    }

    return [pscustomobject]@{
        PackageId       = $identity.Id
        Version         = $identity.Version
        TargetDirectory = $targetDir
        Sha512Path      = $shaPath
    }
}

# ---------------------------------------------------------------------------
# Main: restore every package file found next to this script (build/).
# ---------------------------------------------------------------------------

$workDir = $PSScriptRoot
$files = @(
    Get-ChildItem -LiteralPath $workDir -Filter '*.nupkg' -File
    Get-ChildItem -LiteralPath $workDir -Filter '*.snupkg' -File
) | Sort-Object Name

$exitCode = 0

if ($files.Count -eq 0) {
    Write-Host "No .nupkg or .snupkg files were found in $workDir." -ForegroundColor Yellow
}
else {
    $packagesRoot = Resolve-GlobalPackagesFolder
    Write-Host "Global packages folder: $packagesRoot"
    Write-Host "Restoring $($files.Count) package file(s) from $workDir ..."

    $failed = 0
    foreach ($file in $files) {
        Write-Host "  restore $($file.Name)"
        try {
            $result = Add-PackageToGlobalFolder -PackagePath $file.FullName -PackagesRoot $packagesRoot
            Write-Host "    Restored: $($result.PackageId) $($result.Version)"
            Write-Host "    Target directory: $($result.TargetDirectory)"
            Write-Host "    Checksum file: $($result.Sha512Path)"
        }
        catch {
            # Keep going so one bad package does not block the rest.
            $failed++
            Write-Host "    Failed: $($_.Exception.Message)" -ForegroundColor Yellow
        }
    }

    if ($failed -gt 0) {
        Write-Host "`nCompleted with $failed failure(s)." -ForegroundColor Yellow
        $exitCode = 1
    }
    else {
        Write-Host "`nAll done."
    }
}

Write-Host "Press any key to exit..."
[void][System.Console]::ReadKey($true)
exit $exitCode
