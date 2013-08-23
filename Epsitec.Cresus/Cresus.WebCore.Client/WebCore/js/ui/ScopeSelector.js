// This class provides a combo box to the user so that it can select the current
// scope that he wishes to work in.

Ext.require([
  'Epsitec.cresus.webcore.tools.Tools'
],
function() {
  Ext.define('Epsitec.cresus.webcore.ui.ScopeSelector', {
    extend: 'Ext.container.Container',
    alternateClassName: ['Epsitec.ScopeSelector'],

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

    /* Methods */

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
      var json, scopes, activeScopeId;

      json = Epsitec.Tools.processResponse(success, response);
      if (json === null) {
        return;
      }

      scopes = json.content.scopes;

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
            data: scopes
          })
      );

      if (scopes.length === 0) {
        this.comboBox.disable();
      }

      activeScopeId = json.content.activeId;

      if (activeScopeId !== null) {
        this.comboBox.select(activeScopeId);
      }

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

      //Reload current database
      Epsitec.Cresus.Core.app.reloadCurrentDatabase();
    }
  });
});
