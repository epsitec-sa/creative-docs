// This class is the base class of all entity pickers, i.e. the windows that are
// used to pick one or more entities from a list of them. It is derived by
// EntityListPicker, EntityFavouritesPicker and AiderGroupPicker. Basically this
// is simply a window with an OK and a Cancel buttons, that the child classes
// must fill in order to let the user pick an entity.

Ext.require([
  'Epsitec.cresus.webcore.tools.Texts'
],
function() {
  Ext.define('Epsitec.cresus.webcore.tools.EntityPicker', {
    extend: 'Ext.window.Window',

    /* Configuration */

    width: 640,
    height: 480,
    layout: 'fit',
    modal: true,
    border: false,
    title: Epsitec.Texts.getEntityPickerTitle(),

    /* Properties */

    callback: null,
    okButton: null,

    /* Constructor */

    constructor: function(options) {
      var newOptions;

      this.okButton = this.createOkButton();

      newOptions = {
        buttons: [
          this.okButton,
          this.createCancelButton()
        ]
      };
      Ext.applyIf(newOptions, options);

      this.callParent([newOptions]);
      return this;
    },

    /* Methods */

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

    disableOkButton: function() {
      this.okButton.disable();
    },

    enableOkButton: function() {
      this.okButton.enable();
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
