del Sleep.exe 2>NUL

copy "S:\Epsitec.Cresus\External\Sleep.exe" Sleep.exe


S:\Epsitec.Cresus\External\CodeSigning\signtool.exe sign /a /n Epsitec /d "EPSITEC SA" /t http://timestamp.verisign.com/scripts/timstamp.dll "Sleep.exe"

S:\Epsitec.Cresus\External\CodeSigning\signtool.exe sign /a /n Epsitec /d "EPSITEC SA" /t http://timestamp.verisign.com/scripts/timstamp.dll "Debug\Setup.exe"
S:\Epsitec.Cresus\External\CodeSigning\signtool.exe sign /a /n Epsitec /d "EPSITEC SA" /t http://timestamp.verisign.com/scripts/timstamp.dll "Debug\Dolphin.msi"

S:\Epsitec.Cresus\External\iexpress /N Installer.sed

S:\Epsitec.Cresus\External\CodeSigning\signtool.exe sign /a /n Epsitec /d "EPSITEC SA" /t http://timestamp.verisign.com/scripts/timstamp.dll "Dolphin-Installer.exe"

del Sleep.exe 2>NUL
