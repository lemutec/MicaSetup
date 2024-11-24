rd /s /q .\MicaSetup\obj
rd /s /q .\MicaSetup\bin
del .\MicaSetup\MicaSetup.csproj.user
del .\MicaSetup\Resources\Setups\publish.7z
del .\MicaSetup\Resources\Setups\publish.cer
del .\MicaSetup\Resources\Setups\Uninst.exe
MicaSetup.Tools\7-Zip\7z a default.7z .\MicaSetup\* -t7z -mx=5 -mf=BCJ2 -r -y
mkdir .\Build\template
move default.7z .\Build\template\default.7z
@pause
