@echo off

cd %~d0%~p0%

pushd
call "%VS100COMNTOOLS%\..\..\VC\bin\vcvars32.bat"
popd

if exist "%~n1.res" del /Q "%~n1.res"

rc.exe /nologo /r "%~n1.rc"

del /Q "%~n1.aps" 2>NUL

move "%~n1.RES" "%~n1.tmp" 1>NUL
move "%~n1.tmp" "%~n1.res" 1>NUL

echo Compiled resources