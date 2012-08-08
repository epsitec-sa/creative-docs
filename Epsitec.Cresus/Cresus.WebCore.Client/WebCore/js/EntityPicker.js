Ext.define('Epsitec.cresus.webcore.EntityPicker', {
  extend: 'Ext.window.Window',
  alternateClassName: ['Epsitec.EntityPicker'],

  /* Config */
  width: 640,
  height: 480,
  layout: 'fit',
  modal: true,
  title: 'Entity selection',

  /* Properties */
  callback: null,
  entityList: null,

  /* Constructor */
  constructor: function(options) {
    this.entityList = this.createEntityList(options.databaseName);

    options.items = [this.entityList];
    options.buttons = [
      this.createCancelButton(),
      this.createOkButton()
    ];

    this.callParent(arguments);
    return this;
  },

  /* Additional methods */

  createEntityList: function(databaseName) {
    return Ext.create('Epsitec.EntityList', {
      databaseName: databaseName
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
    var selectedItems = this.entityList.getSelectedItems();
    this.callback.execute([selectedItems]);
    this.close();
  },

  onCancelClick: function() {
    this.close();
  },

  statics: {
    show: function(databaseName, callback) {
      var entityPicker = Ext.create('Epsitec.EntityPicker', {
        databaseName: databaseName,
        callback: callback
      });
      entityPicker.show();
    }
  }
});
