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
    options.url = 'proxy/entity/edit/' + options.entityId;
    this.buttons = this.getButtons();
    this.callParent(arguments);
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
        success: this.onSaveClickSucess,
        failure: this.onSaveClickFailure,
        scope: this
      });
    }
  },

  onSaveClickSucess: function() {
    this.setLoading(false);
    this.column.refreshToLeft(false);
  },

  onSaveClickFailure: function(form, action) {
    var json;

    this.setLoading(false);

    json = Epsitec.Tools.decodeJson(action.response.responseText);
    if (json === null) {
      return;
    }

    form.markInvalid(json.errors);
  }
});
