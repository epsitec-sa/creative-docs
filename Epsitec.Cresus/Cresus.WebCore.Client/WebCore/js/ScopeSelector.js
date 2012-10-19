Ext.require([

],
function() {
  Ext.define('Epsitec.cresus.webcore.ScopeSelector', {
    extend: 'Ext.container.Container',
    alternateClassName: ['Epsitec.ScopeSelector'],

    /* Config */

    /* Properties */

    comboBox: null,

    /* Constructor */

    constructor: function(options) {
      var newOptions;

      this.comboBox = this.createComboBox();

      this.getScopeData();

      newOptions = {
        items: [this.comboBox]
      };
      Ext.applyIf(newOptions, options);

      this.callParent([newOptions]);
      return this;
    },

    createComboBox: function() {
      return Ext.create('Ext.form.field.ComboBox', {
        displayField: 'name',
        valueField: 'id'
      });
    },

    getScopeData: function() {
      Ext.Ajax.request({
        url: 'proxy/scope/list',
        callback: this.getScopeDataCallback,
        scope: this
      });
    },

    getScopeDataCallback: function(options, success, response) {
      var json;

      json = Epsitec.Tools.processResponse(success, response);
      if (json === null) {
        return;
      }

      this.comboBox.bindStore(
          Ext.create('Ext.data.Store', {
            fields: ['id', 'name'],
            autoLoad: true,
            proxy: Ext.create('Ext.data.proxy.Memory', {
              type: 'memory',
              reader: {
                type: 'json'
              }
            }),
            data: json.content.scopes
          })
      );

      this.comboBox.select(json.content.activeId);

      this.comboBox.on('change', this.onSelectionChangeCallback, this);
    },

    onSelectionChangeCallback: function(comboBox, newValue, oldValue, eOpts) {
      Ext.Ajax.request({
        url: 'proxy/scope/set',
        method: 'POST',
        params: {
          scopeId: newValue
        },
        scope: this
      });
    }
  });
});
