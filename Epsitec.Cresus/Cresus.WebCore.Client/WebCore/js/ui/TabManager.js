// This class manages all the tabs that are displayed in the main window of the
// application. There are two kinds of tabs:
// - entity tabs which display a list of entities on the left with their data on
//   the right
// - page tabs, which display a static html page

Ext.require([
  'Epsitec.cresus.webcore.entityUi.ColumnManager'
],
function() {
  Ext.define('Epsitec.cresus.webcore.ui.TabManager', {
    extend: 'Ext.panel.Panel',
    alternateClassName: ['Epsitec.TabManager'],

    /* Configuration */

    border: false,
    plain: true,
    layout: 'card',

    /* Properties */

    application: null,
    entityTabs: null,
    pageTabs: null,

    /* Constructor */

    constructor: function() {
      this.entityTabs = {};
      this.pageTabs = {};

      this.callParent(arguments);
      return this;
    },

    /* Methods */

    showEntityTab: function(database) {
      var key, entityTab;

      key = database.name;
      entityTab = this.entityTabs[key] || null;

      if (entityTab === null || entityTab.isDestroyed) {
        entityTab = Ext.create('Epsitec.ColumnManager', {
          database: database,
          header: false,
          border: false
        });

        this.add(entityTab);
        this.entityTabs[key] = entityTab;
      }

      this.showTab(entityTab);

      return entityTab;
    },

    showPageTab: function(title, url) {
      var pageTab = this.pageTabs[url] || null;

      if (pageTab === null || pageTab.isDestroyed) {
        pageTab = Ext.create('Ext.panel.Panel', {
          title: title,
          border: false,
          id: url,
          loader: {
            url: url,
            autoLoad: true
          }
        });

        this.add(pageTab);
        this.pageTabs[url] = pageTab;
      }

      this.showTab(pageTab);

      return pageTab;
    },

    showTab: function(tab) {
      this.getLayout().setActiveItem(tab);
    }
  });
});
