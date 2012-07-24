@REM The --nomultiprocess flag is rally important here, as it gjslint 2.3.6 will
@REM spawn gazillions of python.exe processes until your PC requires a shutdown
@REM without it.

@ECHO ==================== gjslint ====================
@gjslint --nojsdoc --strict --nomultiprocess %~dp0\WebCore\app.js
@gjslint --nojsdoc --strict --nomultiprocess -r %~dp0\WebCore\js\

@ECHO ==================== jshint ====================
@CALL jshint.bat /bitwise:true /curly:true /eqeqeq:true /forin:true /noarg:true /noempty:true /nonew:true /trailing:true /immed:true /latedef:true /newcap:true /regexp:true /undef:true /onecase:true /plusplus:true /global:document:true,window:true,Ext:true,Epsitec:true %~dp0\WebCore\app.js
@CALL jshint.bat /bitwise:true /curly:true /eqeqeq:true /forin:true /noarg:true /noempty:true /nonew:true /trailing:true /immed:true /latedef:true /newcap:true /regexp:true /undef:true /onecase:true /plusplus:true /global:document:true,window:true,Ext:true,Epsitec:true %~dp0\WebCore\js\