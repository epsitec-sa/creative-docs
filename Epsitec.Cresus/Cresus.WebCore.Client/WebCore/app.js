Ext.Loader.setConfig(
  {
    enabled : true,
    //disableCaching : false,
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
      'Epsitec.Cresus.Core.Static.Checkboxes',
      'Epsitec.Cresus.Core.Static.ColumnManager',
      'Epsitec.Cresus.Core.Static.ColumnPanel',
      'Epsitec.Cresus.Core.Static.Entity',
      'Epsitec.Cresus.Core.Static.EntityComboBox',
      'Epsitec.Cresus.Core.Static.Enum',
      'Epsitec.Cresus.Core.Static.EnumComboBox',
      'Epsitec.Cresus.Core.Static.ErrorHandler',
      'Epsitec.Cresus.Core.Static.List',
      'Epsitec.Cresus.Core.Static.ListContainer',
      'Epsitec.Cresus.Core.Static.ListItem',
      'Epsitec.Cresus.Core.Static.LoginPanel',
      'Epsitec.Cresus.Core.Static.Menu',
      'Epsitec.Cresus.Core.Static.TabbedPage',
      'Epsitec.Cresus.Core.Static.TabMgr',
      'Epsitec.Cresus.Core.Static.WallPanelEdition',
      'Epsitec.Cresus.Core.Static.WallPanelEmptySummary',
      'Epsitec.Cresus.Core.Static.WallPanelSummary',
    ],
    
    columnmgr : null,
    tabmgr : null,
    menu : null,
    listContainer : null,
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
      // This feature could be cool, but does not work with Firefox 4+
      // The message is not shown
      /*
      window.onbeforeunload = function ()
    {
      return 'Please exit the application by clicking the "logout" button';
      }
       */
      
      this.columnmgr = Ext.create('Epsitec.Cresus.Core.Static.ColumnManager');
      this.menu = Ext.create('Epsitec.Cresus.Core.Static.Menu');
      var list = Ext.create('Epsitec.Cresus.Core.Static.List', 'proxy/database/customers');
      this.listContainer = Ext.create('Epsitec.Cresus.Core.Static.ListContainer', list);
      this.tabmgr = Ext.create('Epsitec.Cresus.Core.Static.TabMgr', this.columnmgr);
      
      this.viewport = Ext.create('Ext.container.Viewport',
          {
            layout : 'border',
            items : [
              this.menu,
              this.listContainer,
              this.tabmgr
            ]
          }
        );
    }
  }
);
 