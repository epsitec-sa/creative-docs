// This class is the login panel for the second factor (SMS PIN).

Ext.require([
  'Epsitec.cresus.webcore.tools.ErrorHandler',
  'Epsitec.cresus.webcore.tools.Texts'
],
function() {
  Ext.define('Epsitec.cresus.webcore.ui.LoginPanelPin', {
    extend: 'Ext.form.Panel',
    alternateClassName: ['Epsitec.LoginPanelPin'],

    /* Configuration */

    renderTo: document.body,
    id: 'loginwindowpin',
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
    username: null,

    /* Constructor */

    constructor: function(options) {
      var newOptions = {
        url: 'proxy/log/in2',
        items: this.getItems(),
        buttons: this.getButtons()
      };
      Ext.applyIf(newOptions, options);

      this.callParent([newOptions]);

      return this;
    },

    /* Methods */

    getItems: function() {
      var header, pinField;

      header = Ext.create('Ext.Component', {
        autoEl: {
          tag: 'img',
          src: epsitecConfig.splash
        },
        margin: '0 0 20 0'
      });

      pinField = Ext.create('Ext.form.field.Text', {
        fieldLabel: 'PIN reçu par SMS',
        name: 'pin',
        value: '',
        allowBlank: true,
        listeners: {
          specialkey: this.onSpecialKeyPressed,
          afterrender: function() { this.application.focusTextField('pin'); },
          scope: this
        }
      });
      return [header, pinField];
    },

    getButtons: function() {
      var backButton, loginButton;

      backButton = Ext.create('Ext.button.Button', {
        text: 'Retour',
        listeners: {
          click: this.onBackClick,
          scope: this
        }
      });
      
      loginButton = Ext.create('Ext.button.Button', {
        text: Epsitec.Texts.getLoginLabel(),
        listeners: {
          click: this.onLoginClick,
          scope: this
        }
      });

      return [backButton, loginButton];
    },

    onSpecialKeyPressed: function(field, e) {
      if (e.getKey() === e.ENTER) {
        this.onLoginClick();
      }
    },

    onBackClick: function() {
      this.application.showLoginPanel();
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

      if (success) {
        this.application.showMainPanel(this.username);
      } else {
        Epsitec.ErrorHandler.handleFormError(action);
        this.application.focusTextField('pin');
      }
    }
  });
});
