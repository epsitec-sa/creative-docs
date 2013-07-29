// This class represents an entity picker whose entity list is backed by a
// database on the server.

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
      var newOptions, list, callback;

      callback = Epsitec.Callback.create(
          this.handleEntityListSelectionChange, this);

      list = {
        onSelectionChange: callback
      };
      Ext.applyIf(list, options.list);

      this.entityListPanel = this.createEntityListPanel(list);

      newOptions = {
        items: [this.entityListPanel]
      };
      Ext.applyIf(newOptions, options);

      this.callParent([newOptions]);
      this.disableOkButton();

      return this;
    },

    /* Methods */

    createEntityListPanel: function(options) {
      return Ext.create('Epsitec.EntityListPanel', {
        container: {},
        list: options
      });
    },

    handleEntityListSelectionChange: function(entityItems) {
      if (entityItems.length === 0) {
        this.disableOkButton();
      }
      else {
        this.enableOkButton();
      }
    },

    getSelectedItems: function() {
      return this.entityListPanel.getEntityList().getSelectedItems();
    },

    /* Static methods */

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
          labelExportDefinitions: databaseDefinition.labelItems,
          menuItems: databaseDefinition.menuItems,
          multiSelect: true
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
