@echo off

%~d0
cd %~dp0

echo Signing %1

for %%f in (%1) do S:\Epsitec.Cresus\External\CodeSigning\signtool.exe sign /a /n Epsitec /d "Epsitec SA" /t http://timestamp.verisign.com/scripts/timstamp.dll "%%f"

if not ErrorLevel 1 goto ok

echo Error: Authenticode Signature failure !
pause
exit


:ok

echo OK: Authenticode Signature Applied successfully

