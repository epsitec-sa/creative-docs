Ext.require([
  'Epsitec.cresus.webcore.EntityList'
],
function() {
  Ext.define('Epsitec.cresus.webcore.DatabaseEntityList', {
    extend: 'Epsitec.cresus.webcore.EntityList',
    alternateClassName: ['Epsitec.DatabaseEntityList'],

    /* Constructor */

    constructor: function(options) {
      var newOptions = {
        getUrl: 'proxy/database/get/' + options.databaseName
      };
      Ext.applyIf(newOptions, options);

      this.callParent([newOptions]);
      return this;
    }
  });
});
