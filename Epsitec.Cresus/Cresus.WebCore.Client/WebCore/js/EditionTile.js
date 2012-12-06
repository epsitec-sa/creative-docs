Ext.require([
  'Epsitec.cresus.webcore.EntityCollectionField',
  'Epsitec.cresus.webcore.EntityReferenceField',
  'Epsitec.cresus.webcore.EnumerationField',
  'Epsitec.cresus.webcore.ErrorHandler',
  'Epsitec.cresus.webcore.Texts',
  'Epsitec.cresus.webcore.Tools',
  'Epsitec.cresus.webcore.Tile'
],
function() {
  Ext.define('Epsitec.cresus.webcore.EditionTile', {
    extend: 'Epsitec.cresus.webcore.Tile',
    alternateClassName: ['Epsitec.EditionTile'],
    alias: 'widget.epsitec.editiontile',

    /* Config */

    defaults: {
      anchor: '100%'
    },
    fieldDefaults: {
      labelAlign: 'top',
      msgTarget: 'side'
    },

    /* Properties */

    errorField: null,

    /* Constructor */

    constructor: function(options) {
      var newOptions = {
        url: 'proxy/entity/edit/' + options.entityId,
        fbar: {
          items: this.getButtons(),
          cls: 'tile'
        }
      };
      Ext.applyIf(newOptions, options);

      this.callParent([newOptions]);
      return this;
    },

    getButtons: function() {
      var resetButton, saveButton;

      resetButton = Ext.create('Ext.button.Button', {
        margin: '0 5 0 0',
        text: Epsitec.Texts.getResetLabel(),
        listeners: {
          click: this.onResetClick,
          scope: this
        }
      });

      saveButton = Ext.create('Ext.button.Button', {
        margin: 0,
        text: Epsitec.Texts.getSaveLabel(),
        listeners: {
          click: this.onSaveClick,
          scope: this
        }
      });

      return [resetButton, saveButton];
    },

    onResetClick: function() {
      this.hideError();
      this.getForm().reset();
    },

    onSaveClick: function() {
      this.hideError();
      var form = this.getForm();
      if (form.isValid()) {
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
      var json, businessError;

      this.setLoading(false);

      if (!success) {
        Epsitec.ErrorHandler.handleFormError(action);

        json = Epsitec.Tools.decodeResponse(action.response);
        if (json === null) {
          return;
        }

        businessError = json.errors.business;
        if (!Epsitec.Tools.isUndefined(businessError))
        {
          this.showError(businessError);
        }

        return;
      }

      this.column.refreshToLeft();
    },

    showError: function(error) {
      if (this.errorField === null)
      {
        this.errorField = Ext.create('Ext.form.field.Display', {
          baseBodyCls: 'business-error',
          fieldCls: null,
          fieldLabel: Epsitec.Texts.getErrorTitle()
        }),
        this.insert(0, this.errorField);
      }
      this.errorField.setValue(error);
    },

    hideError: function() {
      if (this.errorField !== null)
      {
        this.remove(this.errorField);
        this.errorField = null;
      }
    }
  });
});
