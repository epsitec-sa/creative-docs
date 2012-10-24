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
      this.fixLocalizationBug();
      this.showLoginPanel();
    },

    /* Additional methods */

    fixLocalizationBug: function() {

      // There is a bug in extjs. The loading texts for Ext.view.AbstractView
      // and Ext.LoadMask are not localized properly. That's a know bug as of
      // version 4.1.2 and its id is EXTJSIV-7483. This method implement the
      // suggested workaround that can be found on the sencha forums at the url
      // http://www.sencha.com/forum/showthread.php?245783 .

      var loadingText = Ext.view.AbstractView.prototype.msg;

      Ext.override(Ext.view.AbstractView, {
        loadingText: loadingText
      });

      Ext.override(Ext.LoadMask, {
        msg: loadingText
      });
    },

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
