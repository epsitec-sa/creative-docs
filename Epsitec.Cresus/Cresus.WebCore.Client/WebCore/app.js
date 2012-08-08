Ext.Loader.setConfig({
  enabled: true,
  paths: {
    'Epsitec.cresus.webcore': 'js'
  }
});

Ext.application({
  name: 'Epsitec.Cresus.Core',
  appFolder: 'js',

  requires: [
    'Epsitec.cresus.webcore.Callback',
    'Epsitec.cresus.webcore.CallbackQueue',
    'Epsitec.cresus.webcore.CollectionSummaryTile',
    'Epsitec.cresus.webcore.ColumnManager',
    'Epsitec.cresus.webcore.EditionTile',
    'Epsitec.cresus.webcore.EmptySummaryTile',
    'Epsitec.cresus.webcore.EntityCollectionField',
    'Epsitec.cresus.webcore.EntityColumn',
    'Epsitec.cresus.webcore.EntityList',
    'Epsitec.cresus.webcore.EntityListItem',
    'Epsitec.cresus.webcore.EntityPicker',
    'Epsitec.cresus.webcore.EntityReferenceField',
    'Epsitec.cresus.webcore.EnumerationComboBox',
    'Epsitec.cresus.webcore.EnumerationField',
    'Epsitec.cresus.webcore.ErrorHandler',
    'Epsitec.cresus.webcore.LeftEntityList',
    'Epsitec.cresus.webcore.LoginPanel',
    'Epsitec.cresus.webcore.Menu',
    'Epsitec.cresus.webcore.SummaryTile',
    'Epsitec.cresus.webcore.TabManager',
    'Epsitec.cresus.webcore.Tools'
  ],

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
      margin: 5
    });

    this.tabManager = Ext.create('Epsitec.TabManager', {
      application: this,
      region: 'center',
      margin: 5
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
