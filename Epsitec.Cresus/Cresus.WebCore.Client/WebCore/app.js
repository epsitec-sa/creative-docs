/*jslint white: true */

// This file is the main entry point of the javascript client application. It
// is mainly used to set up other objects and show the login or main panels of
// the application. It also provides ways to navigate to a given entity, based
// on its entity id and its database name. In addition, it provides fixes to
// localization errors and undesirable behaviors in ExtJs.

//This JQuery.getScript ensure that JS Signalr hubs definitions are ready before loading app
$.getScript('signalr/hubs', function() {
  Ext.Loader.setConfig({
    enabled: true,
    paths: {
      'Epsitec.cresus.webcore': './js',
      'Ext.ux': './lib/extjs/examples/ux'
    }
  });

  Ext.require([
    'Epsitec.cresus.webcore.hub.Hubs',
    'Epsitec.cresus.webcore.locale.Locale',
    'Epsitec.cresus.webcore.ui.LoginPanel',
    'Epsitec.cresus.webcore.ui.Menu',
    'Epsitec.cresus.webcore.ui.TabManager',
    'Epsitec.cresus.webcore.ui.ActionPage',
    'Epsitec.cresus.webcore.ui.EntityBag',
    'Epsitec.cresus.webcore.ui.FaqWindow',
    'Epsitec.cresus.webcore.tools.Texts',
    'Epsitec.cresus.webcore.tools.ViewMode'
  ],
  function() {
    Ext.application({
      name: 'Epsitec.Cresus.Core',
      appFolder: 'js',

      /* Properties */
      viewport: null,
      menu: null,
      loginPanel: null,
      tabManager: null,
      entityBag: null,
      actionPage: null,
      faqWindow: null,
      hubs: null,

      /* Application entry point */

      launch: function() {
        this.setupWindowTitle();
        this.fixLocalizationBug();
        this.fixFrenchLocalizationError();
        this.fixFilterMenuLocalizationError();
        this.fixBooleanColumnLocalizationError();
        this.fixBackspaceHandling();
        this.fixDragAndDropManager();
        this.showLoginPanel();
      },

      /* Methods */
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
            }
            else {
              doPrevent = true;
            }
          }

          if (doPrevent) {
            event.preventDefault();
          }
        });
      },

      fixLocalizationBug: function() {

        // There is a bug in extjs. The loading texts Ext.LoadMask is not
        // localized properly.

        var loadingText = Ext.view.AbstractView.prototype.loadingText;

        Ext.override(Ext.LoadMask, {
          msg: loadingText
        });
      },

      fixFrenchLocalizationError: function() {

        // The french localization is wrong in extjs. This method corrects these
        // errors.

        var cm, exists;

        cm = Ext.ClassManager;
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

      fixDragAndDropManager: function () {
        Ext.dd.DragDropMgr.getLocation = function (i) {
          if (!this.isTypeOfDD(i)) { 
            return null 
          }
          if (i.getRegion){ 
            return i.getRegion() 
          }
          var g = i.getEl(), m, d, c, o, n, p, a, k, h;

          if(g!=null) {
            try { 
              m = Ext.Element.getXY(g) 
            }
            catch (j) { }

            if (!m) { 
              return null 
            }

            d = m[0];
            c = d + g.offsetWidth;
            o = m[1];
            n = o + g.offsetHeight;
            p = o - i.padding[0];
            a = c + i.padding[1];
            k = n + i.padding[2];
            h = d - i.padding[3];       
            return new Ext.util.Region(p, a, k, h)
          }
        };
      },
      

      showLoginPanel: function() {
        this.loginPanel = Ext.create('Epsitec.LoginPanel', {
          application: this
        });
      },

      showMainPanel: function(username) {
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
          margin: '0 0 0 1'
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
        

        this.hubs = Ext.create('Epsitec.Hubs',username);

        

        if(epsitecConfig.featureChat)
        {
          this.hubs.registerHub("chat",SignalRChat);
        } 

        if(epsitecConfig.featureElasticSearch)
        {
          this.hubs.registerHub("elastic",ElasticSearch);
        }

        if (epsitecConfig.featureNotifications) {
          this.hubs.registerHub("toastr",NotificationsToastr);
        }

        if(epsitecConfig.featureEntityBag) {
          this.hubs.registerHub("entitybag",EntityBagHub);
        }

        this.hubs.start();

        this.viewport = Ext.create('Ext.container.Viewport', {
          layout: 'border',
          style: {
            background: '#FFFFFF'
          },
          items: items
        });

        if(epsitecConfig.featureEntityBag) {
          this.entityBag = Ext.create('Epsitec.EntityBag',this.menu);
        }

        if(epsitecConfig.featureActionPage) {
          this.actionPage = Ext.create('Epsitec.ActionPage');
        }

        if(epsitecConfig.featureFaq) {
          this.faqWindow = Ext.create('Epsitec.FaqWindow');
        }      
      },

      reloadCurrentDatabase: function(samePage) {
        var key = this.tabManager.currentTab;
        var columnManager = this.tabManager.entityTabs[key];
        var currentSelection, selectionIndex;
        
        if(Ext.isDefined(columnManager))
        {   
            currentSelection = columnManager.leftList.entityList.getSelectionModel().getSelection();
            if(currentSelection.length>0)
            {
              selectionIndex = currentSelection[0].index;
              columnManager.leftList.entityList.getSelectionModel().deselectAll();
              columnManager.leftList.entityList.reloadAndScrollToEntity(columnManager,currentSelection[0].internalId,selectionIndex,samePage);
            }
            else
            {
              columnManager.leftList.entityList.reload(columnManager);
            }
                
        }
        
      },

      reloadCurrentTile: function(callback) {
        var key = this.tabManager.currentTab;
        var columnManager = this.tabManager.entityTabs[key];
        var currentSelection, selectionIndex;
        var path = {};
        
        if(Ext.isDefined(columnManager))
        {   
          currentSelection = columnManager.leftList.entityList.getSelectionModel().getSelection();
          if(Ext.isDefined(currentSelection[0]))
          {
            path.entityId = currentSelection[0].internalId;
            path.databaseName = key;

            if(columnManager.columns.length > 1)
            {
              if(Ext.isDefined(columnManager.columns[1].viewId))
              {
                this.showEditableEntity(path,callback);
              }
              else
              {
                this.showEntity(path,callback);
              }
              
            }
            else
            {
              this.showEntity(path,callback);
            } 
          }              
        }
        else
        {
          this.reloadCurrentDatabase(true);
        }
        
      },

      getCurrentDatabaseEntityType: function() {
        var key = this.tabManager.currentTab;
        var columnManager = this.tabManager.entityTabs[key];
        if(Ext.isDefined(columnManager))
        {
          return columnManager.leftList.title;
        }
        else
          return "type inconnu";
      },

      addEntityToBag: function(entity) {
        this.entityBag.addEntityToBag(entity);      
      },

      removeEntityFromBag: function(entity) {
        this.entityBag.removeEntityFromBag(entity);      
      },

      removeEntityFromClientBag: function(entity) {
        this.entityBag.removeEntityFromClientBag(entity);      
      },

      addEntityToTarget: function(entity) {
        this.actionPage.addTargetEntity(entity);      
      },


      showEntity: function(path, callback) {
        if (!Ext.isDefined(path.entityId) || !Ext.isDefined(path.databaseName)) {
          return;
        }

        var database, entityId, columnManager;

        // Todo complete this object. Otherwhise, the header of the list with the
        // title and the icon won't be shown.
        database = {
          name: path.databaseName,
          title: path.entityType
        };

        columnManager = this.tabManager.showEntityTab(database);
        if(columnManager.leftList!==null)
        {
          columnManager.leftList.removeAllFilters();
        }
        entityId = path.entityId;
        columnManager.selectEntity(entityId, callback);
      },

      showEditableEntity: function(path, callback) {

        // This callback is kind of brittle here:
        // - It is supposed to open an editable column in the second column. But
        //   it works by simple expanding the first tile in the first column. We
        //   have no guarantee that this will open an edition tile. The server
        //   might likely return a summary column instead.
        // - Moreover, it assumes that the first tile in the first column is a (or
        //   derives from a) summary tile. If it is a grouped summary tile or an
        //   edition tile, it won't work.
        // - What is less likely (but possible), is that the fist colum does not
        //   contain any tile. This might happen if it is a set column for
        //   instance.

        var newCallback = Epsitec.Callback.create(
            function(entityColumn) {
              entityColumn.getTiles()[0].openNextTile(callback);
            },
            this);

        this.showEntity(path, newCallback);
      },

      showEditableEntityWithError: function(path, error) {

        // This callback is kind of brittle here.
        // - It assumes that we have a column with an edition tile as the first
        //   tile. But there is no guarantee that this is the case. If it is not,
        //   the method won't display any error message.
        // - It assumes that the field that we want to tag as wrong is in present
        //   in this first edition tile. Again, there is no guarantee that this is
        //   the case. If it is not, the method won't tag it.

        var callback = Epsitec.Callback.create(
            function(entityColumn) {
              var tile = entityColumn.getTiles()[0];

              if (error.tileMessage) {
                tile.showError(error.tileMessage);
              }

              if (error.fieldName && error.fieldMessage) {
                tile.showFieldError(error.fieldName, error.fieldMessage);
              }
            },
            this);

        this.showEditableEntity(path, callback);
      },

      testAlert: function(message)
      {
        alert(message);
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
});