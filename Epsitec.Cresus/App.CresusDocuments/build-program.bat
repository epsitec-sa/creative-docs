@echo off

rem ---------------------------------------------------------------
rem --
rem -- Build script for Creative Docs .NET
rem -- Copyright © 2011, EPSITEC SA, CH-1400 Yverdon-les-Bains
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


set BUILD=Debug .NET 2.0

echo Building version %version% of Cresus Documents (%BUILD%)
%DEVENV% "%CD%\Epsitec.Cresus.sln" /Build "%BUILD%" /Project "App.CresusDocuments"

