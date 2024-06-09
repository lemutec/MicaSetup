cd /d %~dp0

@echo [prepare somethings]
del MicaSetup.exe
for /f "usebackq tokens=*" %%i in (`"%ProgramFiles(x86)%\Microsoft Visual Studio\Installer\vswhere.exe" -latest -property installationPath`) do set "path=%path%;%%i\MSBuild\Current\Bin;%%i\Common7\IDE"

echo [build app using vs2022]
cd ..\src\
dotnet publish -c Release -p:PublishProfile=FolderProfile
cd /d %~dp0

echo [pack app using 7z]
MicaSetup.Tools\7-Zip\7z a publish.7z ..\src\MicaApp\bin\Release\publish\* -t7z -mx=5 -mf=BCJ2 -r -y
copy /y publish.7z .\MicaSetup\Resources\Setups\publish.7z

@echo [build uninst using vs2022]
msbuild MicaSetup\MicaSetup.Uninst.csproj /t:Rebuild /p:Configuration=Release /p:DeployOnBuild=true /p:PublishProfile=FolderProfile /restore

@echo [build setup using vs2022]
copy /y .\MicaSetup\bin\Release\MicaSetup.exe .\MicaSetup\Resources\Setups\Uninst.exe
msbuild MicaSetup\MicaSetup.csproj /t:Build /p:Configuration=Release /p:DeployOnBuild=true /p:PublishProfile=FolderProfile /restore

@echo [finish]
copy /y .\MicaSetup\bin\Release\MicaSetup.exe .\

@pause
