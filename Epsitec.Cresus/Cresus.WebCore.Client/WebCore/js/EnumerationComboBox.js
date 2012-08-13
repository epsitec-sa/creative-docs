Ext.define('Epsitec.cresus.webcore.EnumerationComboBox', {
  extend: 'Ext.form.field.ComboBox',
  alternateClassName: ['Epsitec.EnumerationComboBox'],
  alias: 'widget.epsitec.enumerationcombobox',

  /* Config */

  valueField: 'id',
  displayField: 'name',
  queryMode: 'local',
  forceSelection: true,
  typeAhead: true,

  /* Constructor */

  constructor: function(options) {
    this.store = Epsitec.EnumerationComboBox.getStore(
        this, options.enumerationName, options.value
        );

    this.callParent(arguments);
    return this;
  },

  statics: {
    getStore: function(comboBox, enumerationName, value) {
      var store, callback;

      this.stores = this.stores || [];

      store = this.stores[enumerationName] || null;

      if (store === null) {
        callback = function() { comboBox.select(value); };
        store = this.createStore(enumerationName, callback);
        this.stores[enumerationName] = store;
      }

      return store;
    },

    createStore: function(enumerationName, callback) {
      return Ext.create('Ext.data.Store', {
        fields: ['id', 'name'],
        autoLoad: true,
        proxy: Ext.create('Ext.data.proxy.Ajax', {
          type: 'ajax',
          url: 'proxy/enum/get/' + enumerationName,
          reader: {
            type: 'json',
            root: 'content.values'
          }
        }),
        listeners: {
          load: {
            fn: callback,
            scope: null
          }
        }
      });
    }
  }
});
