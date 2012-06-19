Ext.define('Epsitec.Cresus.Core.Static.Menu',
  {
    extend : 'Ext.Toolbar',
    id : 'menu',
    
    /* Config */
    region : 'north',
    margins : 5,
    
    /* Constructor */
    constructor : function (options)
    {
      this.items = this.items || new Array();
      
      this.downloadMenus();
      
      var about = Ext.create('Ext.Action',
          {
            text : 'About',
            handler : function ()
            {
              var tabMgr = Ext.getCmp('tabmgr');
              tabMgr.createPage("About box", "proxy/page/about");
            },
            scale : 'large',
            iconAlign : 'top',
            iconCls : 'epsitec-cresus-core-images-base-softwareuserrole-icon32'
          }
        );
      
      var logout = Ext.create('Ext.Action',
          {
            text : 'Logout',
            handler : function ()
            {
              Ext.Ajax.request(
                {
                  url : 'proxy/log/out',
                  callback : function ()
                  {
                    window.location.reload();
                  }
                }
              );
            },
            scale : 'large',
            iconAlign : 'top',
            iconCls : 'epsitec-cresus-core-images-usermanager-icon32'
          }
        );
      
      this.items.push(
        '->',
        {
          xtype : 'buttongroup',
          title : 'Options',
          headerPosition : 'bottom',
          items : [
            about,
            logout
          ]
        }
      );
      
      this.callParent(arguments);
      return this;
    },
    
    /* Additional methods */
    
    downloadMenus : function ()
    {
      Ext.Ajax.request(
        {
          url : 'proxy/database/list',
          success : function (response, options)
          {
            try
            {
              var config = Ext.decode(response.responseText);
            }
            catch (err)
            {
              options.failure.apply(arguments);
              return;
            }
            
            this.handleMenus(config.content);
            
          },
          failure : function (response, options)
          {
            panel.setLoading(false);
            Epsitec.Cresus.Core.Static.ErrorHandler.handleError(response);
          },
          scope : this
        }
      );
    },
    
    handleMenus : function (menus)
    {
      
      var group = Ext.create('Ext.container.ButtonGroup',
          {
            title : 'Databases',
            headerPosition : 'bottom'
          }
        );
      
      Ext.Array.each(menus, function (menu)
        {
          var m = Ext.create('Ext.Action',
              {
                text : menu.Title,
                handler : function ()
                {
                  var l = Ext.getCmp('listContainer');
                  l.showList(menu.DatabaseName);
                },
                scale : 'large',
                iconAlign : 'top',
                iconCls : menu.CssClass
              }
            );
          
          group.add(m);
        }
      );
      
      this.insert(0, group);
    }
  }
);
 