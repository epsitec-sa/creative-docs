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
    lambdaId : null,
    entityType : null,
    isRoot : false,
    subViewControllerMode : 'edition',
    subViewControllerSubTypeId : 'null',
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
        this.showEntityColumn(this.subViewControllerMode, this.subViewControllerSubTypeId, this.entityId, this);
    },

    showEntityColumn: function (subViewControllerMode, subViewControllerSubTypeId, entityId, panel)
    {
      var columnMgr = Ext.getCmp('columnmgr');
      columnMgr.showEntity(subViewControllerMode, subViewControllerSubTypeId, entityId, panel, 1);
    },
    
    refreshEntity : function ()
    {
      var columnMgr = Ext.getCmp('columnmgr');
      columnMgr.refreshColumn(this.ownerCt);
    },

    showNewEntityColumn: function (subViewControllerMode, subViewControllerSubTypeId, entityId, panel)
    {
      var columnMgr = Ext.getCmp('columnmgr');
      columnMgr.showEntity(subViewControllerMode, subViewControllerSubTypeId, entityId, panel, 3);
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
            lambdaId : this.lambdaId
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
            lambdaId : this.lambdaId
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

            this.showNewEntityColumn(this.subViewControllerMode, this.subViewControllerSubTypeId, json.content, this);
            
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
 