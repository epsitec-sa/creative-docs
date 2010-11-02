REM --------------------------------------------------------------------------------------------
REM
REM Based on the 'exported-opac.pfx', directly exported from the certificate store, produce
REM the private key file (PVK) :

S:\Epsitec.Cresus\External\CodeSigning\openssl\openssl.exe pkcs12 -in opac-2013.pfx -nocerts -nodes -out opac-2013.pem
S:\Epsitec.Cresus\External\CodeSigning\pvk.exe -in opac-2013.pem -topvk -strong -out opac-2013.pvk
del opac-2013.pem

REM --------------------------------------------------------------------------------------------
REM
REM Now, also export the Software Publishing Certificate (SPC) :

S:\Epsitec.Cresus\External\CodeSigning\openssl\openssl.exe pkcs12 -in opac-2013.pfx -nokeys -out opac-2013.pem 
S:\Epsitec.Cresus\External\CodeSigning\openssl\openssl.exe crl2pkcs7 -nocrl -certfile opac-2013.pem -outform DER -out opac-2013.spc
del opac-2013.pem

REM --------------------------------------------------------------------------------------------
REM
REM And finally, rebuild a PFX file with a simple output password (opac), since SignTool won't
REM be happy to work on a PFX file which has no such password specified

S:\epsitec.cresus\External\CodeSigning\pvk2pfx.exe -pvk opac-2013.pvk -spc opac-2013.spc -pfx opac.pfx -po opac 

REM --------------------------------------------------------------------------------------------
REM
REM To sign, use the following command line :
REM
REM S:\Epsitec.Cresus\External\CodeSigning\signtool.exe sign /f opac-opac.pfx /p opac <<executable>> 
