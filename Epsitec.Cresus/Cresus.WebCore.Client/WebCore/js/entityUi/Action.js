Ext.require([
  'Epsitec.cresus.webcore.entityUi.BrickWallParser',
  'Epsitec.cresus.webcore.tools.Tools'
],
function() {
  Ext.define('Epsitec.cresus.webcore.entityUi.Action', {
    extend: 'Ext.window.Window',
    alternateClassName: ['Epsitec.Action'],

    /* Config */

    maxHeight: 500,
    layout: 'fit',
    border: false,
    modal: true,
    plain: true,

    /* Properties */

    form: null,
    callback: null,
    errorField: null,

    /* Constructor */

    constructor: function(options) {
      this.form = this.getForm(options);
      this.callParent([{
        callback: options.callback,
        title: options.items[0].title,
        iconCls: options.items[0].iconCls,
        items: [this.form],
        buttons: this.getButtons()
      }]);
      return this;
    },

    /* Additional methods */

    getForm: function(options) {
      return Ext.create('Ext.form.Panel', {
        xtype: 'form',
        url: 'proxy/entity/executeAction/' + options.viewId + '/' +
            options.entityId,
        border: false,
        autoScroll: true,
        frame: true,
        width: 350,
        margin: '5 5 5 5',
        fieldDefaults: {
          labelAlign: 'top',
          msgTarget: 'side',
          anchor: '100%'
        },
        items: this.getFormItems(options.items[0])
      });
    },

    getFormItems: function(item) {
      var items = item.fields;

      if (Ext.isDefined(item.text) && item.text !== null) {
        items.unshift({
          xtype: 'displayfield',
          value: item.text
        });
      }

      return items;
    },

    getButtons: function() {
      var cancelButton = this.createCancelButton();
      var okButton = this.createOkButton();
      return [okButton, cancelButton];
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

    onCancelClick: function() {
      this.close();
    },

    onSaveClick: function() {
      var form = this.form.getForm();
      if (form.isValid()) {
        this.hideError();
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

      this.close();
      this.callback.execute([]);
    },

    showError: function(error) {
      if (this.errorField === null)
      {
        this.errorField = Ext.create('Ext.form.field.Display', {
          baseBodyCls: 'business-error',
          fieldCls: null,
          fieldLabel: Epsitec.Texts.getErrorTitle()
        }),
        this.form.insert(0, this.errorField);
      }
      this.errorField.setValue(error);
    },

    hideError: function() {
      if (this.errorField !== null)
      {
        this.form.remove(this.errorField);
        this.errorField = null;
      }
    },

    /* Static methods */

    statics: {
      showDialog: function(viewId, entityId, callback) {
        Ext.Ajax.request({
          url: 'proxy/layout/6/' + viewId + '/' + entityId,
          callback: function(options, success, response) {
            this.showDialogCallback(success, response, callback);
          },
          scope: this
        });
      },

      showDialogCallback: function(success, response, callback) {
        var json, options, dialog;

        json = Epsitec.Tools.processResponse(success, response);
        if (json === null) {
          return;
        }

        options = Epsitec.BrickWallParser.parseColumn(json.content);
        options.callback = callback;

        dialog = Ext.create('Epsitec.Action', options);
        dialog.show();
      }
    }
  });
});
