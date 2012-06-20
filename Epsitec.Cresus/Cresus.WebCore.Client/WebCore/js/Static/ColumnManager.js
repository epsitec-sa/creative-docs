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
    columns : [],
    selectedPanels : [],
    selectedEntities : [],
    
    /* Additional methods */
    clearColumns : function()
    {
      this.removeColumnsFromIndex(0)
    },
    
    // The arguments parentPanel, callback and callbackContext are optional.
    addEntityColumn : function(controllerMode, controllerSubTypeId, entityId, parentPanel, callback, callbackContext)
    {
      parentPanel = Epsitec.Cresus.Core.Static.Tools.getValueOrNull(parentPanel);
       
      var callback1 = function (config)
      {
        this.addNewColumn(config)
        
        Epsitec.Cresus.Core.Static.Tools.doCallback(callback, callbackContext);
      };
      callbackContext1 = this;
      
      var callback2 = null;
      var callbackContext2 = null;
      
      if (parentPanel === null)
      {
        callback2 = callback1;
        callbackContext2 = callbackContext1;
      }
      else
      {
        var parentColumnId = parentPanel.ownerCt.columnId;
        
        this.removeColumnsFromIndex(parentColumnId + 1);
        
        callback2 = function (config)
        {
          this.selectPanel(parentColumnId, parentPanel);
          
          Epsitec.Cresus.Core.Static.Tools.doCallback(callback1, callbackContext1, [config]);
        };
        callbackContext2 = this;
      }
      
      this.execute(controllerMode, controllerSubTypeId, entityId, parentPanel, callback2, callbackContext2);
    },
    
    refreshColumn : function(columnId, callback, callbackContext)
    {
      var columnPanel = this.columns[columnId];
      
      var callback1 = function (config)
      {
        this.replaceExistingColumn(columnId, config)
        
        Epsitec.Cresus.Core.Static.Tools.doCallback(callback, callbackContext, [config]);
      };
      var callbackContext1 = this;
      
      var controllerMode = columnPanel.controllerMode;
      var controllerSubTypeId = columnPanel.controllerSubTypeId;
      var entityId = columnPanel.parentEntity;
      
      this.execute(controllerMode, controllerSubTypeId, entityId, columnPanel, callback1, callbackContext1);
    },
    
    execute : function(controllerMode, controllerSubTypeId, entityId, loadingPanel, callback, callbackContext)
    {
      if (loadingPanel !== null)
      {
        loadingPanel.setLoading();
      }
      
      Ext.Ajax.request(
        {
          url : 'proxy/layout/' + controllerMode + '/' + controllerSubTypeId + '/' + entityId,
          success : function (response, options)
          {
            if (loadingPanel !== null)
            {
              loadingPanel.setLoading(false);
            }
            
            try
            {
              var config = Ext.decode(response.responseText);
            }
            catch (err)
            {
              options.failure.apply(arguments);
              return;
            }
            
            var callbackArguments = [ config.content ];
            
            Epsitec.Cresus.Core.Static.Tools.doCallback(callback, callbackContext, callbackArguments);
          },
          failure : function (response, options)
          {
            if (loadingPanel !== null)
            {
              loadingPanel.setLoading(false);
            }
            
            Epsitec.Cresus.Core.Static.ErrorHandler.handleError(response);
          },
          scope : this
        }
      );
    },
    
    addNewColumn : function(config)
    {
      config.columnId = this.columns.length;
      
      var column = Ext.create('Epsitec.Cresus.Core.Static.ColumnPanel', config);
      
      this.addExistingColumn(column)
      
      // Scroll all the way to the right, in case there are more panel than the screeen
      // is able to show
      var dom = this.getEl().child('.x-panel-body').dom;
      dom.scrollLeft = 1000000;
      dom.scrollTop = 0;
    },
    
    addExistingColumn : function(column)
    {
      this.add(column);
      
      this.columns.push(column);
      this.selectedPanels.push(null);
      this.selectedEntities.push(null);
    },
    
    replaceExistingColumn : function (columnId, config)
    {
      // This part does awful things.
      //
      // The table layout used to show the columns does not allow to replace a column
      // in the middle of the table. The only solution I've found is the following
      // 1) Remove all the columns to the right of the one that we want to refresh but
      //    keep them around for reuse.
      // 2) Remove and delete the column that we want to refresh.
      // 3) Add the refreshed version of the column that we wanted to refresh.
      // 4) Add the columns that were to the right of the column that we have refreshed.
      //
      // We also have to remember the selected state of one panel. To do so, we remember
      // the id of the entity that is selected in each column and after the refresh,
      // we iterate over the columns to find the panel that has the same entity id,
      // and we select it.
      // This solution has a drawback. If we have a list and the entity that is selected
      // appears more that once in the list we will select the first one, even if another
      // one was selected.^
      //
      // Since the layout is completely rebuilt, we also have to remember the scroll.
      
      // Remember the scroll position.
      var dom = this.getEl().child('.x-panel-body').dom;
      var scrollLeft = dom.scrollLeft;
      var scrollTop = dom.scrollTop;
      
      // Copy the current columns and the selection state.
      var clonedColumns = Ext.Array.clone(this.columns);
      var clonedSelectedEntityies = Ext.Array.clone(this.selectedEntities);
      
      // Remove the columns at the right ot the one we want to refresh, but don't
      // delete them from memory. We need them later.
      this.removeColumnsFromIndex(columnId + 1, false);
      
      
      // Replaces the column that we want to refresh by its new version.
      this.removeColumn(columnId, true);
      
      config.columnId = columnId;
      var refreshedColumn = Ext.create('Epsitec.Cresus.Core.Static.ColumnPanel', config);
      
      this.addExistingColumn(refreshedColumn);
      
      
      var nbColumns = clonedColumns.length;
      
      // Add the column that we removed before to that they are displayed again.
      for (var i = columnId + 1; i < nbColumns; i++)
      {
        var column = clonedColumns[i];
        
        this.addExistingColumn(column);
      }
      
      // Reapply the selection on the columns that we have just added.
      for (var i = columnId; i < nbColumns; i++)
      {
        var selectedEntityId = clonedSelectedEntityies[i];
        
        if (selectedEntityId !== null)
        {
          this.selectEntity(i, selectedEntityId);
        }
      }
     
      // Reapply the scroll position.
      dom.scrollLeft = scrollLeft;
      dom.scrollTop = scrollTop;
    },
    
    removeColumnsFromIndex : function (index, autoDestroy)
    {
      for (var i = this.columns.length - 1; i >= index; i--)
      {
        this.removeColumn(i, autoDestroy);
      }
    },
    
    removeColumn : function (index, autoDestroy)
    {
      var column = this.columns[index];
      
      this.remove(column, autoDestroy);
      
      this.columns.splice(index, 1);
      this.selectedPanels.splice(index, 1);
      this.selectedEntities.splice(index, 1);
    },
    
    selectEntity : function (columnId, entityId)
    {
      var column = this.columns[columnId];
      var panels = column.items.items;
      
      for (var i = 0; i < panels.length; ++i)
      {
        var panel = panels[i];
        
        if (panel.entityId === entityId)
        {
          this.selectPanel(columnId, panel);
          
          break;
        }
      }
    },
    
    selectPanel : function (columnId, panel)
    {
      var oldPanel = this.selectedPanels[columnId];
      if (oldPanel != null)
      {
        oldPanel.unselect();
      }
      
      panel.select();
      
      this.selectedPanels[columnId] = panel;
      this.selectedEntities[columnId] = panel.entityId;
    },
  }
);
 