@echo on

%~d0
cd %~dp0

echo Signing %1

for %%f in (%1) do S:\Epsitec.Cresus\External\CodeSigning\signcode -a sha1 -ky signature -spc S:\Epsitec.Cresus\External\CodeSigning\opac.spc -t http://timestamp.verisign.com/scripts/timstamp.dll -v S:\Epsitec.Cresus\External\CodeSigning\opac.pvk "%%f"
