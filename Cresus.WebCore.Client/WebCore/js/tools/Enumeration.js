// This class provides functions that let the javascript client get enumeration
// data from the server and create store with this data. The content of these
// store never changes, and they are cached to speed up the process. If the data
// of an enumeration has already been loaded in a store, the exact same store
// will always be returned for this enumeration.

Ext.require([
  'Epsitec.cresus.webcore.tools.Tools'
],
function() {
  Ext.define('Epsitec.cresus.webcore.tools.Enumeration', {
    extend: 'Ext.Base',
    alternateClassName: ['Epsitec.Enumeration'],

    /* Static methods */

    statics: {
      getStore: function(name) {
        return this.getEnumerationStore(name, 'proxy/enum/get/' + name);
      },

      getEnumerationStore: function(name, url) {
        var store = Ext.data.StoreManager.lookup(name);

        if (!Ext.isDefined(store) || store === null) {
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
