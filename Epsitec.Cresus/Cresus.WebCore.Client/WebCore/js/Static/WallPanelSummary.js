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
    isRoot : false,
    subViewControllerMode : 'edition',
    subViewControllerSubTypeId : 'null',
    autoCreatorId : null,
    selectedPanelCls : 'selected-entity',
    
    /* Constructor */
    constructor : function (o)
    {
      var options = o || {};
      this.addRefreshButton(options);
      
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
    addRefreshButton : function (options)
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
    },
    
    // Overriden by WallPanelEmptySummary
    bodyClicked : function ()
    {
      if (this.autoCreatorId !== null)
      {
        this.autoCreateNullEntity();
      }
      else
      {
        this.showEntityColumn(this.subViewControllerMode, this.subViewControllerSubTypeId, this.entityId);
      }
    },

    showEntityColumn : function (subViewControllerMode, subViewControllerSubTypeId, entityId, callback, callbackContext)
    {
      var columnMgr = Ext.getCmp('columnmgr');
      columnMgr.addEntityColumn(subViewControllerMode, subViewControllerSubTypeId, entityId, this, callback, callbackContext);
    },
    
    showEntityColumnAndRefresh : function (subViewControllerMode, subViewControllerSubTypeId, entityId)
    {
      var callback = function()
      {
        this.refreshEntity();
      };
      var callbackContext = this;
      
      this.showEntityColumn(subViewControllerMode, subViewControllerSubTypeId, entityId, callback, callbackContext);
    },
    
    refreshEntity : function ()
    {
      var columnId = this.ownerCt.columnId;
      
      var columnMgr = Ext.getCmp('columnmgr');
      columnMgr.refreshColumn(columnId);
    },
    
    autoCreateNullEntity : function()
    {   
      this.setLoading();
      
      Ext.Ajax.request(
        {
          url : 'proxy/entity/autoCreate',
          method : "POST",
          params :
          {
            entityId : this.ownerCt.parentEntity,
            autoCreatorId : this.autoCreatorId,
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
            
            var newEntityId = json.content;
            
            // Here we first add the new column for the entity that we have created with the ajax
            // request. Then we refresh the current pannel where the user has clicked, in case the
            // new entity has some content that we should display. We don't do it in the inverse
            // order, because the refresh replaces the current instance by a new one, and then this
            // messes up the things when we want to add a new column with the current instance which
            // will have been removed from the UI at the time the callback will be called.
            
            if (this.entityId !== newEntityId)
            {
              this.showEntityColumnAndRefresh(this.subViewControllerMode, this.subViewControllerSubTypeId, newEntityId);
            }
            else
            {
              this.showEntityColumn(this.subViewControllerMode, this.subViewControllerSubTypeId, newEntityId);
            }
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
    
    select : function ()
    {
      this.addCls(this.selectedPanelCls);
    },
    
    unselect : function ()
    {
      this.removeCls(this.selectedPanelCls);
    }
  }
);
 