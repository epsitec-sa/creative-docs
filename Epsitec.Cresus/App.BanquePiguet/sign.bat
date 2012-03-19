@echo off

set SIGNTOOL=S:\epsitec\_tools\signtool.exe

%SIGNTOOL% sign /a /n Epsitec /i QuoVadis /d "Epsitec" /t http://timestamp.verisign.com/scripts/timstamp.dll "%1"

@pause
