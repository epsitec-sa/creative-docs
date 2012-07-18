Ext.Loader.setConfig(
  {
    enabled : true,
    paths :
    {
      'Epsitec.Cresus.Core' : 'js'
    }
  }
);

Ext.application(
  {
    name : 'Epsitec.Cresus.Core',
    appFolder : 'js',
    
    requires :
    [
      'Epsitec.Cresus.Core.Static.Callback',
      'Epsitec.Cresus.Core.Static.CallbackQueue',
      'Epsitec.Cresus.Core.Static.CollectionSummaryTile',
      'Epsitec.Cresus.Core.Static.ColumnManager',
      'Epsitec.Cresus.Core.Static.ColumnPanel',
      'Epsitec.Cresus.Core.Static.EditionTile',
      'Epsitec.Cresus.Core.Static.EmptySummaryTile',
      'Epsitec.Cresus.Core.Static.EntityCollectionField',
      'Epsitec.Cresus.Core.Static.EntityList',
      'Epsitec.Cresus.Core.Static.EntityListItem',
      'Epsitec.Cresus.Core.Static.EntityListPanel',
      'Epsitec.Cresus.Core.Static.EntityPanel',
      'Epsitec.Cresus.Core.Static.EntityReferenceComboBox',
      'Epsitec.Cresus.Core.Static.EntityReferenceField',
      'Epsitec.Cresus.Core.Static.EnumerationComboBox',
      'Epsitec.Cresus.Core.Static.EnumerationField',
      'Epsitec.Cresus.Core.Static.ErrorHandler',
      'Epsitec.Cresus.Core.Static.LoginPanel',
      'Epsitec.Cresus.Core.Static.Menu',
      'Epsitec.Cresus.Core.Static.SummaryTile',
      'Epsitec.Cresus.Core.Static.TabbedPage',
      'Epsitec.Cresus.Core.Static.TabMgr',
      'Epsitec.Cresus.Core.Static.Tools',
    ],
    
    columnmgr : null,
    tabmgr : null,
    menu : null,
    entityListPanel : null,
    viewport : null,
    launch : function ()
    {
      this.doLogin();
    },
    
    doLogin : function ()
    {
      var win = Ext.create('Epsitec.Cresus.Core.Static.LoginPanel',
          {
            application : this
          }
        );
    },
    
    runApp : function ()
    {
      this.columnmgr = Ext.create('Epsitec.Cresus.Core.Static.ColumnManager');
      this.menu = Ext.create('Epsitec.Cresus.Core.Static.Menu');
      this.entityListPanel = Ext.create('Epsitec.Cresus.Core.Static.EntityListPanel');
      this.tabmgr = Ext.create('Epsitec.Cresus.Core.Static.TabMgr', this.columnmgr);
      
      this.viewport = Ext.create('Ext.container.Viewport',
        {
          layout : 'border',
          items : [
            this.menu,
            this.entityListPanel,
            this.tabmgr
          ]
        }
      );
    }
  }
);
 