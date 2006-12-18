echo Post-build MSI patching and signing
echo Target: %1
echo Path: %2

cd %2

SDK\msitran -a opac-Cleanup.mst %1
SDK\msitran -a opac-MsiRMFilesInUseDialog.mst %1

call CodeSigning\sign-opac.bat %1

