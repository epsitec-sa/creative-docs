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
    var newOptions;

    this.entityList = this.createEntityList(options);

    newOptions = {
      items: [this.entityList],
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

  createEntityList: function(options) {
    var config = {
      databaseName: options.databaseName
    };

    if (options.multiSelect) {
      config.selModel = {
        type: 'rowmodel',
        mode: 'MULTI'
      };
    }

    return Ext.create('Epsitec.EntityList', config);
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
