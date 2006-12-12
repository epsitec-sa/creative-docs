@echo off

pushd
call "C:\Program Files\Microsoft Visual Studio 8\VC\bin\vcvars32.bat"
popd

del /Q %1.res

rc.exe /r %1.rc

del /Q %1.aps

ren %1.RES %1.tmp
ren %1.tmp %1.res
