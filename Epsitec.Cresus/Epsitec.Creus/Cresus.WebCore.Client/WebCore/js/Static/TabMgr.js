Ext.define('Epsitec.Cresus.Core.Static.TabMgr',
  {
    extend : 'Ext.tab.Panel',
    id : 'tabmgr',
    
    /* Config */
    activeTab : 0,
    plain : true,
    region : 'center',
    margins : '0 5 5 0',
    
    /* Properties */
    pages : null,
    
    /* Constructor */
    constructor : function (list)
    {
      this.items = this.items || [];
      this.items.push(list);
    
      this.callParent();
      return this;
    },
    
    /* Additional methods */
    showEntityTab : function ()
    {
      this.setActiveTab(0);
    },
    
    createPage : function (title, url)
    {
      
      this.pages = this.pages || {};
      
      var panel = this.pages[url];
      
      if (panel == null || panel.isDestroyed)
      {
        var tab = Ext.create('Epsitec.Cresus.Core.Static.TabbedPage',
            {
              title : title,
              id : url,
              loader :
              {
                url : url,
                autoLoad : true
              }
            }
          );
        
        this.add(tab);
        this.setActiveTab(tab);
        
        this.pages[url] = tab;
      }
      else
      {
        panel.show();
      }
    }
  }
);
 