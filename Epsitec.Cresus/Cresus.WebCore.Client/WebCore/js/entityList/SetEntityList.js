Ext.require([
  'Epsitec.cresus.webcore.entityList.EntityList'
],
function() {
  Ext.define('Epsitec.cresus.webcore.entityList.SetEntityList', {
    extend: 'Epsitec.cresus.webcore.entityList.EntityList',
    alternateClassName: ['Epsitec.SetEntityList'],

    /* Constructor */

    constructor: function(options) {
      var newOptions = {
        getUrl: Epsitec.SetEntityList.getUrl(
            options.viewId, options.entityId, 'get/pick'
        )
      };
      Ext.applyIf(newOptions, options);

      this.callParent([newOptions]);
      return this;
    },

    statics: {
      getUrl: function(viewId, entityId, urlSuffix) {
        return 'proxy/set/' + viewId + '/' + entityId + '/' + urlSuffix;
      }
    }
  });
});
