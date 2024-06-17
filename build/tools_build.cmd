cd /d %~dp0

@echo [prepare somethings]
del MicaSetup.exe
for /f "usebackq tokens=*" %%i in (`"%ProgramFiles(x86)%\Microsoft Visual Studio\Installer\vswhere.exe" -latest -property installationPath`) do set "path=%path%;%%i\MSBuild\Current\Bin;%%i\Common7\IDE"

echo [build app using vs2022]
cd ..\src\
dotnet publish -c Release -p:PublishProfile=FolderProfile
cd /d %~dp0

@echo [finish]
mkdir .\Build\
mkdir .\Build\bin\

copy /y .\MicaSetup.Tools\MakeMica\bin\Release\publish\MakeMica.exe .\Build\
ren .\Build\MakeMica.exe makemicaw.exe

copy /y .\MicaSetup.Tools\MakeMica.Cli\bin\Release\publish\MakeMica.Cli.exe .\Build\
ren .\Build\MakeMica.Cli.exe makemica.exe

copy /y .\MicaSetup.Tools\MakeIcon\bin\Release\publish\MakeIcon.exe .\Build\
ren .\Build\MakeIcon.exe makeiconw.exe

copy /y .\MicaSetup.Tools\MakeIcon.Cli\bin\Release\publish\MakeIcon.Cli.exe .\Build\
ren .\Build\MakeIcon.Cli.exe makeicon.exe

copy /y .\MicaSetup.Tools\MakeMui\bin\Release\publish\MakeMui.exe .\Build\
ren .\Build\MakeMui.exe makemuiw.exe

copy /y .\MicaSetup.Tools\7-Zip\7z.dll .\Build\bin\7z.dll
copy /y .\MicaSetup.Tools\7-Zip\7z.exe .\Build\bin\7z.exe

@pause
