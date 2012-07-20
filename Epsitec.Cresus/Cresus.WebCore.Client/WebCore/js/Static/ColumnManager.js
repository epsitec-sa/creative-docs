Ext.define('Epsitec.Cresus.Core.Static.ColumnManager', {
  extend: 'Ext.panel.Panel',

  /* Config */

  layout: 'border',

  /* Properties */

  leftList: null,
  rightPanel: null,
  columns: [],
  selectedPanels: [],

  /* Constructor */

  constructor: function(options) {
    this.callParent(arguments);

    var database = options.database;

    this.title = database.Title;

    this.leftList = Ext.create('Epsitec.Cresus.Core.Static.EntityListPanel', {
      databaseName: database.DatabaseName,
      region: 'west',
      margin: 5,
      width: 250,
      columnManager: this,
      columnId: null
    });

    this.rightPanel = Ext.create('Ext.panel.Panel', {
      region: 'center',
      margin: '5',
      layout: {
        type: 'table',
        tdAttrs: {
          valign: 'top'
        }
      },
      defaults: {
        width: 350
      },
      autoScroll: true
    });

    this.add([this.leftList, this.rightPanel]);

    return this;
  },

  /* Additional methods */

  clearColumns: function() {
    this.removeColumnsFromIndex(0);
  },

  // The arguments parentPanel and callbackQueue are optional.
  addEntityColumn: function(
      viewMode,
      viewId,
      entityId,
      parentPanel,
      callbackQueue) {
    parentPanel = Epsitec.Cresus.Core.Static.Tools.getValueOrNull(parentPanel);
    callbackQueue = Epsitec.Cresus.Core.Static.Tools.getValueOrDefault(
        callbackQueue, Epsitec.Cresus.Core.Static.CallbackQueue.empty()
        );

    var newCallbackQueue = Epsitec.Cresus.Core.Static.CallbackQueue.empty();

    if (parentPanel !== null) {
      var parentColumnId = parentPanel.entityPanel.columnId;

      this.removeColumnsFromIndex(parentColumnId + 1);

      newCallbackQueue = newCallbackQueue.enqueueCallback(
          function() {
            this.selectPanel(parentColumnId, parentPanel);
          },
          this
          );
    }

    newCallbackQueue = newCallbackQueue.enqueueCallback(
        function(config) {
          this.addNewColumn(config);
        },
        this
        );

    newCallbackQueue = newCallbackQueue.merge(callbackQueue);

    this.execute(viewMode, viewId, entityId, parentPanel, newCallbackQueue);
  },

  // The arguments callbackQueue is optional.
  refreshColumns: function(firstColumnId, lastColumnId, callbackQueue) {
    callbackQueue = Epsitec.Cresus.Core.Static.Tools.getValueOrDefault(
        callbackQueue, Epsitec.Cresus.Core.Static.CallbackQueue.empty()
        );

    var configArray = [];
    var configArrayCount = 0;

    var callbackQueueCreator = function(index) {
      return Epsitec.Cresus.Core.Static.CallbackQueue.create(
          function(config) {
            configArrayCount += 1;
            configArray[index] = config;

            if (configArrayCount === lastColumnId - firstColumnId + 1) {
              this.replaceExistingColumns(
                  firstColumnId, lastColumnId, configArray
              );

              callbackQueue.execute(configArray);
            }
          },
          this
      );
    };

    for (var i = firstColumnId; i <= lastColumnId; i += 1) {
      var columnPanel = this.columns[i];
      var viewMode = columnPanel.viewMode;
      var viewId = columnPanel.viewId;
      var entityId = columnPanel.entityId;

      var index = i - firstColumnId;
      var newCallbackQueue = callbackQueueCreator.call(this, index);

      this.execute(viewMode, viewId, entityId, columnPanel, newCallbackQueue);
    }
  },

  execute: function(viewMode, viewId, entityId, loadingPanel, callbackQueue) {
    if (loadingPanel !== null) {
      loadingPanel.setLoading();
    }

    Ext.Ajax.request({
      url: 'proxy/layout/' + viewMode + '/' + viewId + '/' + entityId,
      success: function(response, options) {
        if (loadingPanel !== null) {
          loadingPanel.setLoading(false);
        }

        var config;

        try {
          config = Ext.decode(response.responseText);
        }
        catch (err) {
          options.failure.apply(arguments);
          return;
        }

        var callbackArguments = [config.content];

        callbackQueue.execute(callbackArguments);
      },
      failure: function(response, options) {
        if (loadingPanel !== null) {
          loadingPanel.setLoading(false);
        }

        Epsitec.Cresus.Core.Static.ErrorHandler.handleError(response);
      },
      scope: this
    });
  },

  addNewColumn: function(config) {
    config.columnId = this.columns.length;
    config.columnManager = this;

    var column = Ext.create('Epsitec.Cresus.Core.Static.EntityPanel', config);

    this.addExistingColumn(column);

    // Scroll all the way to the right, in case there are more panel than the
    // screen is able to show
    var dom = this.rightPanel.getEl().child('.x-panel-body').dom;
    dom.scrollLeft = 1000000;
    dom.scrollTop = 0;
  },

  addExistingColumn: function(column) {
    this.rightPanel.add(column);

    this.columns.push(column);
    this.selectedPanels.push(null);
  },

  replaceExistingColumns: function(firstColumnId, lastColumnId, configArray) {
    // This part does awful things.
    //
    // The table layout used to show the columns does not allow to replace some
    // column in the middle of the table. The only solution I've found is the
    // following
    // 1) Remove all the columns to the right of the ones that we want to
    //    replace but keep them around for later reuse.
    // 2) Remove and delete the columns that we want to replace.
    // 3) Add the new version of the columns that we wanted to replace.
    // 4) Add the columns that were to the right of the columns that we have
    //    replaced.
    //
    // We also have to remember the selected state of one panel. To do so, we
    // remember the id of the entity that is selected in each column and after
    // the replacement, we iterate over the columns to find the panel that has
    // the same entity id, and we select it.
    // This solution has a drawback. If we have a list and the entity that is
    // selected appears more that once in the list we will select the first one,
    // even if another one was selected.
    //
    // Since the layout is completely rebuilt, we also have to remember the
    // scroll.

    // Used in the for loops to iterate.
    var i;

    // Remember the scroll position.
    var dom = this.rightPanel.getEl().child('.x-panel-body').dom;
    var scrollLeft = dom.scrollLeft;
    var scrollTop = dom.scrollTop;

    // Copy the current columns and the selection state.
    var clonedColumns = Ext.Array.clone(this.columns);
    var selectedEntityIds = this.selectedPanels.map(
        function(p) { return p === null ? null : p.entityId; }
        );

    // Remove the columns at the right ot the ones that we want to replace, but
    // don't delete them. We need them later.
    this.removeColumnsFromIndex(lastColumnId + 1, false);

    // Removes the columns that we will replace later on and deletes thems as we
    // won't need them later.
    for (i = lastColumnId; i >= firstColumnId; i -= 1) {
      this.removeColumn(i, true);
    }

    // Adds the new version of the columns that we want to replace.
    for (i = firstColumnId; i <= lastColumnId; i += 1) {
      var index = i - firstColumnId;
      var config = configArray[index];

      config.columnId = i;
      config.columnManager = this;

      this.addExistingColumn(
          Ext.create('Epsitec.Cresus.Core.Static.EntityPanel', config)
      );
    }

    var nbColumns = clonedColumns.length;

    // Add the column that we removed before so that they are displayed again.
    for (i = lastColumnId + 1; i < nbColumns; i += 1) {
      var column = clonedColumns[i];

      this.addExistingColumn(column);
    }

    // Reapply the selection on the columns that we have just added.
    for (i = firstColumnId; i < nbColumns; i += 1) {
      var selectedEntityId = selectedEntityIds[i];

      if (selectedEntityId !== null) {
        this.selectEntity(i, selectedEntityId);
      }
    }

    // Reapply the scroll position.
    dom.scrollLeft = scrollLeft;
    dom.scrollTop = scrollTop;
  },

  removeColumnsFromIndex: function(index, autoDestroy) {
    for (var i = this.columns.length - 1; i >= index; i -= 1) {
      this.removeColumn(i, autoDestroy);
    }
  },

  removeColumn: function(index, autoDestroy) {
    var column = this.columns[index];

    this.rightPanel.remove(column, autoDestroy);

    this.columns.splice(index, 1);
    this.selectedPanels.splice(index, 1);
  },

  selectEntity: function(columnId, entityId) {
    var column = this.columns[columnId];
    var panels = column.items.items;

    for (var i = 0; i < panels.length; i += 1) {
      var panel = panels[i];

      if (panel.entityId === entityId) {
        this.selectPanel(columnId, panel);

        break;
      }
    }
  },

  selectPanel: function(columnId, panel) {
    var oldPanel = this.getSelectedPanel(columnId);
    if (oldPanel !== null) {
      oldPanel.unselect();
    }

    panel.select();

    this.selectedPanels[columnId] = panel;
  },

  getSelectedEntity: function(columnId) {
    var selectedPanel = this.getSelectedPanel(columnId);

    return selectedPanel === null ?
        null : selectedPanel.entityId;
  },

  getSelectedPanel: function(columnId) {
    return this.selectedPanels[columnId] || null;
  }
});
