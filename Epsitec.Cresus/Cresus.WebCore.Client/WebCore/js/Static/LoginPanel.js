Ext.define('Epsitec.Cresus.Core.Static.LoginPanel', {
  extend: 'Ext.form.Panel',
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

  defaultType: 'textfield',
  items: [
    {
      xtype: 'box',
      autoEl: {
        tag: 'img',
        src: 'images/Static/Logo.png'
      },
      margin: '0 0 20 0'
    },
    {
      fieldLabel: 'Username',
      name: 'username',
      value: '',
      allowBlank: false,
      listeners: {
        specialkey: function(field, e) {
          if (e.getKey() === e.ENTER) {
            var form = Ext.getCmp('loginwindow');
            form.submitLogin();
          }
        }
      }
    },
    {
      inputType: 'password',
      fieldLabel: 'Password',
      name: 'password',
      value: '',
      allowBlank: false,
      listeners: {
        specialkey: function(field, e) {
          if (e.getKey() === e.ENTER) {
            var form = Ext.getCmp('loginwindow');
            form.submitLogin();
          }
        }
      }
    }
  ],

  /* Properties */

  application: null,

  /* Buttons */

  buttons: [
    {
      text: 'Reset',
      handler: function() {
        this.up('form').getForm().reset();
      }
    },
    {
      text: 'Log in',
      handler: function()  {
        var form = Ext.getCmp('loginwindow');
        form.submitLogin();
      }
    }
  ],

  /* Constructor */

  constructor: function(options) {
    options = options || {};
    options.url = 'proxy/log/in';
    this.application = options.application;

    this.callParent([options]);

    return this;
  },

  /* Additional methods */

  submitLogin: function() {
    var form = this.getForm();
    if (form.isValid()) {
      this.setLoading();
      form.submit({
        success: function(form, action) {
          try {
            Ext.decode(action.response.responseText);
          }
          catch (err) {
            this.failure.apply(arguments);
            return;
          }

          Ext.getCmp('loginwindow').close();
          this.form.application.runApp();
        },
        failure: function(form, action) {
          Ext.Msg.alert('Failed', 'Could not login. Please try again');

          var win = Ext.getCmp('loginwindow');
          win.setLoading(false);

          try {
            var config = Ext.decode(action.response.responseText);
            win.getForm().markInvalid(config.errors);
          }
          catch (err) {
            return;
          }
        }
      });
    }
  }
});
