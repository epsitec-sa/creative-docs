Ext.define('Epsitec.cresus.webcore.TabManager', {
  extend: 'Ext.tab.Panel',

  /* Config */
  plain: true,

  /* Properties */

  application: null,
  entityTabs: {},
  pageTabs: {},

  /* Additional methods */

  showEntityTab: function(database) {
    var key = database.DatabaseName;
    var entityTab = this.entityTabs[key] || null;

    if (entityTab === null || entityTab.isDestroyed) {
      entityTab = Ext.create('Epsitec.cresus.webcore.ColumnManager', {
        database: database,
        closable: true
      });

      this.add(entityTab);
      this.entityTabs[key] = entityTab;
    }

    this.setActiveTab(entityTab);
  },

  showPageTab: function(title, url) {
    var pageTab = this.pageTabs[url] || null;

    if (pageTab === null || pageTab.isDestroyed) {
      pageTab = Ext.create('Ext.panel.Panel', {
        title: title,
        closable: true,
        id: url,
        loader: {
          url: url,
          autoLoad: true
        }
      });

      this.add(pageTab);
      this.pageTabs[url] = pageTab;
    }

    this.setActiveTab(pageTab);
  }
});
