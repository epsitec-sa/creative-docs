@echo off

@rem Set the current working directory to the directory of this batch file so we can launch it from
@rem anywhere.
cd /d %~dp0

@rem Here we store whether we must cleanup or not
set cleanup=0

@rem Here we set the configuration directory where we want to pick the configuration for the build.
set configurationDirectory=0

@rem Process the command line arguments.
for %%a in (%*) do (
    if "%%a"=="-cleanup" set cleanup=1
    if "%%a"=="-production" set configurationDirectory=ServerConfigProduction
    if "%%a"=="-production" set suffix=prod
    if "%%a"=="-test" set configurationDirectory=ServerConfigTest
    if "%%a"=="-test" set suffix=test
)

@rem Exits if the configuration directory is not set up properly
if %configurationDirectory%==0 (
    echo The configuration is not set up properly. Use -production or -test.
	echo To clean up everything, additionally specify -cleanup.
    exit /B
)

@rem Cleans up if we must do so
if %cleanup%==1 (
    @rem Ask the user if is really wants to revert everything in its git repositories
    echo This script will:
    echo - Clean submodule %~dp0..\..\Epsitec.Serial
    echo - Clean submodule %~dp0..\..\Epsitec.ShellPE
    echo - Clean submodule %~dp0..\..\Epsitec.SwissPost
    echo - Clean submodule %~dp0..\..\Epsitec.TwixClip
    echo - Clean %~dp0..\
    choice /M "Are you sure that you want to continue"
    if ERRORLEVEL 2 exit /B

    @echo on

    @rem Clean all repositories.
    git clean -xdf ..\..\Epsitec.Serial
    git clean -xdf ..\..\Epsitec.ShellPE
    git clean -xdf ..\..\Epsitec.SwissPost
    git clean -xdf ..\..\Epsitec.TwixClip
    git clean -xdf ..\
)

@echo on


@rem Build the solution.
msbuild /verbosity:minimal /property:Configuration=Release /target:Build ..\Epsitec.Cresus.2013.sln
if %ERRORLEVEL% neq 0 exit /B 1

@rem Copy the client, maintenance and server directories to the output folder.
rmdir /s /q bin\Build
mkdir bin\Build\aider
mkdir bin\Build\aider\client
mkdir bin\Build\aider\maintenance
mkdir bin\Build\aider\server
mkdir bin\Build\aider\assets

xcopy /e ..\Cresus.WebCore.Client\WebCore bin\Build\aider\client
xcopy /e ..\Cresus.WebCore.Maintenance bin\Build\aider\maintenance
xcopy /e bin\Release bin\Build\aider\server
xcopy /e assets bin\Build\aider\assets

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
set zipname=aider-%suffix%
..\..\Epsitec.ShellPE\Epsitec.ZipMe\Epsitec.ZipMe\bin\Release\Epsitec.ZipMe.exe bin\Build\%zipname%.zip bin\Build\aider

copy bin\Build\%zipname%.zip S:\ /Y
