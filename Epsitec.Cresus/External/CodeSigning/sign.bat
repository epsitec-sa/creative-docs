@cls
@echo off

echo Signature du fichier "%1"

C:
cd C:\Tools\CodeSigning

signcode -a sha1 -ky signature -spc epsitec.spc -t http://timestamp.verisign.com/scripts/timstamp.dll -v epsitec.pvk "%1"

if %ErrorLevel% EQU 0 goto ok

echo -
echo -
echo -------------------------------
echo -   ECHEC DE LA SIGNATURE !   -
echo -------------------------------
echo -
echo -
pause


ok:
