@echo off

@rem Set the current working directory to the directory of this batch file so we can launch it from
@rem anywhere.
cd /d %~dp0

@rem If the first argument is -nocleanup, skip the svn cleanup
if "%1"=="-nocleanup" goto skipCleanup

@rem Ask the user if is really wants to revert everything in its svn repositories
echo This script will:
echo - Revert all changes to %~dp0..\..\Epsitec
echo - Delete all unversionned items in %~dp0..\..\Epsitec
echo - Delete all ignored items in %~dp0..\..\Epsitec
echo - Revert all changes to %~dp0..\
echo - Delete all unversionned items in %~dp0..\
echo - Delete all ignored items in %~dp0..\
choice /M "Are you sure that you want to continue"
if %ERRORLEVEL% neq 1 exit /B

@echo on

@rem Revert all changes to the two repositories.
svn revert -R ..\..\Epsitec
svn revert -R ..\

@rem Delete all unversionned files and folders in the two repositories.
for /f "usebackq tokens=2*" %%i in (`svn status ..\..\Epsitec ^| findstr /r "^\?"`) do svn delete --force "%%i %%j"
for /f "usebackq tokens=2*" %%i in (`svn status ..\ ^| findstr /r "^\?"`) do svn delete --force "%%i %%j"

@rem Delete all ignored files and folders in the two repositories
for /f "usebackq tokens=2*" %%i in (`svn status --no-ignore ..\..\Epsitec ^| findstr /r "^I"`) do svn delete --force "%%i %%j"
for /f "usebackq tokens=2*" %%i in (`svn status --no-ignore ..\ ^| findstr /r "^I"`) do svn delete --force "%%i %%j"

:skipCleanup

@rem There are many solutions and configurations involved here. Below is the details about them.
@rem - Epsitec.Cresus.sln, Release, Mixed Platforms
@rem   That's the solution and the configuration that we want to build.
@rem - Epsitec.Cresus.sln, Debug, Mixed Platforms
@rem   We need the main solution built in debug mode because there is a link to a dll compiled in
@rem   debug mode in the project App.CresusCore.
@rem - Epsitec.ShellPE.sln, Release, ANY CPU
@rem   This solution is referenced by Epsitec.Cresus.sln, Release, Mixed Platforms
@rem - Epsitec.ShellPE.sln, Debug, ANY CPU
@rem   This solution is referenced by Epsitec.Cresus.sln, Debug, Mixed Platforms
@rem - Epsitec.ZipMe.sln, Release, ANY CPU
@rem   This solution is referenced by Epsitec.ShellPE.sln, Release, ANY CPU
@rem - Epsitec.ZipMe.sln, Debug, ANY CPU
@rem   This solution is referenced by Epsitec.ShellPE.sln, Debug, ANY CPU
@rem - Epsitec.SwissPost.Webservices.sln, Release, ANY CPU
@rem   This solution is referenced by Epsitec.Cresus.sln, Release, Mixed Platforms
@rem - Epsitec.SwissPost.Webservices.sln, Debug, ANY CPU
@rem   This solution is referenced by Epsitec.Cresus.sln, Debug, Mixed Platforms

@rem Build the solutions.
msbuild /verbosity:minimal /property:Configuration=Debug;Platform="ANY CPU" /target:Build ..\..\Epsitec\dot.net\Epsitec.ZipMe\Epsitec.ZipMe.sln
if %ERRORLEVEL% neq 0 exit /B 1
msbuild /verbosity:minimal /property:Configuration=Release;Platform="ANY CPU" /target:Build ..\..\Epsitec\dot.net\Epsitec.ZipMe\Epsitec.ZipMe.sln
if %ERRORLEVEL% neq 0 exit /B 1
msbuild /verbosity:minimal /property:Configuration=Debug;Platform="ANY CPU" /target:Build ..\..\Epsitec\dot.net\Epsitec.ShellPE\Epsitec.ShellPE.sln
if %ERRORLEVEL% neq 0 exit /B 1
msbuild /verbosity:minimal /property:Configuration=Release;Platform="ANY CPU" /target:Build ..\..\Epsitec\dot.net\Epsitec.ShellPE\Epsitec.ShellPE.sln
if %ERRORLEVEL% neq 0 exit /B 1
msbuild /verbosity:minimal /property:Configuration=Debug;Platform="Mixed Platforms" /target:Build ..\..\Epsitec\dot.net\Epsitec.SwissPost\Epsitec.SwissPost.Webservices.sln
if %ERRORLEVEL% neq 0 exit /B 1
msbuild /verbosity:minimal /property:Configuration=Release;Platform="Mixed Platforms" /target:Build ..\..\Epsitec\dot.net\Epsitec.SwissPost\Epsitec.SwissPost.Webservices.sln
if %ERRORLEVEL% neq 0 exit /B 1
msbuild /verbosity:minimal /property:Configuration=Debug;Platform="Mixed Platforms" /target:Build ..\Epsitec.Cresus.sln
if %ERRORLEVEL% neq 0 exit /B 1
msbuild /verbosity:minimal /property:Configuration=Release;Platform="Mixed Platforms" /target:Build ..\Epsitec.Cresus.sln
if %ERRORLEVEL% neq 0 exit /B 1

@rem Copy the client, maintenance and server directories to the output folder.
rmdir /s /q bin\Build
mkdir bin\Build\aider
mkdir bin\Build\aider\client
mkdir bin\Build\aider\maintenance
mkdir bin\Build\aider\server
xcopy /e ..\Cresus.WebCore.Client\WebCore bin\Build\aider\client
xcopy /e ..\Cresus.WebCore.Maintenance bin\Build\aider\maintenance
xcopy /e bin\Release bin\Build\aider\server

@rem Copy the production config file.
copy Production\app.config bin\Build\aider\server\App.Aider.exe.config

@rem Copy the production nginx config files.
copy Production\nginx-maintenance.conf bin\Build\aider\maintenance\Nginx\conf\nginx.conf
copy Production\nginx-server.conf bin\Build\aider\server\Nginx\conf\nginx.conf

@rem Copy the production certificate and keys
mkdir bin\Build\aider\server\certificate
copy Production\certificate.crt bin\Build\aider\server\certificate\certificate.crt
copy Production\certificate.key bin\Build\aider\server\certificate\certificate.key

@rem Zip the build.
..\..\Epsitec\dot.net\Epsitec.ZipMe\Epsitec.ZipMe\bin\Release\Epsitec.ZipMe.exe bin\Build\aider.zip bin\Build\aider