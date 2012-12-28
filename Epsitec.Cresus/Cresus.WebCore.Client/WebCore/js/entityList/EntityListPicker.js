Ext.require([
  'Epsitec.cresus.webcore.entityList.EntityListPanel',
  'Epsitec.cresus.webcore.tools.EntityPicker'
],
function() {
  Ext.define('Epsitec.cresus.webcore.entityList.EntityListPicker', {
    extend: 'Epsitec.cresus.webcore.tools.EntityPicker',
    alternateClassName: ['Epsitec.EntityListPicker'],

    /* Properties */

    entityListPanel: null,

    /* Constructor */

    constructor: function(options) {
      var newOptions;

      this.entityListPanel = this.createEntityListPanel(options.list);

      newOptions = {
        items: [this.entityListPanel]
      };
      Ext.applyIf(newOptions, options);

      this.callParent([newOptions]);
      return this;
    },

    /* Additional methods */

    createEntityListPanel: function(options) {
      return Ext.create('Epsitec.EntityListPanel', {
        container: { },
        list: options
      });
    },

    getSelectedItems: function() {
      return this.entityListPanel.getEntityList().getSelectedItems();
    },

    statics: {
      showDatabase: function(databaseName, multiSelect, callback) {
        this.show(callback, {
          entityListTypeName: 'Epsitec.DatabaseEntityList',
          databaseName: databaseName,
          multiSelect: multiSelect,
          onSelectionChange: null
        });
      },

      showSet: function(viewId, entityId, databaseDefinition, callback) {
        this.show(callback, {
          entityListTypeName: 'Epsitec.SetEntityList',
          viewId: viewId,
          entityId: entityId,
          columnDefinitions: databaseDefinition.columns,
          sorterDefinitions: databaseDefinition.sorters,
          multiSelect: true,
          onSelectionChange: null
        });
      },

      show: function(callback, listOptions) {
        var entityListPicker = Ext.create('Epsitec.EntityListPicker', {
          list: listOptions,
          callback: callback
        });
        entityListPicker.show();
      }
    }
  });
});
