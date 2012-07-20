@ECHO ==================== gjslint ====================
@gjslint --nojsdoc --strict WebCore\app.js
@gjslint --nojsdoc --strict -r WebCore\js\Static\

@ECHO ==================== jshint ====================
@CALL jshint.bat /bitwise:true /curly:true /eqeqeq:true /forin:true /noarg:true /noempty:true /nonew:true /trailing:true /immed:true /latedef:true /newcap:true /regexp:true /undef:true /onecase:true /plusplus:true /global:document:true,window:true,Ext:true,Epsitec:true WebCore\app.js
@CALL jshint.bat /bitwise:true /curly:true /eqeqeq:true /forin:true /noarg:true /noempty:true /nonew:true /trailing:true /immed:true /latedef:true /newcap:true /regexp:true /undef:true /onecase:true /plusplus:true /global:document:true,window:true,Ext:true,Epsitec:true WebCore\js\Static\