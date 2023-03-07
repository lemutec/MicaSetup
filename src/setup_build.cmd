@REM Copy yourapp.7z to .\MicaSetup\Resources\Setups\publish.7z
@REM copy yourapp.7z .\MicaSetup\Resources\Setups\publish.7z
@REM xcopy path\to\yourapp.7z .\MicaSetup\Resources\Setups\publish.7z /y /e /d
for /f "usebackq tokens=*" %%i in (`"%ProgramFiles(x86)%\Microsoft Visual Studio\Installer\vswhere.exe" -latest -property installationPath`) do set "path=%path%;%%i\MSBuild\Current\Bin"
msbuild MicaSetup\MicaSetup.Uninst.csproj /t:Rebuild /p:Configuration=Release /p:DeployOnBuild=true /p:PublishProfile=FolderProfile /restore
copy /y .\MicaSetup\bin\Release\net48\MicaSetup.exe .\MicaSetup\Resources\Setups\Uninst.exe
msbuild MicaSetup\MicaSetup.csproj /t:Rebuild /p:Configuration=Release /p:DeployOnBuild=true /p:PublishProfile=FolderProfile
copy /y .\MicaSetup\bin\Release\net48\MicaSetup.exe .\
@pause
