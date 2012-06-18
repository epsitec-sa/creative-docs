Ext.define('Epsitec.Cresus.Core.Static.WallPanelCollectionSummary',
  {
    extend : 'Epsitec.Cresus.Core.Static.WallPanelSummary',
    alias : 'widget.collectionsummary',
    
    /* Properties */
    propertyAccessorId : null,
    entityType : null,
    hideRemoveButton : false,
    hideAddButton : false,
    
    /* Constructor */
    constructor : function (o)
    {
      var options = o || {};
      this.addPlusMinusButtons(options);
      
      this.callParent(new Array(options));
      return this;
    },
    
    /* Additional methods */ 
    addPlusMinusButtons : function (options)
    {
      options.tools = options.tools || new Array();
      
      if (!options.hideAddButton)
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
      
      if (!options.hideRemoveButton)
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
            propertyAccessorId : this.propertyAccessorId
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
            propertyAccessorId : this.propertyAccessorId
          },
          success : function (response, options)
          {
            this.setLoading(false);
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
    
  }
);
 