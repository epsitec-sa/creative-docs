Ext.require([
  'Epsitec.cresus.webcore.entityList.EntityList'
],
function () {
  Ext.define('Epsitec.cresus.webcore.entityList.DatabaseEntityList', {
    extend: 'Epsitec.cresus.webcore.entityList.EntityList',
    alternateClassName: ['Epsitec.DatabaseEntityList'],

    /* Constructor */

    constructor: function (options) {
      var newOptions;
      if (options.favoritesId) {
        newOptions = {
          getUrl: 'proxy/favorites/get/' + options.favoritesId
        };
      } else {
        newOptions = {
          getUrl: 'proxy/database/get/' + options.databaseName
        };
      }
      Ext.applyIf(newOptions, options);

      this.callParent([newOptions]);
      return this;
    }
  });
});
