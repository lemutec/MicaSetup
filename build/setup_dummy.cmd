cd /d %~dp0

@echo [prepare test publish.7z]
echo test > test.txt
MicaSetup.Tools\7-Zip\7z a publish.7z test.txt -t7z -mx=5 -mf=BCJ2 -r -y
del test.txt
copy publish.7z .\MicaSetup\Resources\Setups\publish.7z

@echo [prepare dummy uninst]
echo dummy-uninst > uninst.exe
copy uninst.exe .\MicaSetup\Resources\Setups\Uninst.exe
del uninst.exe

@pause
