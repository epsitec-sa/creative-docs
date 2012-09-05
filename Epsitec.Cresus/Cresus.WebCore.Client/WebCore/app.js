Ext.Loader.setConfig({
  enabled: true,
  paths: {
    'Epsitec.cresus.webcore': 'js',
    'Ext.ux': 'lib/extjs/examples/ux'
  }
});

Ext.require([
  'Epsitec.cresus.webcore.LoginPanel',
  'Epsitec.cresus.webcore.Menu',
  'Epsitec.cresus.webcore.TabManager'
],
function() {
  Ext.application({
    name: 'Epsitec.Cresus.Core',
    appFolder: 'js',

    /* Properties */

    menu: null,
    loginPanel: null,
    tabManager: null,

    launch: function() {
      this.showLoginPanel();
    },

    /* Additional methods */

    showLoginPanel: function() {
      this.loginPanel = Ext.create('Epsitec.LoginPanel', {
        application: this
      });
    },

    showMainPanel: function() {
      this.loginPanel.close();
      this.loginPanel = null;

      this.menu = Ext.create('Epsitec.Menu', {
        application: this,
        region: 'north',
        cls: 'border-bottom',
        margin: '0 0 5 0'
      });

      this.tabManager = Ext.create('Epsitec.TabManager', {
        application: this,
        region: 'center',
        border: false
      });

      Ext.create('Ext.container.Viewport', {
        layout: 'border',
        items: [
          this.menu,
          this.tabManager
        ]
      });
    }
  });
});
