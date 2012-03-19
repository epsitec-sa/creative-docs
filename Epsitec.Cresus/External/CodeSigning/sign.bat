@echo off

%~d0
cd %~dp0

echo Signing:
echo File path is %1
echo File id is %2

for %%f in (%1) do S:\Epsitec.Cresus\External\CodeSigning\signtool.exe sign /a /n Epsitec /i QuoVadis /d %2 /t http://timestamp.verisign.com/scripts/timstamp.dll "%%f"

if not ErrorLevel 1 goto ok

echo Error: Authenticode Signature failure !
pause
exit


:ok

echo OK: Authenticode Signature Applied successfully

