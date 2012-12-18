Ext.require([
  'Epsitec.cresus.webcore.entityList.EntityListPanel',
  'Epsitec.cresus.webcore.tools.Texts'
],
function() {
  Ext.define('Epsitec.cresus.webcore.entityList.EntityPicker', {
    extend: 'Ext.window.Window',
    alternateClassName: ['Epsitec.EntityPicker'],

    /* Config */

    width: 640,
    height: 480,
    layout: 'fit',
    modal: true,
    border: false,
    title: Epsitec.Texts.getEntityPickerTitle(),

    /* Properties */

    callback: null,
    entityListPanel: null,

    /* Constructor */

    constructor: function(options) {
      var newOptions;

      this.entityListPanel = this.createEntityListPanel(options.list);

      newOptions = {
        items: [this.entityListPanel],
        buttons: [
          this.createCancelButton(),
          this.createOkButton()
        ]
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

    createCancelButton: function() {
      return Ext.create('Ext.Button', {
        text: Epsitec.Texts.getCancelLabel(),
        handler: this.onCancelClick,
        scope: this
      });
    },

    createOkButton: function() {
      return Ext.create('Ext.Button', {
        text: Epsitec.Texts.getOkLabel(),
        handler: this.onSaveClick,
        scope: this
      });
    },

    onSaveClick: function() {
      var entityList, selectedItems;

      entityList = this.entityListPanel.getEntityList();
      selectedItems = entityList.getSelectedItems();

      this.callback.execute([selectedItems]);
      this.close();
    },

    onCancelClick: function() {
      this.close();
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
        var entityPicker = Ext.create('Epsitec.EntityPicker', {
          list: listOptions,
          callback: callback
        });
        entityPicker.show();
      }
    }
  });
});
