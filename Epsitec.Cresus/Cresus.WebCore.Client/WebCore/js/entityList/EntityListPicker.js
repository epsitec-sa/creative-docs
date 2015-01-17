// This class represents an entity picker where the user can pick an entity by
// using one or more entity lists. The lists are displayed within tabs (if
// there is only a single list, the tabs are not displayed).

Ext.require([
  'Epsitec.cresus.webcore.entityList.EntityListPanel',
  'Epsitec.cresus.webcore.tools.EntityPicker',
  'Epsitec.cresus.webcore.tools.Texts'
],
function() {
  Ext.define('Epsitec.cresus.webcore.entityList.EntityListPicker', {
    extend: 'Epsitec.cresus.webcore.tools.EntityPicker',
    alternateClassName: ['Epsitec.EntityListPicker'],

    /* Properties */

    activeEntityListPanel: null,

    /* Constructor */

    constructor: function(options) {
      var newOptions, tabPanel;

      tabPanel = this.createTabPanel(options.lists);
      this.activeEntityListPanel = tabPanel.items[0].entityListPanel;

      newOptions = {
        items: [tabPanel]
      };

      Ext.applyIf(newOptions, options);

      this.callParent([newOptions]);
      this.disableOkButton();

      return this;
    },

    /* Methods */

    createTabPanel: function(lists) {
      var tabPanel = {
        xtype: 'tabpanel',
        activeTab: 0,
        layoutOnTabChange: true,
        items: this.createTabs(lists),
        listeners: {
          tabchange: this.handleTabChange,
          scope: this
        }
      };

      // If we have a single tab, we don't display the header as it is useless.
      if (tabPanel.items.length === 1) {
        tabPanel.tabBar = {
          hidden: true
        };
      }

      return tabPanel;
    },

    createTabs: function(lists) {
      var callback = Epsitec.Callback.create(
          this.handleEntityListSelectionChange, this);

      return lists.map(
          function(l) { return this.createTab(callback, l); },
          this);
    },

    createTab: function(callback, list) {
      var entityListPanel = this.createEntityListPanel(callback, list.options);

      return {
        xtype: 'panel',
        layout: 'fit',
        title: list.title,
        items: [entityListPanel],
        entityListPanel: entityListPanel
      };
    },

    createEntityListPanel: function(callback, options) {
      var newOptions = {
        onSelectionChange: callback
      };
      Ext.applyIf(newOptions, options);

      return Ext.create('Epsitec.EntityListPanel', {
        container: {
          border: false
        },
        list: newOptions
      });
    },

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

    getSelectedItems: function() {
      return this.activeEntityListPanel.getEntityList().getSelectedItems();
    },

    /* Static methods */

    statics: {
      showDatabase: function(databaseName, multiSelect, callback) {
        this.show(callback, [{
          title: Epsitec.Texts.getPickerAllItems(),
          options: {
            entityListTypeName: 'Epsitec.DatabaseEntityList',
            databaseName: databaseName,
            multiSelect: multiSelect
          }
        }]);
      },

      showSet: function(viewId, entityId, databaseDefinition, callback) {
        this.show(callback, [{
          title: Epsitec.Texts.getPickerAllItems(),
          options: {
            entityListTypeName: 'Epsitec.SetEntityList',
            viewId: viewId,
            entityId: entityId,
            columnDefinitions: databaseDefinition.columns,
            sorterDefinitions: databaseDefinition.sorters,
            labelExportDefinitions: databaseDefinition.labelItems,
            menuItems: databaseDefinition.menuItems,
            multiSelect: true
          }
        }]);
      },

      showFavorites: function(dbName, favId, favOnly, multiSelect, callback) {
        var lists = [{
          title: Epsitec.Texts.getPickerFavouriteItems(),
          options: {
            entityListTypeName: 'Epsitec.FavoritesEntityList',
            databaseName: dbName,
            favoritesId: favId,
            multiSelect: multiSelect
          }
        }];

        // If we are not required to display only the favorites list, we add a
        // tab to display the full content of the database.

        if (!favOnly) {
          lists.push({
            title: Epsitec.Texts.getPickerAllItems(),
            options: {
              entityListTypeName: 'Epsitec.DatabaseEntityList',
              databaseName: dbName,
              multiSelect: multiSelect
            }
          });
        }

        return this.show(callback, lists);
      },

      show: function(callback, lists) {
        var entityListPicker = Ext.create('Epsitec.EntityListPicker', {
          lists: lists,
          callback: callback
        });
        entityListPicker.show();
      }
    }
  });
});
