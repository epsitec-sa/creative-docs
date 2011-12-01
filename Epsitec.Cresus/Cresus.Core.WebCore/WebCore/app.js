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

// Some classes need to be loaded before the launch
// They define aliases that have to be known
Ext.require('Epsitec.Cresus.Core.Static.Checkboxes');
Ext.require('Epsitec.Cresus.Core.Static.Enum');
Ext.require('Epsitec.Cresus.Core.Static.Entity');
Ext.require('Epsitec.Cresus.Core.Static.ErrorHandler');
Ext.require('Epsitec.Cresus.Core.Static.ListItem');
Ext.require('Epsitec.Cresus.Core.Static.WallPanelEdition');
Ext.require('Epsitec.Cresus.Core.Static.WallPanelEmptySummary');
Ext.require('Epsitec.Cresus.Core.Static.WallPanelSummary');

Ext.application(
  {
    name : 'Epsitec.Cresus.Core',
    appFolder : 'js',
    
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
 