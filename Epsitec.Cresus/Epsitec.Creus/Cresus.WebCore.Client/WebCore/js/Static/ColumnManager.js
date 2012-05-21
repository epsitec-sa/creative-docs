Ext.define('Epsitec.Cresus.Core.Static.ColumnManager',
  {
    extend : 'Ext.panel.Panel',
    id : 'columnmgr',
    
    /* Config */
    title : "Entity edition",
    layout :
    {
      type : 'table',
      tdAttrs :
      {
        valign : 'top'
      }
    },
    defaults :
    {
      width : 350
    },
    autoScroll : true,
    
    /* Properties */
    selectedPanels : null,
    selectedEntities : null,
    
    /* Constructor */
    constructor : function (options)
    {
      this.selectedPanels = new Array();
      this.selectedEntities = new Array();
      this.callParent(arguments);
      return this;
    },
    
    /* Additional methods */
    
    /*
     * ColumnModes:
     *  0: Clear the screen and add the new column
     *  1: Clear coulmns on the right of this one
     *  2: Replace the current column
     *  3: Show a column for a newly created entity, and set the "selected" to the new entity id
     */
    showEntity : function (controllerMode, controllerSubTypeId, entityId, panel, columnMode)
    {
      panel.setLoading();
      
      Ext.Ajax.request(
        {
          url : 'proxy/layout/' + controllerMode + '/' + controllerSubTypeId + '/' + entityId,
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
            config = config.content;
            
            panel.setLoading(false);
            
            switch (columnMode)
            {
            case 1:
              this.addColumnFromColumn(config, panel);
              break;
            case 2:
              this.replaceColumn(config, panel);
              break;
            case 3:
              this.showNewEntityColumn(config, panel, entityId);
              break;
            case 0:
            default:
              this.addNewColumn(config);
              break;
            }
            
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
    
    refreshColumn : function (panel)
    {
      this.showEntity(panel.controllerMode, panel.controllerSubTypeId, panel.parentEntity, panel, 2);
    },
    
    refreshLeftColumn : function (panel)
    {
      var columnId = panel.columnId - 1;
      var leftPanel = this.selectedPanels[columnId];
      if (leftPanel != null)
      {
        this.refreshColumn(leftPanel.ownerCt);
      }
    },
    
    // Clear all columns and add a new one
    addNewColumn : function (config)
    {
      config.columnId = 0;
      
      this.clearAllColumns();
      this.addColumn(config);
    },
    
    // Add a column to the list, but removes the ones to the right
    addColumnFromColumn : function (config, panel)
    {
      var oldId = panel.ownerCt.columnId;
      var newId = oldId + 1;
      config.columnId = newId;
      
      this.clearColumns(newId);
      this.addColumn(config);
      
      this.setSelectedPanel(panel, oldId);
    },
    
    // Replace the current column
    replaceColumn : function (config, panel)
    {
      /*
      
      This part does awful things.
      The table layout used to show the columns does not allow to replace a column
      in the middle of the table.
      The only solution I've found is to remove all the columns then to re-add them
      using the correct order.
      When re-adding, we replace the old column by the new one.
      
      Plus, we have to remember the "selected" state of one panel.
      To do so, we remember the entity id of the currently selected panel,
      then after the refresh, we iterate over the new panel to find the panel
      that has the same entity id, and we "select" it.
      
      Since the layout is completely rebuild, we also have to remember the scroll.
      
       */
      
      var newId = panel.columnId;
      config.columnId = newId;
      
      // Create the new column
      var col = Ext.create('Epsitec.Cresus.Core.Static.ColumnPanel', config);
      
      // Remember scroll
      var dom = this.getEl().child('.x-panel-body').dom;
      var scrollLeft = dom.scrollLeft;
      var scrollTop = dom.scrollTop;
      
      // Copy the old children
      var items = Ext.Array.clone(this.items.items);
      
      // Remove the old children
      var length = this.items.items.length;
      for (var i = 0; i < length; ++i)
      {
        // Remove the children from the dom, but not from the memory,
        // except the refreshed panel that we can delete
        this.remove(this.items.items[0], i == newId);
      }
      
      // Re-add the children, with the new one
      for (var i = 0; i < items.length; ++i)
      {
        if (i == newId)
        {
          this.add(col);
        }
        else
        {
          var item = items[i];
          this.add(item);
        }
      }
      
      // Re-apply the selection
      var selectedEntityId = this.selectedEntities[newId];
      if (selectedEntityId != null)
      {
        Ext.Array.each(col.items.items, function (item)
          {
            if (item.entityId == selectedEntityId)
            {
              this.setSelectedPanel(item, newId);
            }
          }, this);
      }
      
      // Re-apply scroll
      dom.scrollLeft = scrollLeft;
      dom.scrollTop = scrollTop;
    },
    
    showNewEntityColumn : function (config, panel, entityId)
    {
      var oldId = panel.ownerCt.columnId;
      var newId = oldId + 1;
      config.columnId = newId;
      
      this.clearColumns(newId);
      this.addColumn(config);
      
      this.setSelectedEntity(oldId, panel, entityId);
      this.refreshColumn(panel.ownerCt);
    },
    
    // Add a column at the end
    addColumn : function (config)
    {
      var col = Ext.create('Epsitec.Cresus.Core.Static.ColumnPanel', config);
      this.add(col);
      
      // Scroll all the way to the right,
      // in case there are more panel than the screeen is able to show
      var dom = this.getEl().child('.x-panel-body').dom;
      dom.scrollLeft = 1000000;
      dom.scrollTop = 0;
    },
    
    // index: number of columns to keep
    clearColumns : function (index)
    {
      while (this.items.length > index)
      {
        var rem = this.items.items[this.items.length - 1];
        this.remove(rem);
      }
    },
    
    clearAllColumns : function ()
    {
      this.removeAll();
    },
    
    setSelectedPanel : function (panel, columnId)
    {
      var oldPanel = this.selectedPanels[columnId];
      if (oldPanel != null)
      {
        oldPanel.setUnSelected();
      }
      
      panel.setSelected();
      this.selectedPanels[columnId] = panel;
      this.selectedEntities[columnId] = panel.entityId;
    },
    
    setSelectedEntity : function (columnId, panel, entityId)
    {
      var oldPanel = this.selectedPanels[columnId];
      if (oldPanel != null)
      {
        oldPanel.setUnSelected();
      }
      
      this.selectedPanels[columnId] = panel;
      this.selectedEntities[columnId] = entityId;
    },
  }
);
 