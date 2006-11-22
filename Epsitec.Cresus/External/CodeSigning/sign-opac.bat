@echo off

echo Signing %1

S:
cd S:\Epsitec.Cresus\External\CodeSigning

signcode -a sha1 -ky signature -spc opac.spc -t http://timestamp.verisign.com/scripts/timstamp.dll -v opac.pvk %1

if not ErrorLevel 1 goto ok

echo Error: Authenticode Signature failure !
pause
exit


:ok

echo OK: Authenticode Signature Applied successfully
