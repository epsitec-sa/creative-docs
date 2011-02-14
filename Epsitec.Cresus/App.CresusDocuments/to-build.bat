@echo off

pushd %CD%

xcopy "bin\Debug .NET 2.0\*.exe" S:\build\CrDoc\ProgramFiles /Y
xcopy "bin\Debug .NET 2.0\*.dll" S:\build\CrDoc\ProgramFiles /Y
xcopy "..\External\*.Win32.dll" S:\build\CrDoc\ProgramFiles /Y

xcopy "Resources\Common.Dialogs\*.??.resource" S:\build\CrDoc\ProgramFiles\Resources\Common.Dialogs /Y
xcopy "Resources\Common.Dialogs\module.info"   S:\build\CrDoc\ProgramFiles\Resources\Common.Dialogs /Y
xcopy "Resources\Common.Document\*.??.resource" S:\build\CrDoc\ProgramFiles\Resources\Common.Document /Y
xcopy "Resources\Common.Document\module.info"   S:\build\CrDoc\ProgramFiles\Resources\Common.Document /Y
xcopy "Resources\Common.DocumentEditor\*.??.resource" S:\build\CrDoc\ProgramFiles\Resources\Common.DocumentEditor /Y
xcopy "Resources\Common.DocumentEdirot\module.info"   S:\build\CrDoc\ProgramFiles\Resources\Common.DocumentEditor /Y
xcopy "Resources\Common.Support\*.??.resource" S:\build\CrDoc\ProgramFiles\Resources\Common.Support /Y
xcopy "Resources\Common.Support\module.info"   S:\build\CrDoc\ProgramFiles\Resources\Common.Support /Y
xcopy "Resources\Common.Types\*.??.resource" S:\build\CrDoc\ProgramFiles\Resources\Common.Types /Y
xcopy "Resources\Common.Types\module.info"   S:\build\CrDoc\ProgramFiles\Resources\Common.Types /Y
xcopy "Resources\Common.Widgets\*.??.resource" S:\build\CrDoc\ProgramFiles\Resources\Common.Widgets /Y
xcopy "Resources\Common.Widgets\module.info"   S:\build\CrDoc\ProgramFiles\Resources\Common.Widgets /Y

xcopy "Exemples originaux\*.crdoc" "S:\build\CrDoc\ProgramFiles\Exemples originaux" /Y

call "S:\Epsitec.Cresus\External\CodeSigning\sign-epsitec.bat" "S:\build\CrDoc\ProgramFiles\*.exe" "Cr‚sus Documents"
call "S:\Epsitec.Cresus\External\CodeSigning\sign-epsitec.bat" "S:\build\CrDoc\ProgramFiles\*.dll" "Cr‚sus Documents"

popd
