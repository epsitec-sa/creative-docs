@rem Set the current working directory to the directory of this batch file so we can launch it from
@rem anywhere.
cd /d %~dp0

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

@rem Clean all the solutions that will be built by this script.
msbuild /property:Configuration=Debug;Platform="ANY CPU" /target:Clean ..\..\Epsitec\dot.net\Epsitec.ZipMe\Epsitec.ZipMe.sln
if %ERRORLEVEL% neq 0 exit /B 1
msbuild /property:Configuration=Release;Platform="ANY CPU" /target:Clean ..\..\Epsitec\dot.net\Epsitec.ZipMe\Epsitec.ZipMe.sln
if %ERRORLEVEL% neq 0 exit /B 1
msbuild /property:Configuration=Debug;Platform="ANY CPU" /target:Clean ..\..\Epsitec\dot.net\Epsitec.ShellPE\Epsitec.ShellPE.sln
if %ERRORLEVEL% neq 0 exit /B 1
msbuild /property:Configuration=Release;Platform="ANY CPU" /target:Clean ..\..\Epsitec\dot.net\Epsitec.ShellPE\Epsitec.ShellPE.sln
if %ERRORLEVEL% neq 0 exit /B 1
msbuild /property:Configuration=Debug;Platform="Mixed Platforms" /target:Clean ..\..\Epsitec\dot.net\Epsitec.SwissPost\Epsitec.SwissPost.Webservices.sln
if %ERRORLEVEL% neq 0 exit /B 1
msbuild /property:Configuration=Release;Platform="Mixed Platforms" /target:Clean ..\..\Epsitec\dot.net\Epsitec.SwissPost\Epsitec.SwissPost.Webservices.sln
if %ERRORLEVEL% neq 0 exit /B 1
msbuild /property:Configuration=Debug;Platform="Mixed Platforms" /target:Clean ..\Epsitec.Cresus.sln
if %ERRORLEVEL% neq 0 exit /B 1
msbuild /property:Configuration=Debug;Platform="Mixed Platforms" /target:Clean ..\Epsitec.Cresus.sln
if %ERRORLEVEL% neq 0 exit /B 1

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

@rem Copy the client and server directories to the output folder.
rmdir /s /q bin\Build
mkdir bin\Build\aider
mkdir bin\Build\aider\server
mkdir bin\Build\aider\client
xcopy /e bin\Release bin\Build\aider\server
xcopy /e ..\Cresus.WebCore.Client\WebCore bin\Build\aider\client

@rem Replace the debug config files with the production ones.
copy Production\app.config bin\Build\aider\server\App.Aider.exe.config
copy Production\nginx.conf bin\Build\aider\server\Nginx\conf\nginx.conf

@rem Zip the build.
..\..\Epsitec\dot.net\Epsitec.ZipMe\Epsitec.ZipMe\bin\Release\Epsitec.ZipMe.exe bin\Build\aider.zip bin\Build\aider