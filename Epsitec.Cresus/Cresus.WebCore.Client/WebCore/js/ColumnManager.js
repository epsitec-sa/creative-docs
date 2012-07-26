Ext.define('Epsitec.cresus.webcore.ColumnManager', {
  extend: 'Ext.panel.Panel',
  alternateClassName: ['Epsitec.ColumnManager'],

  /* Config */

  layout: 'border',

  /* Properties */

  leftList: null,
  rightPanel: null,
  columns: null,

  /* Constructor */

  constructor: function(options) {
    this.callParent(arguments);

    this.columns = [];

    var database = options.database;

    this.title = database.Title;

    this.leftList = Ext.create('Epsitec.LeftEntityListPanel', {
      databaseName: database.DatabaseName,
      region: 'west',
      margin: 5,
      width: 250,
      columnManager: this
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

  // The arguments parentColumn and callbackQueue are optional.
  addEntityColumn: function(
      viewMode,
      viewId,
      entityId,
      parentColumn,
      callbackQueue) {
    parentColumn = Epsitec.Tools.getValueOrNull(parentColumn);
    callbackQueue = Epsitec.Tools.getValueOrNull(callbackQueue);

    if (parentColumn !== null) {
      this.removeRightColumns(parentColumn);
    }

    var newCallbackQueue = Epsitec.CallbackQueue.create(
        function(config) { this.addNewColumn(config); },
        this
        );

    if (callbackQueue !== null) {
      newCallbackQueue = newCallbackQueue.merge(callbackQueue);
    }

    this.execute(viewMode, viewId, entityId, parentColumn, newCallbackQueue);
  },

  refreshColumn: function(column) {
    var columnId = column.columnId;
    this.refreshColumns(columnId, columnId);
  },

  refreshColumnsToLeft: function(column, includeColumn) {
    var columnId = column.columnId;
    if (!includeColumn) {
      columnId -= 1;
    }
    this.refreshColumns(0, columnId);
  },

  refreshColumns: function(firstColumnId, lastColumnId) {
    var configArray = [];
    var configArrayCount = 0;

    var callbackQueueCreator = function(index) {
      return Epsitec.CallbackQueue.create(
          function(config) {
            configArrayCount += 1;
            configArray[index] = config;

            if (configArrayCount === lastColumnId - firstColumnId + 1) {
              this.replaceExistingColumns(
                  firstColumnId, lastColumnId, configArray
              );
            }
          },
          this
      );
    };

    for (var i = firstColumnId; i <= lastColumnId; i += 1) {
      var column = this.columns[i];
      var viewMode = column.viewMode;
      var viewId = column.viewId;
      var entityId = column.entityId;

      var index = i - firstColumnId;
      var callbackQueue = callbackQueueCreator.call(this, index);

      this.execute(viewMode, viewId, entityId, column, callbackQueue);
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
          options.failure.apply(this, arguments);
          return;
        }

        var callbackArguments = [config.content];

        callbackQueue.execute(callbackArguments);
      },
      failure: function(response, options) {
        if (loadingPanel !== null) {
          loadingPanel.setLoading(false);
        }

        Epsitec.ErrorHandler.handleError(response);
      },
      scope: this
    });
  },

  addNewColumn: function(config) {
    config.columnId = this.columns.length;
    config.columnManager = this;

    var column = Ext.create('Epsitec.EntityPanel', config);

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
  },

  replaceExistingColumns: function(firstColumnId, lastColumnId, configArray) {
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
    // We have to remember the state of the columns. so we can re-apply it once
    // they are replaced.
    //
    // We have to remember the scroll, to re-apply it once the layout has been
    // rebuilt.

    // Used in the for loops to iterate.
    var i;

    // Remember the scroll position.
    var dom = this.rightPanel.getEl().child('.x-panel-body').dom;
    var scrollLeft = dom.scrollLeft;
    var scrollTop = dom.scrollTop;

    // Copy the current columns.
    var clonedColumns = Ext.Array.clone(this.columns);

    // Save the column state.
    var columnStates = [];
    for (i = firstColumnId; i <= lastColumnId; i += 1) {
      columnStates.push(this.columns[i].getState());
    }

    // Remove the columns at the right ot the ones that we want to replace, but
    // don't delete them as we need them later.
    this.removeColumns(lastColumnId + 1, this.columns.length - 1, false);

    // Removes the columns that we will replace and deletes them.
    this.removeColumns(firstColumnId, lastColumnId, true);

    // Replace the columns with their new version.
    for (i = firstColumnId; i <= lastColumnId; i += 1) {
      var index = i - firstColumnId;
      var config = configArray[index];

      config.columnId = i;
      config.columnManager = this;

      this.addExistingColumn(Ext.create('Epsitec.EntityPanel', config));
    }

    var nbColumns = clonedColumns.length;

    // Add the column that we removed before so that they are displayed again.
    for (i = lastColumnId + 1; i < nbColumns; i += 1) {
      var column = clonedColumns[i];

      this.addExistingColumn(column);
    }

    // Re-apply the state on the columns that we have just added.
    for (i = firstColumnId; i <= lastColumnId; i += 1) {
      this.columns[i].setState(columnStates[i - firstColumnId]);
    }

    // Re-apply the scroll position.
    dom.scrollLeft = scrollLeft;
    dom.scrollTop = scrollTop;
  },

  removeAllColumns: function() {
    this.removeColumns(0, this.columns.length - 1);
  },

  removeRightColumns: function(column) {
    this.removeColumns(column.columnId + 1, this.columns.length - 1, true);
  },

  removeColumns: function(startIndex, endIndex, autoDestroy) {
    for (var i = endIndex; i >= startIndex; i -= 1) {
      this.rightPanel.remove(this.columns[i], autoDestroy);
      this.columns.splice(i, 1);
    }
  }
});
