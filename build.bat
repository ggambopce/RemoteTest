@echo off
"C:\Program Files\Microsoft Visual Studio\2022\Professional\MSBuild\Current\Bin\MSBuild.exe" "D:\Work\VDesk\remotetest\remotetest.sln" /p:Configuration=Release /v:minimal
echo Exit code: %ERRORLEVEL%
