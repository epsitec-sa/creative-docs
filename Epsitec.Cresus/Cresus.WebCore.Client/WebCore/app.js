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
    'Epsitec.cresus.webcore.ColumnPanel',
    'Epsitec.cresus.webcore.EditionTile',
    'Epsitec.cresus.webcore.EmptySummaryTile',
    'Epsitec.cresus.webcore.EntityCollectionField',
    'Epsitec.cresus.webcore.EntityList',
    'Epsitec.cresus.webcore.EntityListItem',
    'Epsitec.cresus.webcore.EntityListPanel',
    'Epsitec.cresus.webcore.EntityPanel',
    'Epsitec.cresus.webcore.EntityReferenceComboBox',
    'Epsitec.cresus.webcore.EntityReferenceField',
    'Epsitec.cresus.webcore.EnumerationComboBox',
    'Epsitec.cresus.webcore.EnumerationField',
    'Epsitec.cresus.webcore.ErrorHandler',
    'Epsitec.cresus.webcore.LoginPanel',
    'Epsitec.cresus.webcore.Menu',
    'Epsitec.cresus.webcore.SummaryTile',
    'Epsitec.cresus.webcore.TabManager',
    'Epsitec.cresus.webcore.Tools'
  ],

  /* Properties */

  menu: null,
  tabManager: null,

  launch: function() {
    this.doLogin();
  },

  /* Additional methods */

  doLogin: function() {
    Ext.create('Epsitec.cresus.webcore.LoginPanel', {
      application: this
    });
  },

  runApp: function() {
    this.menu = Ext.create('Epsitec.cresus.webcore.Menu', {
      application: this,
      region: 'north',
      margin: 5
    });

    this.tabManager = Ext.create('Epsitec.cresus.webcore.TabManager', {
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
