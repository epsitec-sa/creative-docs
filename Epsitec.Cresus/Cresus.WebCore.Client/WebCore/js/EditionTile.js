Ext.define('Epsitec.cresus.webcore.EditionTile', {
  extend: 'Epsitec.cresus.webcore.Tile',
  alternateClassName: ['Epsitec.EditionTile'],
  alias: 'widget.epsitec.editiontile',

  /* Config */

  border: false,
  frame: true,
  margin: '0 0 5 0',
  defaultType: 'textfield',
  defaults: {
    anchor: '100%'
  },
  fieldDefaults: {
    labelAlign: 'top',
    msgTarget: 'side'
  },

  /* Constructor */

  constructor: function(options) {
    var newOptions = {
      url: 'proxy/entity/edit/' + options.entityId,
      buttons: this.getButtons()
    };
    Ext.applyIf(newOptions, options);

    this.callParent([newOptions]);
    return this;
  },

  getButtons: function() {
    var resetButton, saveButton;

    resetButton = Ext.create('Ext.button.Button', {
      text: 'Reset',
      listeners: {
        click: this.onResetClick,
        scope: this
      }
    });

    saveButton = Ext.create('Ext.button.Button', {
      text: 'Save',
      listeners: {
        click: this.onSaveClick,
        scope: this
      }
    });

    return [resetButton, saveButton];
  },

  onResetClick: function() {
    this.getForm().reset();
  },

  onSaveClick: function() {
    var form = this.getForm();
    if (this.form.isValid()) {
      this.setLoading();
      form.submit({
        success: function(form, action) {
          this.onSaveClickCallback(true, form, action);
        },
        failure: function(form, action) {
          this.onSaveClickCallback(false, form, action);
        },
        scope: this
      });
    }
  },

  onSaveClickCallback: function(success, form, action) {
    this.setLoading(false);

    if (!success) {
      Epsitec.ErrorHandler.handleFormError(action);
      return;
    }

    this.column.refreshToLeft(false);
  }
});
