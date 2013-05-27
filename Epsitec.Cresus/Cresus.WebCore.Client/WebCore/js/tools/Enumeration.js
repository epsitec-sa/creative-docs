Ext.require([
  'Epsitec.cresus.webcore.tools.Tools'
],
function() {
  Ext.define('Epsitec.cresus.webcore.tools.Enumeration', {
    extend: 'Ext.Base',
    alternateClassName: ['Epsitec.Enumeration'],

    statics: {
      getStore: function(name) {
        return this.getEnumerationStore(name, 'proxy/enum/get/' + name);
      },

      getEnumerationStore: function(name, url) {
        var store = Ext.data.StoreManager.lookup(name);

        if (Epsitec.Tools.isUndefined(store) || store === null) {
          store = Ext.create('Ext.data.Store', {
            storeId: name,
            fields: ['id', 'text'],
            autoLoad: true,
            isLoaded: false,
            listeners: {
              load: function(me, records, successful, eOpts) {
                me.isLoaded = true;
              },
              scope: this
            },
            proxy: Ext.create('Ext.data.proxy.Ajax', {
              type: 'ajax',
              url: url,
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
