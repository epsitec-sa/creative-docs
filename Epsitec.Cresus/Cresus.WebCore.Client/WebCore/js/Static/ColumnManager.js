Ext.define('Epsitec.Cresus.Core.Static.ColumnManager',
  {
    extend : 'Ext.panel.Panel',
    id : 'columnmanager',
    
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
    
    /* Additional methods */
    clearColumns : function()
    {
      this.removeColumnsFromIndex(0)
    },
    
    // The arguments parentPanel and callbackQueue.
    addEntityColumn : function(viewMode, viewId, entityId, parentPanel, callbackQueue)
    {
      parentPanel = Epsitec.Cresus.Core.Static.Tools.getValueOrNull(parentPanel);
      callbackQueue = Epsitec.Cresus.Core.Static.Tools.getValueOrDefault(callbackQueue, Epsitec.Cresus.Core.Static.CallbackQueue.empty());
      
      var newCallbackQueue = Epsitec.Cresus.Core.Static.CallbackQueue.empty();
      
      if (parentPanel !== null)
      {
        var parentColumnId = parentPanel.ownerCt.columnId;
        
        this.removeColumnsFromIndex(parentColumnId + 1);
        
        newCallbackQueue = newCallbackQueue.enqueueCallback
        (
          function (config)
          {
            this.selectPanel(parentColumnId, parentPanel);
          },
          this
        );
      }
      
      newCallbackQueue = newCallbackQueue.enqueueCallback
      (
        function (config)
        {
          this.addNewColumn(config)
        },
        this
      );
      
      newCallbackQueue = newCallbackQueue.merge(callbackQueue);
       
      this.execute(viewMode, viewId, entityId, parentPanel, newCallbackQueue);
    },
    
    // The arguments callbackQueue is optional.
    refreshColumns : function(firstColumnId, lastColumnId, callbackQueue)
    {
      callbackQueue = Epsitec.Cresus.Core.Static.Tools.getValueOrDefault(callbackQueue, Epsitec.Cresus.Core.Static.CallbackQueue.empty());
      
      var configArray = [];
      var configArrayCount = 0;
      
      var callbackQueueCreator = function (index)
      {
        return Epsitec.Cresus.Core.Static.CallbackQueue.create
        (
          function (config)
          {
            configArrayCount++;
            configArray[index] = config;
            
            if (configArrayCount == lastColumnId - firstColumnId + 1)
            {
              this.replaceExistingColumns(firstColumnId, lastColumnId, configArray)
              
              callbackQueue.execute(configArray);
            }
          },
          this
        );
      };
      
      for (var i = firstColumnId; i <= lastColumnId; i++)
      {
        var columnPanel = this.columns[i];
        var viewMode = columnPanel.viewMode;
        var viewId = columnPanel.viewId;
        var entityId = columnPanel.entityId;
        
        var index = i - firstColumnId;        
        var newCallbackQueue = callbackQueueCreator.call(this, index);
        
        this.execute(viewMode, viewId, entityId, columnPanel, newCallbackQueue); 
      }
    },
    
    execute : function(viewMode, viewId, entityId, loadingPanel, callbackQueue)
    {
      if (loadingPanel !== null)
      {
        loadingPanel.setLoading();
      }
      
      Ext.Ajax.request(
        {
          url : 'proxy/layout/' + viewMode + '/' + viewId + '/' + entityId,
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
            
            callbackQueue.execute(callbackArguments);
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
      
      var column = Ext.create('Epsitec.Cresus.Core.Static.EntityPanel', config);
      
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
    },
    
    replaceExistingColumns : function (firstColumnId, lastColumnId, configArray)
    {
      // This part does awful things.
      //
      // The table layout used to show the columns does not allow to replace some column
      // in the middle of the table. The only solution I've found is the following
      // 1) Remove all the columns to the right of the ones that we want to replace but
      //    keep them around for later reuse.
      // 2) Remove and delete the columns that we want to replace.
      // 3) Add the new version of the columns that we wanted to replace.
      // 4) Add the columns that were to the right of the columns that we have replaced.
      //
      // We also have to remember the selected state of one panel. To do so, we remember
      // the id of the entity that is selected in each column and after the replacement,
      // we iterate over the columns to find the panel that has the same entity id,
      // and we select it.
      // This solution has a drawback. If we have a list and the entity that is selected
      // appears more that once in the list we will select the first one, even if another
      // one was selected.
      //
      // Since the layout is completely rebuilt, we also have to remember the scroll.
      
      // Remember the scroll position.
      var dom = this.getEl().child('.x-panel-body').dom;
      var scrollLeft = dom.scrollLeft;
      var scrollTop = dom.scrollTop;
      
      // Copy the current columns and the selection state.
      var clonedColumns = Ext.Array.clone(this.columns);
      var selectedEntityIds = this.selectedPanels.map
      (
        function (p) { return p == null ? null : p.entityId; }
      );
      
      // Remove the columns at the right ot the ones that we want to replace, but don't
      // delete them. We need them later.
      this.removeColumnsFromIndex(lastColumnId + 1, false);
      
      // Removes the columns that we will replace later on and deletes thems as we won't
      // need them later.
      for (var i = lastColumnId; i >= firstColumnId; i--)
      {
        this.removeColumn(i, true);
      }
      
      // Adds the new version of the columns that we want to replace.
      for (var i = firstColumnId; i <= lastColumnId; i++)
      {
        var index = i - firstColumnId;
        var config = configArray[index];
        
        config.columnId = i;
        var column = Ext.create('Epsitec.Cresus.Core.Static.EntityPanel', config);
        
        this.addExistingColumn(column);
      }
      
      var nbColumns = clonedColumns.length;
      
      // Add the column that we removed before so that they are displayed again.
      for (var i = lastColumnId + 1; i < nbColumns; i++)
      {
        var column = clonedColumns[i];
        
        this.addExistingColumn(column);
      }
      
      // Reapply the selection on the columns that we have just added.
      for (var i = firstColumnId; i < nbColumns; i++)
      {
        var selectedEntityId = selectedEntityIds[i];
        
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
      var oldPanel = this.getSelectedPanel(columnId);
      if (oldPanel != null)
      {
        oldPanel.unselect();
      }
      
      panel.select();
      
      this.selectedPanels[columnId] = panel;
    },
    
    getSelectedEntity : function (columnId)
    {
      var selectedPanel = this.getSelectedPanel(columnId);
      
      return selectedPanel == null
        ? null
        : selectedPanel.entityId;
    },
    
    getSelectedPanel : function (columnId)
    {
      return this.selectedPanels[columnId];
    },
  }
);
 