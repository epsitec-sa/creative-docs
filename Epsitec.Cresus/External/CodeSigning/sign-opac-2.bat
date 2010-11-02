@echo on

%~d0
cd %~dp0

echo Signing %1

for %%f in (%1) do S:\Epsitec.Cresus\External\CodeSigning\signtool.exe sign /f S:\Epsitec.Cresus\External\CodeSigning\opac.pfx /p opac /n OPaC /d %2 /t http://timestamp.verisign.com/scripts/timstamp.dll "%%f"
