Ext.require([
  'Epsitec.cresus.webcore.entityList.EntityListPicker',
  'Epsitec.cresus.webcore.field.ReferenceField',
  'Epsitec.cresus.webcore.tools.Callback'
],
function() {
  Ext.define('Epsitec.cresus.webcore.field.EntityReferenceField', {
    extend: 'Epsitec.cresus.webcore.field.ReferenceField',
    alternateClassName: ['Epsitec.EntityReferenceField'],
    alias: 'widget.epsitec.entityreferencefield',

    /* Properties */

    databaseName: null,

    onPickClick: function() {
      var callback = Epsitec.Callback.create(this.onPickClickCallback, this);
      Epsitec.EntityPicker.showDatabase(this.databaseName, false, callback);
    }
  });
});
