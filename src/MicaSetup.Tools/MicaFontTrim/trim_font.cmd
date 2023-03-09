@REM pip install fonttools
cd /d %~dp0
cd subset
del subset.txt
dotnet run
cd ..
del HarmonyOS_Sans_SC_Regular.ttf
fonttools subset "fonts/HarmonyOS_Sans_SC_Regular.ttf" --text-file="subset/subset.txt" --output-file="HarmonyOS_Sans_SC_Regular.ttf"
copy /y HarmonyOS_Sans_SC_Regular.ttf ..\..\MicaSetup\Resources\Fonts\HarmonyOS_Sans_SC_Regular.ttf
@REM @pause
