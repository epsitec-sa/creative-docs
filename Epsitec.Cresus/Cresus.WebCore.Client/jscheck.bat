@ECHO ==================== gjslint ====================
@gjslint --nojsdoc --strict %~dp0\WebCore\app.js
@gjslint --nojsdoc --strict -r %~dp0\WebCore\js\

@ECHO ==================== jshint ====================
@CALL jshint.bat /bitwise:true /curly:true /eqeqeq:true /forin:true /noarg:true /noempty:true /nonew:true /trailing:true /immed:true /latedef:true /newcap:true /regexp:true /undef:true /onecase:true /plusplus:true /global:document:true,window:true,Ext:true,Epsitec:true %~dp0\WebCore\app.js
@CALL jshint.bat /bitwise:true /curly:true /eqeqeq:true /forin:true /noarg:true /noempty:true /nonew:true /trailing:true /immed:true /latedef:true /newcap:true /regexp:true /undef:true /onecase:true /plusplus:true /global:document:true,window:true,Ext:true,Epsitec:true %~dp0\WebCore\js\
