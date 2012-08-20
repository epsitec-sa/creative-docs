Ext.define('Epsitec.cresus.webcore.EntityPicker', {
  extend: 'Ext.window.Window',
  alternateClassName: ['Epsitec.EntityPicker'],

  /* Config */
  width: 640,
  height: 480,
  layout: 'fit',
  modal: true,
  border: false,
  title: 'Entity selection',

  /* Properties */
  callback: null,
  entityListPanel: null,

  /* Constructor */
  constructor: function(options) {
    var newOptions;

    this.entityListPanel = this.createEntityListPanel(options);

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
      container: {},
      list: {
        databaseName: options.databaseName,
        editable: false,
        multiSelect: options.multiSelect,
        onSelectionChange: null
      }
    });
  },

  createCancelButton: function() {
    return Ext.create('Ext.Button', {
      text: 'Cancel',
      handler: this.onCancelClick,
      scope: this
    });
  },

  createOkButton: function() {
    return Ext.create('Ext.Button', {
      text: 'Ok',
      handler: this.onSaveClick,
      scope: this
    });
  },

  onSaveClick: function() {
    var selectedItems = this.entityListPanel.getEntityList().getSelectedItems();
    this.callback.execute([selectedItems]);
    this.close();
  },

  onCancelClick: function() {
    this.close();
  },

  statics: {
    show: function(databaseName, multiSelect, callback) {
      var entityPicker = Ext.create('Epsitec.EntityPicker', {
        databaseName: databaseName,
        multiSelect: multiSelect,
        callback: callback
      });
      entityPicker.show();
    }
  }
});
