Ext.require([
  'Epsitec.cresus.webcore.entityList.EntityListPicker',
  'Epsitec.cresus.webcore.entityList.EntityFavoritesPicker',
  'Epsitec.cresus.webcore.field.ReferenceField',
  'Epsitec.cresus.webcore.tools.Callback'
],
function () {
  Ext.define('Epsitec.cresus.webcore.field.EntityReferenceField', {
    extend: 'Epsitec.cresus.webcore.field.ReferenceField',
    alternateClassName: ['Epsitec.EntityReferenceField'],
    alias: 'widget.epsitec.entityreferencefield',

    /* Properties */

    databaseName: null,
    favoritesId: null,
    favoritesOnly: null,

    onPickClick: function () {
      var callback = Epsitec.Callback.create(this.onPickClickCallback, this);

      if (this.favoritesId) {
        Epsitec.EntityFavoritesPicker.showDatabase(
            this.databaseName, this.favoritesId, this.favoritesOnly, false, callback
        );
      } else {
        Epsitec.EntityListPicker.showDatabase(
            this.databaseName, false, callback
        );
      }
    }
  });
});
