Ext.define('Epsitec.Cresus.Core.Static.ListContainer',
  {
    extend : 'Ext.panel.Panel',
    id : 'listContainer',
    
    /* Config */
    title : 'Selection',
    region : 'west',
    width : 200,
    minWidth : 175,
    maxWidth : 400,
    margins : '0 0 5 5',
    collapsible : true,
    split : true,
    layout : 'fit',
    
    /* Constructor */
    constructor : function (list)
    {
      this.items = this.items || [];
      this.items.push(list);
    
      this.callParent();
      return this;
    },
    
    /* Additional methods */
    
    showList: function(databaseName) {
      this.removeAll();
      var list = Ext.create('Epsitec.Cresus.Core.Static.List', databaseName);
      this.add(list);
    
      var columnMgr = Ext.getCmp('columnmgr');
      columnMgr.clearColumns();
      
      var tabMgr = Ext.getCmp('tabmgr');
      if (tabMgr != null)
      {
        tabMgr.showEntityTab();
      }
    }
  }
);
 