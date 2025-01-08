cd /d %~dp0

echo [build]

dotnet publish MicaSetup.Tools\FetchVer\FetchVer.csproj -c Release -p:PublishProfile=FolderProfile
dotnet publish MicaSetup.Tools\MakeIcon\MakeIcon.csproj -c Release -p:PublishProfile=FolderProfile
dotnet publish MicaSetup.Tools\MakeIcon.Cli\MakeIcon.Cli.csproj -c Release -p:PublishProfile=FolderProfile
dotnet publish MicaSetup.Tools\MakeMica\MakeMica.csproj -c Release -p:PublishProfile=FolderProfile
dotnet publish MicaSetup.Tools\MakeMica.Cli\MakeMica.Cli.csproj -c Release -p:PublishProfile=FolderProfile
dotnet publish MicaSetup.Tools\MakeMui\MakeMui.csproj -c Release -p:PublishProfile=FolderProfile
dotnet publish MicaSetup.Tools\MICA\MICA.csproj -c Release -p:PublishProfile=FolderProfile

echo [copy]

rd /s /q .\Build\

mkdir .\Build\
mkdir .\Build\bin\

copy /y .\MicaSetup.Tools\MICA\bin\Release\publish\MICA.exe .\Build\

copy /y .\MicaSetup.Tools\FetchVer\bin\Release\publish\FetchVer.exe .\Build\
ren .\Build\FetchVer.exe fetchver.exe

@REM copy /y .\MicaSetup.Tools\MakeMica\bin\Release\publish\MakeMica.exe .\Build\
@REM del .\Build\makemicaw.exe
@REM ren .\Build\MakeMica.exe makemicaw.exe

copy /y .\MicaSetup.Tools\MakeMica.Cli\bin\Release\publish\MakeMica.Cli.exe .\Build\
del .\Build\makemica.exe
ren .\Build\MakeMica.Cli.exe makemica.exe

copy /y .\MicaSetup.Tools\MakeIcon\bin\Release\publish\MakeIcon.exe .\Build\
del .\Build\makeiconw.exe
ren .\Build\MakeIcon.exe makeiconw.exe

copy /y .\MicaSetup.Tools\MakeIcon.Cli\bin\Release\publish\MakeIcon.Cli.exe .\Build\
del .\Build\makeicon.exe
ren .\Build\MakeIcon.Cli.exe makeicon.exe

copy /y .\MicaSetup.Tools\7-Zip\7z.dll .\Build\bin\7z.dll
copy /y .\MicaSetup.Tools\7-Zip\7z.exe .\Build\bin\7z.exe

echo [create]

rd /s /q .\MicaSetup\obj
rd /s /q .\MicaSetup\bin
del .\MicaSetup\MicaSetup.csproj.user
del .\MicaSetup\Resources\Setups\publish.7z
del .\MicaSetup\Resources\Setups\publish.cer
del .\MicaSetup\Resources\Setups\Uninst.exe
MicaSetup.Tools\7-Zip\7z a default.7z .\MicaSetup\* -t7z -mx=5 -mf=BCJ2 -r -y
mkdir .\Build\template
move default.7z .\Build\template\default.7z

echo [pack]

MicaSetup.Tools\7-Zip\7z a micasetup.7z .\Build\* -t7z -mx=5 -mf=BCJ2 -r -y
Build\makemica micasetup.json

@pause
