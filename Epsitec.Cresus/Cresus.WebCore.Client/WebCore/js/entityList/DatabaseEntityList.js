// This class represents two kinds of entity lists:
// - entity lists that are backed by a regular database on the server.
// - entity lists that are backed by a favourite list on the server.
// It really only should by used for the database entity lists (as it is said by
// its name) but the support for favourites entity lists has been quickly added
// in a hacky way. Ideally, we should have another class that would be called
// FavouritesEntityList that would deal with them.

Ext.require([
  'Epsitec.cresus.webcore.entityList.EntityList'
],
function() {
  Ext.define('Epsitec.cresus.webcore.entityList.DatabaseEntityList', {
    extend: 'Epsitec.cresus.webcore.entityList.EntityList',
    alternateClassName: ['Epsitec.DatabaseEntityList'],

    /* Constructor */

    constructor: function(options) {
      var newOptions;
      if (options.favoritesId) {
        newOptions = {
          getUrl: 'proxy/favorites/get/' + options.favoritesId,
          exportUrl: 'proxy/favorites/export/' + options.favoritesId
        };
      }
      else {
        newOptions = {
          getUrl: 'proxy/database/get/' + options.databaseName,
          exportUrl: 'proxy/database/export/' + options.databaseName
        };
      }
      Ext.applyIf(newOptions, options);

      this.callParent([newOptions]);
      return this;
    }
  });
});
