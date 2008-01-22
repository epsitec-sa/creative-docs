del Sleep.exe 2>NUL

copy "S:\Epsitec.Cresus\External\Sleep.exe" Sleep.exe


S:\Epsitec.Cresus\External\CodeSigning\signtool.exe sign /a /n OPaC /d "OPaC bright ideas" /t http://timestamp.verisign.com/scripts/timstamp.dll "Sleep.exe"

S:\Epsitec.Cresus\External\CodeSigning\signtool.exe sign /a /n OPaC /d "OPaC bright ideas" /t http://timestamp.verisign.com/scripts/timstamp.dll "Debug\Setup.exe"
S:\Epsitec.Cresus\External\CodeSigning\signtool.exe sign /a /n OPaC /d "OPaC bright ideas" /t http://timestamp.verisign.com/scripts/timstamp.dll "Debug\CreativeDocs.msi"

iexpress /N Installer.sed

S:\Epsitec.Cresus\External\CodeSigning\signtool.exe sign /a /n OPaC /d "OPaC bright ideas" /t http://timestamp.verisign.com/scripts/timstamp.dll "CrDoc-Installer.exe"

del Sleep.exe 2>NUL
