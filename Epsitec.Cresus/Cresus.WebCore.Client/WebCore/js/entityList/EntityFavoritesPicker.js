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
          list1, list2, callback;

      callback = Epsitec.Callback.create(
          this.handleEntityListSelectionChange, this);

      // I've no clue why I need to assign the callback manually to both lists;
      // I've observed that if I insert the callback before calling Ext.apply
      // above, the property 'onselectionChange' will be copied as 'null'.
      // [PA: 2013-02-24]

      list1 = {};
      list2 = {};

      Ext.apply(list1, options.list);
      Ext.apply(list2, list1);

      delete list2.favoritesId;

      list1.onSelectionChange = callback;
      list2.onSelectionChange = callback;

      this.entityListPanel1 = this.createEntityListPanel(list1);
      this.entityListPanel2 = this.createEntityListPanel(list2);

      this.tabPanel = new Ext.TabPanel({
        xtype: 'tabpanel',
        id: 'tabpanel',
        activeTab: 0,
        layoutOnTabChange: true,
        items: [{
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
        }],
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

    /* Additional methods */

    handleTabChange: function(tabPanel, newCard, oldCard, eOpts) {
      this.activeEntityListPanel = newCard.entityListPanel;
      this.handleEntityListSelectionChange(this.getSelectedItems());
    },

    handleEntityListSelectionChange: function(entityItems) {
      if (entityItems.length === 0) {
        this.disableOkButton();
      } else {
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

    statics: {
      showDatabase: function(databaseName, favoritesId, multiSelect, callback) {
        this.show(callback, {
          entityListTypeName: 'Epsitec.DatabaseEntityList',
          databaseName: databaseName,
          favoritesId: favoritesId,
          multiSelect: multiSelect,
          onSelectionChange: null
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
