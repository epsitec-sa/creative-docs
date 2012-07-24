Ext.define('Epsitec.cresus.webcore.LoginPanel', {
  extend: 'Ext.form.Panel',
  alternateClassName: ['Epsitec.LoginPanel'],

  renderTo: document.body,

  /* Config */

  id: 'loginwindow',
  title: 'Crésus.Core Login',
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
    msgTarget: 'side'
  },

  /* Properties */

  application: null,

  /* Constructor */

  constructor: function(options) {
    options.url = 'proxy/log/in';

    this.application = options.application;

    this.items = this.getItems();
    this.buttons = this.getButtons();

    this.callParent([options]);

    return this;
  },

  /* Additional methods */

  getItems: function() {
    var header, usernameField, passwordField;

    header = Ext.create('Ext.Component', {
      autoEl: {
        tag: 'img',
        src: 'images/Static/Logo.png'
      },
      margin: '0 0 20 0'
    });

    usernameField = Ext.create('Ext.form.field.Text', {
      fieldLabel: 'Username',
      name: 'username',
      value: '',
      allowBlank: false,
      listeners: {
        specialkey: this.onSpecialKeyPressed,
        scope: this
      }
    });

    passwordField = Ext.create('Ext.form.field.Text', {
      inputType: 'password',
      fieldLabel: 'Password',
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
    var resetButton, loginButton;

    resetButton = Ext.create('Ext.button.Button', {
      text: 'Reset',
      listeners: {
        click: this.onResetPressed,
        scope: this
      }
    });

    loginButton = Ext.create('Ext.button.Button', {
      text: 'Log in',
      listeners: {
        click: this.onLoginPressed,
        scope: this
      }
    });

    return [resetButton, loginButton];
  },

  onSpecialKeyPressed: function(field, e) {
    if (e.getKey() === e.ENTER) {
      this.onLoginPressed();
    }
  },

  onResetPressed: function() {
    this.getForm().reset();
  },

  onLoginPressed: function() {
    var form = this.getForm();
    if (form.isValid()) {
      this.setLoading();
      form.submit({
        success: function(form, action) {
          try {
            Ext.decode(action.response.responseText);
          }
          catch (err) {
            Epsitec.ErrorHandler.handleErrorDefault();
            return;
          }

          this.close();
          this.application.runApp();
        },
        failure: function(form, action) {
          this.setLoading(false);

          try {
            var config = Ext.decode(action.response.responseText);
            this.getForm().markInvalid(config.errors);
            Ext.Msg.alert('Failed', 'Could not login. Please try again');
          }
          catch (err) {
            Epsitec.ErrorHandler.handleErrorDefault();
          }
        },
        scope: this
      });
    }
  }
});
