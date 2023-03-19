cd /d %~dp0
cd MicaSetup
set "path=%path%;C:\Program Files (x86)\Microsoft SDKs\Windows\v10.0A\bin\NETFX 4.8 Tools"
sn -k app.snk
cd /d %~dp0
@pause
