// This class represents an entity list that is backed by a favorites collection
// on the server.

Ext.require([
  'Epsitec.cresus.webcore.entityList.EntityList'
],
function() {
  Ext.define('Epsitec.cresus.webcore.entityList.FavoritesEntityList', {
    extend: 'Epsitec.cresus.webcore.entityList.EntityList',
    alternateClassName: ['Epsitec.FavoritesEntityList'],

    /* Constructor */

    constructor: function(options) {
      var newOptions = {
        getUrl: 'proxy/favorites/get/' + options.favoritesId,
        exportUrl: 'proxy/favorites/export/' + options.favoritesId
      };
      Ext.applyIf(newOptions, options);

      this.callParent([newOptions]);
      return this;
    }
  });
});
