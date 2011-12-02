Ext.define('Epsitec.Cresus.Core.Static.WallPanelSummary',
  {
    extend : 'Ext.Panel',
    alias : 'widget.summary',
    
    /* Config */
    margin : '0 0 5 0',
    bodyCls : 'summary',
    overCls : 'over-summary',
    
    /* Properties */
    entityId : null,
    lambda : null,
    entityType : null,
    isRoot : false,
    clickToEdit : true,
    hideRemoveButton : false,
    hideAddButton : false,
    selectedPanelCls : 'selected-entity',
    selected : false,
    
    /* Constructor */
    constructor : function (o)
    {
      var options = o || {};
      this.addEntityTools(options);
      
      this.callParent(new Array(options));
      return this;
    },
    
    /* Listeners */
    listeners :
    {
      render : function (c)
      {
        c.body.on('click',
          function ()
          {
            this.bodyClicked();
          },
          this);
      }
    },
    
    /* Additional methods */
    
    addEntityTools : function (options)
    {
      options.tools = options.tools || new Array();
      
      if (options.isRoot)
      {
        options.tools.push(
          {
            type : 'refresh',
            tooltip : 'Refresh entity',
            handler : this.refreshEntity,
            scope : this
          }
        );
      }
      else
      {
        if (options.hideRemoveButton == null || !options.hideRemoveButton)
        {
          options.tools.push(
            {
              type : 'minus',
              tooltip : 'Remove this item',
              handler : this.deleteEntity,
              scope : this
            }
          );
        }
        
        if (options.hideAddButton == null || !options.hideAddButton)
        {
          options.tools.push(
            {
              type : 'plus',
              tooltip : 'Add a new item',
              handler : this.addEntity,
              scope : this
            }
          );
        }
      }
    },
    
    // Overriden by WallPanelEmptySummary
    bodyClicked : function ()
    {
      this.showEntityColumn(this.clickToEdit, this.entityId, this);
    },
    
    showEntityColumn : function (clickToEdit, entityId, panel)
    {
      var columnMgr = Ext.getCmp('columnmgr');
      columnMgr.showEntity(clickToEdit, entityId, panel, 1);
    },
    
    refreshEntity : function ()
    {
      var columnMgr = Ext.getCmp('columnmgr');
      columnMgr.refreshColumn(this.ownerCt);
    },
    
    showNewEntityColumn : function (clickToEdit, entityId, panel)
    {
      var columnMgr = Ext.getCmp('columnmgr');
      columnMgr.showEntity(clickToEdit, entityId, panel, 3);
    },
    
    deleteEntity : function ()
    {
      this.setLoading();
      
      Ext.Ajax.request(
        {
          url : 'proxy/collection/delete',
          params :
          {
            parentEntity : this.ownerCt.parentEntity,
            deleteEntity : this.entityId,
            lambda : this.lambda
          },
          success : function (response, options)
          {
            this.getEl().slideOut();
          },
          failure : function ()
          {
            this.setLoading(false);
            Epsitec.Cresus.Core.Static.ErrorHandler.handleError(response);
          },
          scope : this
        }
      );
    },
    
    addEntity : function ()
    {
      this.setLoading();
      Ext.Ajax.request(
        {
          url : 'proxy/collection/create',
          params :
          {
            parentEntity : this.ownerCt.parentEntity,
            entityType : this.entityType,
            lambda : this.lambda
          },
          success : function (response, options)
          {
            this.setLoading(false);
            
            try
            {
              var json = Ext.decode(response.responseText);
            }
            catch (err)
            {
              options.failure.apply(arguments);
              return;
            }
            
            this.showNewEntityColumn(this.clickToEdit, json.content, this);
            
          },
          failure : function ()
          {
            this.setLoading(false);
            Epsitec.Cresus.Core.Static.ErrorHandler.handleError(response);
          },
          scope : this
        }
      );
      
    },
    
    isSelected : function ()
    {
      return this.selected != null ? this.selected : false;
    },
    
    setSelected : function ()
    {
      this.selected = true;
      this.addCls(this.selectedPanelCls);
    },
    
    setUnSelected : function ()
    {
      this.selected = false;
      this.removeCls(this.selectedPanelCls);
    }
  }
);
 