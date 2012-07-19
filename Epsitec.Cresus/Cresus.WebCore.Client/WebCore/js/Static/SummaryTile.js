Ext.define('Epsitec.Cresus.Core.Static.SummaryTile',
  {
    extend : 'Ext.Panel',
    alias : 'widget.summarytile',
    
    /* Config */
    margin : '0 0 5 0',
    bodyCls : 'summary',
    overCls : 'over-summary',
    
    /* Properties */
    entityId : null,
    isRoot : false,
    subViewMode : 'edition',
    subViewId : 'null',
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
            // We don't call the function directly because ExtJs calls the handler with some arguments
            // that are not compatible witht the refreshEntity signature.
            handler : function () { this.refreshEntity(false); },
            scope : this
          }
        );
      }
    },
    
    // Overriden by EmptySummaryTile
    bodyClicked : function ()
    {
      if (this.autoCreatorId !== null)
      {
        this.autoCreateNullEntity();
      }
      else
      {
        this.showEntityColumn(this.subViewMode, this.subViewId, this.entityId);
      }
    },

    showEntityColumn : function (subViewMode, subViewId, entityId, callbackQueue)
    {
      var columnManager = this.ownerCt.columnManager;
      columnManager.addEntityColumn(subViewMode, subViewId, entityId, this, callbackQueue);
    },
    
    showEntityColumnAndRefresh : function (subViewMode, subViewId, entityId, callbackQueue)
    {
      var newCallbackQueue = Epsitec.Cresus.Core.Static.CallbackQueue.create
      (
        function ()
        {
          this.refreshEntity(true, callbackQueue);
        },
        this
      );
      
      this.showEntityColumn(subViewMode, subViewId, entityId, newCallbackQueue);
    },
    
    refreshEntity : function (refreshAll, callbackQueue)
    {
      var firstColumnId = refreshAll ? 0 : this.ownerCt.columnId;
      var lastColumnId = this.ownerCt.columnId;
      
      var columnManager = this.ownerCt.columnManager;
      columnManager.refreshColumns(firstColumnId, lastColumnId, callbackQueue);
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
            entityId : this.ownerCt.entityId,
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
              this.showEntityColumnAndRefresh(this.subViewMode, this.subViewId, newEntityId);
            }
            else
            {
              this.showEntityColumn(this.subViewMode, this.subViewId, newEntityId);
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
 