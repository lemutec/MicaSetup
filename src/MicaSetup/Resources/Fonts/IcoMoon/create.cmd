cd /d %~dp0
@REM npm i -g icomoon-cli
@REM icomoon-cli -h
@REM icomoon-cli -i 1.svg -s selection.json -n 1 -o output
@REM icomoon-cli -i "%~1" -s selection.json -n "%~n1" -o output
for %%i in (%*) do (
  icomoon-cli -i "%%~i" -s selection.json -n "%%~ni" -o output
)
@pause
