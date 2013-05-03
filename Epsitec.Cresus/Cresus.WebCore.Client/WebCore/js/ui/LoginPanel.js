Ext.require([
  'Epsitec.cresus.webcore.tools.ErrorHandler',
  'Epsitec.cresus.webcore.tools.Texts'
],
function() {
  Ext.define('Epsitec.cresus.webcore.ui.LoginPanel', {
    extend: 'Ext.form.Panel',
    alternateClassName: ['Epsitec.LoginPanel'],

    renderTo: document.body,

    /* Config */

    id: 'loginwindow',
    title: Epsitec.Texts.getLoginTitle(),
    bodyPadding: 5,
    width: 400,
    height: 300,
    floating: true,
    frame: true,
    layout: 'anchor',
    defaults: {
      anchor: '100%'
    },
    fieldDefaults: {
      labelSeparator: null,
      msgTarget: 'side'
    },

    /* Properties */

    application: null,

    /* Constructor */

    constructor: function(options) {
      var newOptions = {
        url: 'proxy/log/in',
        items: this.getItems(),
        buttons: this.getButtons()
      };
      Ext.applyIf(newOptions, options);

      this.callParent([newOptions]);

      return this;
    },

    /* Additional methods */

    getItems: function() {
      var header, usernameField, passwordField;

      header = Ext.create('Ext.Component', {
        autoEl: {
          tag: 'img',
          src: epsitecConfig.splash
        },
        margin: '0 0 20 0'
      });

      usernameField = Ext.create('Ext.form.field.Text', {
        fieldLabel: Epsitec.Texts.getUsernameLabel(),
        name: 'username',
        value: '',
        allowBlank: false,
        listeners: {
          specialkey: this.onSpecialKeyPressed,
          afterrender: function() { usernameField.focus(); },
          scope: this
        }
      });

      passwordField = Ext.create('Ext.form.field.Text', {
        inputType: 'password',
        fieldLabel: Epsitec.Texts.getPasswordLabel(),
        name: 'password',
        value: '',
        allowBlank: false,
        listeners: {
          specialkey: this.onSpecialKeyPressed,
          scope: this
        }
      });

      return [header, usernameField, passwordField];
    },

    getButtons: function() {
      var loginButton;

      loginButton = Ext.create('Ext.button.Button', {
        text: Epsitec.Texts.getLoginLabel(),
        listeners: {
          click: this.onLoginClick,
          scope: this
        }
      });

      return [loginButton];
    },

    onSpecialKeyPressed: function(field, e) {
      if (e.getKey() === e.ENTER) {
        this.onLoginClick();
      }
    },

    onLoginClick: function() {
      var form = this.getForm();
      if (form.isValid()) {
        this.setLoading();
        form.submit({
          success: function(form, action) {
            this.onLoginClickCallback(true, form, action);
          },
          failure: function(form, action) {
            this.onLoginClickCallback(false, form, action);
          },
          scope: this
        });
      }
    },

    onLoginClickCallback: function(success, form, action) {
      this.setLoading(false);

      if (!success) {
        Epsitec.ErrorHandler.handleFormError(action);
        return;
      }

      this.application.showMainPanel();
    }
  });
});
