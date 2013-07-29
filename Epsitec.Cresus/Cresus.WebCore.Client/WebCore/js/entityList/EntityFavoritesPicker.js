// This class is an entity picker with two entity lists, the main one backed by
// a favourite collection on the server and another one backed by a database on
// the server.

Ext.require([
  'Epsitec.cresus.webcore.entityList.EntityListPanel',
  'Epsitec.cresus.webcore.tools.EntityPicker',
  'Epsitec.cresus.webcore.tools.Texts'
],
function() {
  Ext.define('Epsitec.cresus.webcore.entityList.EntityFavoritesPicker', {
    extend: 'Epsitec.cresus.webcore.tools.EntityPicker',
    alternateClassName: ['Epsitec.EntityFavoritesPicker'],

    /* Properties */

    entityListPanel1: null,
    entityListPanel2: null,
    tabPanel: null,
    activeEntityListPanel: null,

    /* Constructor */

    constructor: function(options) {
      var newOptions,
          list1, list2, callback,
          tabItems;

      callback = Epsitec.Callback.create(
          this.handleEntityListSelectionChange, this);

      list1 = {
        entityListTypeName: 'Epsitec.FavoritesEntityList',
        onSelectionChange: callback
      };
      list2 = {
        entityListTypeName: 'Epsitec.DatabaseEntityList',
        onSelectionChange: callback
      };

      Ext.applyIf(list1, options.list);
      Ext.applyIf(list2, options.list);

      this.entityListPanel1 = this.createEntityListPanel(list1);
      this.entityListPanel2 = this.createEntityListPanel(list2);

      if (options.list.favoritesOnly) {
        tabItems = [{
          xtype: 'panel',
          layout: 'fit',
          title: Epsitec.Texts.getPickerFavouriteItems(),
          items: [this.entityListPanel1],
          entityListPanel: this.entityListPanel1
        }];
      }
      else {
        tabItems = [{
          xtype: 'panel',
          layout: 'fit',
          title: Epsitec.Texts.getPickerFavouriteItems(),
          items: [this.entityListPanel1],
          entityListPanel: this.entityListPanel1
        }, {
          xtype: 'panel',
          layout: 'fit',
          title: Epsitec.Texts.getPickerAllItems(),
          items: [this.entityListPanel2],
          entityListPanel: this.entityListPanel2
        }];
      }

      this.tabPanel = new Ext.TabPanel({
        xtype: 'tabpanel',
        id: 'tabpanel',
        activeTab: 0,
        layoutOnTabChange: true,
        items: tabItems,
        listeners: {
          tabchange: this.handleTabChange,
          scope: this
        }
      });

      this.activeEntityListPanel = this.entityListPanel1;

      newOptions = {
        items: [this.tabPanel]
      };
      Ext.applyIf(newOptions, options);

      this.callParent([newOptions]);
      this.disableOkButton();

      return this;
    },

    /* Methods */

    handleTabChange: function(tabPanel, newCard, oldCard, eOpts) {
      this.activeEntityListPanel = newCard.entityListPanel;
      this.handleEntityListSelectionChange(this.getSelectedItems());
    },

    handleEntityListSelectionChange: function(entityItems) {
      if (entityItems.length === 0) {
        this.disableOkButton();
      }
      else {
        this.enableOkButton();
      }
    },

    createEntityListPanel: function(options) {
      return Ext.create('Epsitec.EntityListPanel', {
        container: {},
        list: options
      });
    },

    getSelectedItems: function() {
      return this.activeEntityListPanel.getEntityList().getSelectedItems();
    },

    /* Static methods */

    statics: {
      showFavorites: function(dbName, favId, favOnly, multiSelect, callback) {
        this.show(callback, {
          databaseName: dbName,
          favoritesId: favId,
          favoritesOnly: favOnly,
          multiSelect: multiSelect
        });
      },

      show: function(callback, listOptions) {
        var entityListPicker = Ext.create('Epsitec.EntityFavoritesPicker', {
          list: listOptions,
          callback: callback
        });
        entityListPicker.show();
      }
    }
  });
});
