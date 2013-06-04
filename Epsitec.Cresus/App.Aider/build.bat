@echo off

@rem Set the current working directory to the directory of this batch file so we can launch it from
@rem anywhere.
cd /d %~dp0

@rem Here we store whether we must cleanup or not
set cleanup=1

@rem Here we set the configuration directory where we want to pick the configuration for the build.
set configurationDirectory=0

@rem Process the command line arguments.
for %%a in (%*) do (
    if "%%a"=="-nocleanup" set cleanup=0
    if "%%a"=="-production" set configurationDirectory=ServerConfigProduction
    if "%%a"=="-test" set configurationDirectory=ServerConfigTest
)

@rem Exits if the configuration directory is not set up properly
if %configurationDirectory%==0 (
    echo The configuration is not set up properly. Use -production or -test.
    exit /B
)

@rem Cleans up if we must do so
if %cleanup%==1 (
    @rem Ask the user if is really wants to revert everything in its svn repositories
    echo This script will:
    echo - Revert all changes to %~dp0..\..\Epsitec
    echo - Delete all unversionned items in %~dp0..\..\Epsitec
    echo - Delete all ignored items in %~dp0..\..\Epsitec
    echo - Revert all changes to %~dp0..\
    echo - Delete all unversionned items in %~dp0..\
    echo - Delete all ignored items in %~dp0..\
    choice /M "Are you sure that you want to continue"
    if ERRORLEVEL 2 exit /B

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
)

@echo on

@rem Build the solution.
msbuild /verbosity:minimal /property:Configuration=Release;Platform="x86" /target:Build ..\Epsitec.Cresus.sln
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

@rem Copy the app.config file.
copy %configurationDirectory%\app.config bin\Build\aider\server\App.Aider.exe.config

@rem Copy the nginx config files.
copy %configurationDirectory%\nginx-maintenance.conf bin\Build\aider\maintenance\Nginx\conf\nginx.conf
copy %configurationDirectory%\nginx-server.conf bin\Build\aider\server\Nginx\conf\nginx.conf

@rem Copy the certificate and key
mkdir bin\Build\aider\server\certificate
copy %configurationDirectory%\certificate.pem bin\Build\aider\server\certificate\certificate.pem
copy %configurationDirectory%\certificate.key bin\Build\aider\server\certificate\certificate.key

@rem copy the custom .crconfig files
copy %configurationDirectory%\Aider.Environment.crconfig bin\Build\aider\server\Aider.Environment.crconfig
copy %configurationDirectory%\Aider.Features.crconfig bin\Build\aider\server\Aider.Features.crconfig

@rem Zip the build.
..\..\Epsitec\dot.net\Epsitec.ZipMe\Epsitec.ZipMe\bin\Release\Epsitec.ZipMe.exe bin\Build\aider.zip bin\Build\aider