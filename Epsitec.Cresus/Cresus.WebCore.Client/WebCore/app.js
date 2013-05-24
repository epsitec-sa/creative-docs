/*jslint white: true */

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
  'Epsitec.cresus.webcore.tools.Texts',
  'Epsitec.cresus.webcore.tools.ViewMode',
  'Epsitec.cresus.webcore.hub.Notifications'
],
function() {
  Ext.application({
    name: 'Epsitec.Cresus.Core',
    appFolder: 'js',

    /* Properties */

    menu: null,
    loginPanel: null,
    tabManager: null,
    notificationsClient: null,

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

      Ext.EventManager.on(document, 'keydown', function(event) {

        var doPrevent = false, d, tagName, tagType, isTextField, isTextArea;

        if (event.keyCode === 8) {

          d = event.srcElement || event.target;
          tagName = d.tagName.toUpperCase();
          tagType = d.type.toUpperCase();

          isTextField = tagName === 'INPUT' &&
              (tagType === 'TEXT' || tagType === 'PASSWORD');
          isTextArea = tagName === 'TEXTAREA';

          if (isTextField || isTextArea) {
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

    showMainPanel: function(form) {
      var items;

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

      if (epsitecConfig.displayBannerMessage) {
        items = [
          this.menu,
          this.createBanner('north', 'test-banner-top'),
          this.tabManager
        ];
      }
      else {
        items = [
          this.menu,
          this.tabManager
        ];
      }

      if (epsitecConfig.featureNotifications) {
        this.notificationsClient = Ext.create(
            'Epsitec.Notifications', NotificationsToastr, form);
      }

      Ext.create('Ext.container.Viewport', {
        layout: 'border',
        style: {
          background: '#FFFFFF'
        },
        items: items
      });
    },

    showEditableEntity: function(path, message, errorField, endCallbackFunc) {

      //check if navigation data is present
      if (path.id && path.name)
      {
        var tab, callback, endCallback, lastTile;

        //executed when edition tile is loaded
        endCallback = Epsitec.CallbackQueue.create(
            function() {
              lastTile = tab.columns[tab.columns.length - 1];
              //finaly
              endCallbackFunc(
                  lastTile, errorField.header, errorField.name,
                  errorField.message
              );
            },
            this);

        //prepare callback for editing
        callback = Epsitec.CallbackQueue.create(
            function() {
              tab.addEntityColumn(
                  Epsitec.ViewMode.edition, null, path.id, null, endCallback
              );
            },
            this);

        if (this.tabManager.getEntityTab(path) === null) {
          this.tabManager.showEntityTab(path);
          tab = this.tabManager.getEntityTab(path);
        }
        else {
          tab = this.tabManager.getEntityTab(path);
          this.tabManager.showTab(tab);
          tab.removeAllColumns();
        }
        //summary tile
        tab.addEntityColumn(
            Epsitec.ViewMode.summary, null, path.id, null, callback
        );
      }
    },

    createBanner: function(region, cls) {
      return Ext.create('Ext.Panel', {
        region: region,
        bodyCls: ['test-banner', cls],
        html: epsitecConfig.bannerMessage
      });
    }
  });
});
