@echo off

rem ---------------------------------------------------------------
rem --
rem -- Build script for Creative Docs .NET
rem -- Copyright © 2008, EPSITEC SA, CH-1400 Yverdon-les-Bains
rem -- Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD
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
  set DEVENV="%ProgramFiles%\Microsoft Visual Studio 10.0\Common7\IDE\devenv.exe"
) ELSE (
  echo Running on a 64-bit System
  set DEVENV="%ProgramFiles(x86)%\Microsoft Visual Studio 10.0\Common7\IDE\devenv.exe"
)


rem -- Extract the text found in the AssemblyInfo file, which specifies the assembly version
rem -- such as : [assembly: AssemblyVersion ("3.1.0832.0")] by first getting the second token
rem -- of the line, using "(" and ")" as separators, and stripping the quotes thanks to %%~a
rem -- Then, tokenize once more by splitting at the "." and create a "3.1.0832" version
rem -- number, which will be used in the output file name.

for /f "tokens=2 delims=()" %%a in ('type "Solution Items\AssemblyInfo.CrDocVer.cs" ^| find "AssemblyVersion"') do set version=%%~a
for /f "tokens=1-3 delims=." %%a in ('echo %version%') do set version=%%a.%%b.%%c

for /f "tokens=2 delims==" %%a in ('type Setup.CreativeDocs\Setup.CreativeDocs.vdproj ^| find """ProductVersion"""') do set setupversion=%%~a
for /f "tokens=1-2 delims=:" %%a in ('echo %setupversion%') do set setupversion="%%b

IF %setupversion%=="%version%" (
  echo OK: setup and product versions match.
) ELSE (
  echo Update Setup version! %setupversion% / "%version%"
  pause
  exit
)


set EXE=CrDoc-%version%-installer.exe
set EXEPATH="%CD%\Setup.CreativeDocs\%EXE%"
set BUILD=Debug
set IEXPRESS="%CD%\External\iexpress.exe"
set SIGNTOOL="%CD%\External\CodeSigning\signtool.exe"

echo Building version %version% of Creative Docs .NET (%BUILD%)
%DEVENV% "%CD%\Epsitec.Cresus.sln" /Build "%BUILD%" /Project "App.CreativeDocs"

echo Building version %version% of Creative Docs .NET installer (%BUILD%)
%DEVENV% "%CD%\Epsitec.Cresus.sln" /Build "%BUILD%" /Project "Setup.CreativeDocs"

del "%CD%\Setup.CreativeDocs\Sleep.exe" 2>NUL
del %EXEPATH% 2>NUL
del "%CD%\Setup.CreativeDocs\CrDoc-2.x.x-installer.exe" 2>NUL

copy "%CD%\External\Sleep.exe" "%CD%\Setup.CreativeDocs\Sleep.exe"
copy "%CD%\Setup.CreativeDocs\%BUILD%\Setup.exe" "%CD%\Setup.CreativeDocs\Setup.exe"
copy "%CD%\Setup.CreativeDocs\%BUILD%\CreativeDocs.msi" "%CD%\Setup.CreativeDocs\CreativeDocs.msi"

%SIGNTOOL% sign /a /n OPaC /i Verisign /d "Creative Docs .NET Installer" /t http://timestamp.verisign.com/scripts/timstamp.dll "%CD%\Setup.CreativeDocs\Sleep.exe"
%SIGNTOOL% sign /a /n OPaC /i Verisign /d "Creative Docs .NET Installer" /t http://timestamp.verisign.com/scripts/timstamp.dll "%CD%\Setup.CreativeDocs\Setup.exe"
%SIGNTOOL% sign /a /n OPaC /i Verisign /d "Creative Docs .NET Installer" /t http://timestamp.verisign.com/scripts/timstamp.dll "%CD%\Setup.CreativeDocs\CreativeDocs.msi"

echo Packaging installer into %EXE%

cd Setup.CreativeDocs

"%IEXPRESS%" /N Installer.sed
rename CrDoc-2.x.x-installer.exe %EXE%

cd ..

%SIGNTOOL% sign /a /n OPaC /i Verisign /d "Creative Docs .NET Installer" /t http://timestamp.verisign.com/scripts/timstamp.dll %EXEPATH%

del "%CD%\Setup.CreativeDocs\Sleep.exe" 2>NUL
del "%CD%\Setup.CreativeDocs\Setup.exe" 2>NUL
del "%CD%\Setup.CreativeDocs\CreativeDocs.msi" 2>NUL
