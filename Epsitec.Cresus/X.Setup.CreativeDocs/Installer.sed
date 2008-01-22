[Version]
Class=IEXPRESS
SEDVersion=3

[Options]
PackagePurpose=InstallApp
ShowInstallProgramWindow=0
HideExtractAnimation=0
UseLongFileName=1
InsideCompressed=0
CAB_FixedSize=0
CAB_ResvCodeSigning=0
RebootMode=N
InstallPrompt=%InstallPrompt%
DisplayLicense=%DisplayLicense%
FinishMessage=%FinishMessage%
TargetName=%TargetName%
FriendlyName=%FriendlyName%
AppLaunched=%AppLaunched%
PostInstallCmd=%PostInstallCmd%
AdminQuietInstCmd=%AdminQuietInstCmd%
UserQuietInstCmd=%UserQuietInstCmd%
SourceFiles=SourceFiles

[Strings]
InstallPrompt=
DisplayLicense=
FinishMessage=Wait until end of install, then press OK.
TargetName=CrDoc-Installer.exe
FriendlyName=Creative Docs .NET Installer
AppLaunched=setup.exe
PostInstallCmd=Sleep.exe
AdminQuietInstCmd=
UserQuietInstCmd=
FILE0="setup.exe"
FILE1="CreativeDocs.msi"
FILE2="Sleep.exe"


[SourceFiles]
SourceFiles0=Debug\
SourceFiles1=.\
[SourceFiles0]
%FILE0%=
%FILE1%=
[SourceFiles1]
%FILE2%=
