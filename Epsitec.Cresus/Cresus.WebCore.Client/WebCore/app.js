Ext.Loader.setConfig({
  enabled: true,
  paths: {
    'Epsitec.cresus.webcore': './js',
    'Ext.ux': './lib/extjs/examples/ux'
  }
});

Ext.require([
  'Epsitec.cresus.webcore.locale.Locale',
  'Epsitec.cresus.webcore.ui.LoginPanel',
  'Epsitec.cresus.webcore.ui.Menu',
  'Epsitec.cresus.webcore.ui.TabManager',
  'Epsitec.cresus.webcore.tools.Texts'
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
      this.setupWindowTitle();
      this.fixLocalizationBug();
      this.fixFrenchLocalizationError();
      this.fixFilterMenuLocalizationError();
      this.fixBooleanColumnLocalizationError();
      this.fixBackspaceHandling();
      this.showLoginPanel();
    },

    /* Additional methods */

    setupWindowTitle: function() {
      window.document.title = Epsitec.Texts.getWindowTitle();
    },

    fixBackspaceHandling: function() {

      // Prevent the backspace key from navigating back.

      Ext.EventManager.on(document, 'keydown', function (event) {

        var doPrevent = false;

        if (event.keyCode === 8) {
          
          var d = event.srcElement || event.target;
          var tagName = d.tagName.toUpperCase();
          var tagType = d.type.toUpperCase();
          
          if ((tagName === 'INPUT' && (tagType === 'TEXT' || tagType === 'PASSWORD'))
           || (tagName === 'TEXTAREA')) {
            doPrevent = d.readOnly || d.disabled;
          } else {
            doPrevent = true;
          }
        }
        
        if (doPrevent) {
          event.preventDefault();
        }
      });
    },

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

    fixFrenchLocalizationError: function() {

      // The french localization is wrong in extjs. This method corrects these
      // errors.

      var cm, exists;

      cm = Ext.ClassManager,
      exists = Ext.Function.bind(cm.get, cm);

      if (Epsitec.Locale.getLocaleName() === 'fr') {
        if (exists('Ext.util.Format')) {
          Ext.apply(Ext.util.Format, {
            thousandSeparator: '\'',
            decimalSeparator: '.'
          });
        }

        Ext.define('Ext.locale.fr.form.field.Number', {
          override: 'Ext.form.field.Number',
          decimalSeparator: '.'
        });
      }
    },

    fixFilterMenuLocalizationError: function() {

      // This menu is not localized, so we do it here.

      var cm, exists;

      cm = Ext.ClassManager;
      exists = Ext.Function.bind(cm.get, cm);

      if (Epsitec.Locale.getLocaleName() === 'fr') {
        Ext.override(Ext.ux.grid.FiltersFeature, {
          menuFilterText: 'Filtres'
        });
        Ext.override(Ext.ux.grid.menu.RangeMenu, {
          menuItemCfgs: {
            emptyText: 'Entrez un nombre...',
            selectOnFocus: false,
            width: 200
          }
        });
        Ext.override(Ext.ux.grid.filter.DateFilter, {
          afterText: 'Apr\u00E8s le',
          beforeText: 'Avant le',
          onText: 'Le'
        });
        Ext.override(Ext.ux.grid.filter.BooleanFilter, {
          yesText: 'Oui',
          noText: 'Non'
        });
        Ext.override(Ext.ux.grid.menu.ListMenu, {
          loadingText: 'Chargement...'
        });
      }
    },

    fixBooleanColumnLocalizationError: function() {

      // This column is not localized, so we do it here.

      var cm, exists;

      cm = Ext.ClassManager;
      exists = Ext.Function.bind(cm.get, cm);

      if (Epsitec.Locale.getLocaleName() === 'fr') {
        Ext.override(Ext.grid.column.Boolean, {
          falseText: 'Non',
          trueText: 'Oui'
        });
      }
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
        region: 'north'
      });

      this.tabManager = Ext.create('Epsitec.TabManager', {
        application: this,
        region: 'center',
        border: false,
        margin: '1 0 0 0'
      });

      Ext.create('Ext.container.Viewport', {
        layout: 'border',
        style: {
          background: '#FFFFFF'
        },
        items: [
          this.menu,
          this.tabManager
        ]
      });
    }
  });
});
