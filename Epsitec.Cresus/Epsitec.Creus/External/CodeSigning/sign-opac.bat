@echo on

%~d0
cd %~dp0

echo Signing %1

for %%f in (%1) do S:\Epsitec.Cresus\External\CodeSigning\signtool.exe sign /a /n OPaC /i Verisign /d "Creative Docs .NET" /t http://timestamp.verisign.com/scripts/timstamp.dll "%%f"
