call "C:\Program Files (x86)\Microsoft Visual Studio\2019\Enterprise\VC\Auxiliary\Build\vcvars32.bat"

call ".\build.bat" -production
call ".\build.bat" -test

pause
