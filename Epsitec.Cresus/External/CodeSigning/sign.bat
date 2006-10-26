@echo off

echo Signing %1

S:
cd S:\Epsitec.Cresus\External\CodeSigning

signcode -a sha1 -ky signature -spc epsitec.spc -t http://timestamp.verisign.com/scripts/timstamp.dll -v epsitec.pvk %1

if not ErrorLevel 1 goto ok

echo Error: Authenticode Signature failure !
pause
exit


:ok

echo OK: Authenticode Signature Applied successfully
