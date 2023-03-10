# 🏆Usage

## ⚙️Debug Codes

Run the `setup_dummy.cmd` for creating some dummy things used in project.

Such as `publish.7z` as your app published package and full size font used in Setup programs.

**The same idea for fixing following errors**

```bash
3>C:\Program Files\dotnet\sdk\7.0.101\Sdks\Microsoft.NET.Sdk.WindowsDesktop\targets\Microsoft.WinFX.targets(705,5): error BG1002: File 'Resources\Fonts\HarmonyOS_Sans_SC_Regular.ttf' cannot be found.
3>C:\Program Files\dotnet\sdk\7.0.101\Sdks\Microsoft.NET.Sdk.WindowsDesktop\targets\Microsoft.WinFX.targets(705,5): error BG1002: File 'Resources\Setups\publish.7z' cannot be found.
3>C:\Program Files\dotnet\sdk\7.0.101\Sdks\Microsoft.NET.Sdk.WindowsDesktop\targets\Microsoft.WinFX.targets(705,5): error BG1002: File 'Resources\Setups\Uninst.exe' cannot be found.
3>Done building project "MicaSetup.csproj" -- FAILED.
```

## 📦Create Setup

Run the `setup_build.cmd` for building the full Setup programs.

Default using VS2022.

**Requests**

```bash
# For trim the font size.
pip install fonttools
```

**Command Flow**

1. Build my app, named `MicaApp` using `dotnet publish`.
2. Pack my app using `7z`.
3. Trim font using `fonttools`.
4. Build Uninst, named `Uninst.exe` using `msbuild`.
5. Build Setup, named `MicaSetup.exe` using `msbuild`.

## 🌒Add Icon

We use icon with font type icon, and add new icon with `IcoMoon CLI`.

See more [here](MicaSetup\Resources\Fonts\IcoMoon\README.md).

**Requests**

```bash
# For add icon to your icomoon project, file named `selection.json`.
npm i -g icomoon-cli
```

