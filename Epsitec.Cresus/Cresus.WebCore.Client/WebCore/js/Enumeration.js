Ext.require([
  'Epsitec.cresus.webcore.Tools'
],
function() {
  Ext.define('Epsitec.cresus.webcore.Enumeration', {
    extend: 'Ext.Base',
    alternateClassName: ['Epsitec.Enumeration'],

    statics: {
      getStore: function(name) {
        var store = Ext.data.StoreManager.lookup(name);

        if (Epsitec.Tools.isUndefined(store) || store === null) {
          store = Ext.create('Ext.data.Store', {
            storeId: name,
            fields: ['id', 'text'],
            autoLoad: true,
            proxy: Ext.create('Ext.data.proxy.Ajax', {
              type: 'ajax',
              url: 'proxy/enum/get/' + name,
              reader: {
                type: 'json',
                root: 'content.values'
              },
              listeners: {
                exception: function(proxy, response, operation, eOpts) {
                  this.onExceptionCallback(store, response);
                },
                scope: this
              }
            })
          });
        }

        return store;
      },

      onExceptionCallback: function(store, response) {
        Ext.data.StoreManager.remove(store);
        Epsitec.Tools.processProxyError(response);
      }
    }
  });
});
