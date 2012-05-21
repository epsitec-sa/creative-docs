@echo off

rem ---------------------------------------------------------------
rem --
rem -- Build script pour Cr‚sus Documents
rem -- Copyright ¸ 2008-2009, EPSITEC SA, CH-1400 Yverdon-les-Bains
rem -- Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD
rem --
rem -- Enregistrer avec "Western European DOS" Codepage 850 dans le
rem -- menu File/Advanced Save Options de Visual Studio 2008.
rem --
rem ---------------------------------------------------------------


rem -- Set directory and drive to path of executing batch script

cd /d %~dp0
cd ..

echo Root directory is %CD%


rem -- Find on what kind of system we are running, in order to use
rem -- the correct Visual Studio folder

IF "%ProgramFiles(x86)%"=="" (
  echo Running on a 32-bit System
  set DEVENV="%ProgramFiles%\Microsoft Visual Studio 9.0\Common7\IDE\devenv.exe"
  set IEXPRESS="%ProgramFiles%\BuildInstall\iexpress.exe"
) ELSE (
  echo Running on a 64-bit System
  set DEVENV="%ProgramFiles(x86)%\Microsoft Visual Studio 9.0\Common7\IDE\devenv.exe"
  set IEXPRESS="%ProgramFiles(x86)%\BuildInstall\iexpress.exe"
)


rem -- Extract the text found in the AssemblyInfo file, which specifies the assembly version
rem -- such as : [assembly: AssemblyVersion ("3.1.0832.0")] by first getting the second token
rem -- of the line, using "(" and ")" as separators, and stripping the quotes thanks to %%~a
rem -- Then, tokenize once more by splitting at the "." and create a "3.1.0832" version
rem -- number, which will be used in the output file name.

for /f "tokens=2 delims=()" %%a in ('type Properties\AssemblyInfo.CrDocVer.cs ^| find "AssemblyVersion"') do set version=%%~a
for /f "tokens=1-3 delims=." %%a in ('echo %version%') do set version=%%a.%%b.%%c
for /f "tokens=1-3 delims=." %%a in ('echo %version%') do set versionX=%%a-%%b-00%%c

for /f "tokens=2 delims==" %%a in ('type X.Setup.CresusDocuments\X.Setup.CresusDocuments.vdproj ^| find """ProductVersion"""') do set setupversion=%%~a
for /f "tokens=1-2 delims=:" %%a in ('echo %setupversion%') do set setupversion="%%b

IF %setupversion%=="%version%" (
  echo OK: setup and product versions match.
) ELSE (
  echo Update Setup version! %setupversion% / "%version%"
  pause
  exit
)


set EXE=install-cresus-doc-%versionX%.exe
set DEMOEXE=install-cresus-doc-%versionX%-demo.exe

set EXEPATH="%CD%\X.Setup.CresusDocuments\%EXE%"
set BUILD=Release
set SIGNTOOL="%CD%\External\CodeSigning\signtool.exe"

echo Building version %version% of Cr‚sus Documents (%BUILD%)
%DEVENV% "%CD%\Epsitec.Cresus.sln" /Build "%BUILD%" /Project "App.CresusDocuments"

echo Building version %version% of Cr‚sus Documents installer (%BUILD%)
%DEVENV% "%CD%\Epsitec.Cresus.sln" /Build "%BUILD%" /Project "X.Setup.CresusDocuments"

del "%CD%\X.Setup.CresusDocuments\Sleep.exe" 2>NUL
del %EXEPATH% 2>NUL
del "%CD%\X.Setup.CresusDocuments\CrDoc-Installer.exe" 2>NUL

copy "%CD%\External\Sleep.exe" "%CD%\X.Setup.CresusDocuments\Sleep.exe"
copy "%CD%\X.Setup.CresusDocuments\%BUILD%\Setup.exe" "%CD%\X.Setup.CresusDocuments\Setup.exe"
copy "%CD%\X.Setup.CresusDocuments\%BUILD%\CresusDocuments.msi" "%CD%\X.Setup.CresusDocuments\CresusDocuments.msi"


%SIGNTOOL% sign /a /n Epsitec /i QuoVadis /d "Installateur Cr‚sus Documents" /t http://timestamp.verisign.com/scripts/timstamp.dll "%CD%\X.Setup.CresusDocuments\Sleep.exe"
%SIGNTOOL% sign /a /n Epsitec /i QuoVadis /d "Installateur Cr‚sus Documents" /t http://timestamp.verisign.com/scripts/timstamp.dll "%CD%\X.Setup.CresusDocuments\Setup.exe"
%SIGNTOOL% sign /a /n Epsitec /i QuoVadis /d "Installateur Cr‚sus Documents" /t http://timestamp.verisign.com/scripts/timstamp.dll "%CD%\X.Setup.CresusDocuments\CresusDocuments.msi"

echo Packaging installer into %EXE%

cd X.Setup.CresusDocuments

%IEXPRESS% /N Installer.sed

copy CrDoc-Installer.exe %DEMOEXE%
rename CrDoc-Installer.exe %EXE%

cd ..

%SIGNTOOL% sign /a /n Epsitec /i QuoVadis /d "Installateur Cr‚sus Documents" /t http://timestamp.verisign.com/scripts/timstamp.dll %EXEPATH%

del "%CD%\X.Setup.CresusDocuments\Sleep.exe" 2>NUL
del "%CD%\X.Setup.CresusDocuments\Setup.exe" 2>NUL
del "%CD%\X.Setup.CresusDocuments\CresusDocuments.msi" 2>NUL
