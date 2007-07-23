@echo off

pushd
call "C:\Program Files\Microsoft Visual Studio 8\VC\bin\vcvars32.bat"
popd

del /Q "%~1.res"

rc.exe /r "%~1.rc"

del /Q "%~1.aps" 2>NUL

move "%~1.RES" "%~1.tmp" 1>NUL
move "%~1.tmp" "%~1.res" 1>NUL

echo Compiled resources