Ext.require([
  'Epsitec.cresus.webcore.tools.Texts'
],
function() {
  Ext.define('Epsitec.cresus.webcore.tools.EntityPicker', {
    extend: 'Ext.window.Window',

    /* Config */

    width: 640,
    height: 480,
    layout: 'fit',
    modal: true,
    border: false,
    title: Epsitec.Texts.getEntityPickerTitle(),

    /* Properties */

    callback: null,

    /* Constructor */

    constructor: function(options) {
      var newOptions = {
        buttons: [
          this.createOkButton(),
          this.createCancelButton()
        ]
      };
      Ext.applyIf(newOptions, options);

      this.callParent([newOptions]);
      return this;
    },

    /* Additional methods */

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
      this.callback.execute([this.getSelectedItems()]);
      this.close();
    },

    getSelectedItems: function() {
      // This function is to be overriden in derived classes.
    },

    onCancelClick: function() {
      this.close();
    }
  });
});
