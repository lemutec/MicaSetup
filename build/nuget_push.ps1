param(
    [string]$ApiKey
)

Set-Location $PSScriptRoot

# Avoid mojibake: Windows PowerShell defaults to the system ANSI/OEM code page (e.g. GBK),
# while Unicode banners and `dotnet` CLI localized output are UTF-8.
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
# Keep CLI messages in English so push errors stay readable even if the host encoding drifts.
$env:DOTNET_CLI_UI_LANGUAGE = 'en'

# Symmetric key for encrypting %USERPROFILE%\.nuget\push.token (do not commit real prod secrets elsewhere)
$script:TokenCryptoSecret = "vK9mP2xR7nQ4wL8sT6yH1jF5cB0aZ3eD9uI2oG7pA4kM6nX8qW5rY1tU3vS0bN7cF2hJ9gL4mP6xR8wT1yH5jK0aZ3eD"

$tokenPath = Join-Path $env:USERPROFILE ".nuget\push.token"

function Get-AesKeyBytes {
    $sha = [System.Security.Cryptography.SHA256]::Create()
    try {
        return $sha.ComputeHash([System.Text.Encoding]::UTF8.GetBytes($script:TokenCryptoSecret))
    }
    finally {
        $sha.Dispose()
    }
}

function Protect-NuGetToken {
    param([Parameter(Mandatory = $true)][string]$PlainText)

    $aes = [System.Security.Cryptography.Aes]::Create()
    try {
        $aes.Key = Get-AesKeyBytes
        $aes.GenerateIV()
        $aes.Mode = [System.Security.Cryptography.CipherMode]::CBC
        $aes.Padding = [System.Security.Cryptography.PaddingMode]::PKCS7

        $encryptor = $aes.CreateEncryptor()
        try {
            $plainBytes = [System.Text.Encoding]::UTF8.GetBytes($PlainText)
            $cipherBytes = $encryptor.TransformFinalBlock($plainBytes, 0, $plainBytes.Length)
            $payload = New-Object byte[] ($aes.IV.Length + $cipherBytes.Length)
            [Array]::Copy($aes.IV, 0, $payload, 0, $aes.IV.Length)
            [Array]::Copy($cipherBytes, 0, $payload, $aes.IV.Length, $cipherBytes.Length)
            return [Convert]::ToBase64String($payload)
        }
        finally {
            $encryptor.Dispose()
        }
    }
    finally {
        $aes.Dispose()
    }
}

function Unprotect-NuGetToken {
    param([Parameter(Mandatory = $true)][string]$CipherText)

    $payload = [Convert]::FromBase64String($CipherText.Trim())
    if ($payload.Length -lt 17) {
        throw "Stored token payload is invalid."
    }

    $aes = [System.Security.Cryptography.Aes]::Create()
    try {
        $aes.Key = Get-AesKeyBytes
        $aes.Mode = [System.Security.Cryptography.CipherMode]::CBC
        $aes.Padding = [System.Security.Cryptography.PaddingMode]::PKCS7

        $iv = New-Object byte[] 16
        [Array]::Copy($payload, 0, $iv, 0, 16)
        $aes.IV = $iv

        $cipherLen = $payload.Length - 16
        $cipherBytes = New-Object byte[] $cipherLen
        [Array]::Copy($payload, 16, $cipherBytes, 0, $cipherLen)

        $decryptor = $aes.CreateDecryptor()
        try {
            $plainBytes = $decryptor.TransformFinalBlock($cipherBytes, 0, $cipherBytes.Length)
            return [System.Text.Encoding]::UTF8.GetString($plainBytes)
        }
        finally {
            $decryptor.Dispose()
        }
    }
    finally {
        $aes.Dispose()
    }
}

function Save-NuGetToken {
    param([Parameter(Mandatory = $true)][string]$PlainText)

    $nugetDir = Split-Path -Parent $tokenPath
    if (-not (Test-Path -LiteralPath $nugetDir)) {
        New-Item -ItemType Directory -Path $nugetDir -Force | Out-Null
    }

    $encrypted = Protect-NuGetToken -PlainText $PlainText
    Set-Content -LiteralPath $tokenPath -Value $encrypted -Encoding ascii -NoNewline
    Write-Host "Token saved to $tokenPath (encrypted)."
}

function Read-StoredNuGetToken {
    if (-not (Test-Path -LiteralPath $tokenPath)) {
        return $null
    }

    try {
        $raw = Get-Content -LiteralPath $tokenPath -Raw -ErrorAction Stop
        if ([string]::IsNullOrWhiteSpace($raw)) {
            return $null
        }
        return Unprotect-NuGetToken -CipherText $raw
    }
    catch {
        Write-Host "Failed to read/decrypt stored token: $($_.Exception.Message)"
        return $null
    }
}

function Request-NuGetToken {
    param([string]$Reason)

    if ($Reason) {
        Write-Host $Reason
    }

    $inputKey = Read-Host "Enter NuGet API Key"
    if ([string]::IsNullOrWhiteSpace($inputKey)) {
        throw "NuGet API Key is required."
    }

    Save-NuGetToken -PlainText $inputKey.Trim()
    return $inputKey.Trim()
}

function Resolve-NuGetApiKey {
    if (-not [string]::IsNullOrWhiteSpace($ApiKey)) {
        return $ApiKey.Trim()
    }

    $stored = Read-StoredNuGetToken
    if (-not [string]::IsNullOrWhiteSpace($stored)) {
        Write-Host "Using encrypted token from $tokenPath"
        return $stored
    }

    return Request-NuGetToken -Reason "No stored token at $tokenPath"
}

function Invoke-NuGetPush {
    param([Parameter(Mandatory = $true)][string]$Key)

    & dotnet nuget push *.nupkg --api-key $Key --source https://api.nuget.org/v3/index.json
    return $LASTEXITCODE
}

Write-Host @"
███╗   ██╗██╗   ██╗ ██████╗ ███████╗████████╗
████╗  ██║██║   ██║██╔════╝ ██╔════╝╚══██╔══╝
██╔██╗ ██║██║   ██║██║  ███╗█████╗     ██║   
██║╚██╗██║██║   ██║██║   ██║██╔══╝     ██║   
██║ ╚████║╚██████╔╝╚██████╔╝███████╗   ██║   
╚═╝  ╚═══╝ ╚═════╝  ╚═════╝ ╚══════╝   ╚═╝   
"@

$resolvedKey = Resolve-NuGetApiKey
$exitCode = Invoke-NuGetPush -Key $resolvedKey

# Token expired / rejected / push failed: prompt, overwrite stored token, retry once
if ($exitCode -ne 0) {
    Write-Host "`nNuGet push failed (exit code $exitCode). Token may be invalid or expired."
    $resolvedKey = Request-NuGetToken -Reason "Please enter a new NuGet API Key."
    $exitCode = Invoke-NuGetPush -Key $resolvedKey
}

if ($exitCode -ne 0) {
    Write-Host "`nNuGet push failed again (exit code $exitCode)."
}

Write-Host "`nPress any key to exit..."
[void][System.Console]::ReadKey($true)
exit $exitCode
