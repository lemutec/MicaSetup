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
$env:DOTNET_CLI_UI_LANGUAGE = 'en'

Write-Host @"
███╗   ██╗██╗   ██╗ ██████╗ ███████╗████████╗
████╗  ██║██║   ██║██╔════╝ ██╔════╝╚══██╔══╝
██╔██╗ ██║██║   ██║██║  ███╗█████╗     ██║   
██║╚██╗██║██║   ██║██║   ██║██╔══╝     ██║   
██║ ╚████║╚██████╔╝╚██████╔╝███████╗   ██║   
╚═╝  ╚═══╝ ╚═════╝  ╚═════╝ ╚══════╝   ╚═╝   
"@

cd ..
git clean -xdf

Write-Host "`nPress any key to exit..."
try {
    [void][Console]::ReadKey($true)
} catch {
    # Ignore when console input is unavailable (e.g. CI or automated runs).
}
