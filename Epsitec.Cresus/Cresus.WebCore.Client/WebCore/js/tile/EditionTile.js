Ext.require([
  'Epsitec.cresus.webcore.field.EntityCollectionField',
  'Epsitec.cresus.webcore.field.EntityReferenceField',
  'Epsitec.cresus.webcore.field.EnumerationField',
  'Epsitec.cresus.webcore.tile.EntityTile',
  'Epsitec.cresus.webcore.tools.ErrorHandler',
  'Epsitec.cresus.webcore.tools.Texts',
  'Epsitec.cresus.webcore.tools.Tools'
],
function() {
  Ext.define('Epsitec.cresus.webcore.tile.EditionTile', {
    extend: 'Epsitec.cresus.webcore.tile.EntityTile',
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
        margin: '0 0 0 0',
        text: Epsitec.Texts.getResetLabel(),
        listeners: {
          click: this.onResetClick,
          scope: this
        }
      });

      saveButton = Ext.create('Ext.button.Button', {
        margin: '0 5 0 0',
        text: Epsitec.Texts.getSaveLabel(),
        listeners: {
          click: this.onSaveClick,
          scope: this
        }
      });

      return [saveButton, resetButton];
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

      this.column.refreshToLeft(true);
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
    },

    getState: function() {
      return {
        type: 'edidionTile',
        entityId: this.entityId
      };
    },

    setState: function(state) {
      // Nothing to do here.
    },

    isStateApplicable: function(state) {
      return state.type === 'editionTile' && state.entityId === this.entityId;
    }
  });
});
