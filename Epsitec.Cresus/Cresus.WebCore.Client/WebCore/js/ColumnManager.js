Ext.define('Epsitec.cresus.webcore.ColumnManager', {
  extend: 'Ext.panel.Panel',
  alternateClassName: ['Epsitec.ColumnManager'],

  /* Config */

  layout: 'border',

  /* Properties */

  leftList: null,
  rightPanel: null,
  columns: null,
  database: null,

  /* Constructor */

  constructor: function(options) {
    this.callParent([options]);

    this.columns = [];
    this.title = this.database.title;
    this.leftList = this.createLeftList(this.database);
    this.rightPanel = this.createRightPanel();

    this.add([this.leftList, this.rightPanel]);

    return this;
  },

  /* Additional methods */

  createLeftList: function(database) {
    return Ext.create('Epsitec.EntityListPanel', {
      list: {
        databaseName: database.name,
        editable: true,
        multiSelect: false,
        onSelectionChange: Epsitec.Callback.create(
            this.onEntityListSelectionChange,
            this
        )
      },
      container: {
        region: 'west',
        margin: 5,
        width: 250
      }
    });
  },

  onEntityListSelectionChange: function(entityItems) {
    this.removeAllColumns();
    if (entityItems.length === 1) {
      this.addEntityColumn('1', 'null', entityItems[0].id);
    }
  },

  createRightPanel: function() {
    return Ext.create('Ext.panel.Panel', {
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
  },

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
    var configs, nbConfigs, callbackQueue, callbackQueueCreator, i, column;

    configs = [];
    nbConfigs = 0;
    callbackQueueCreator = function(index) {
      return Epsitec.CallbackQueue.create(
          function(config) {
            nbConfigs += 1;
            configs[index] = config;
            if (nbConfigs === lastColumnId - firstColumnId + 1) {
              this.replaceExistingColumns(firstColumnId, lastColumnId, configs);
            }
          },
          this
      );
    };

    for (i = firstColumnId; i <= lastColumnId; i += 1) {
      column = this.columns[i];
      callbackQueue = callbackQueueCreator.call(this, i - firstColumnId);
      this.execute(
          column.viewMode, column.viewId, column.entityId, column, callbackQueue
      );
    }
  },

  execute: function(viewMode, viewId, entityId, loadingColumn, callbackQueue) {
    if (loadingColumn !== null) {
      loadingColumn.setLoading();
    }
    Ext.Ajax.request({
      url: 'proxy/layout/' + viewMode + '/' + viewId + '/' + entityId,
      callback: function(options, success, response) {
        this.executeCallback(success, response, loadingColumn, callbackQueue);
      },
      scope: this
    });
  },

  executeCallback: function(success, response, loadingColumn, callbackQueue) {
    var json, config;

    if (loadingColumn !== null) {
      loadingColumn.setLoading(false);
    }

    json = Epsitec.Tools.processResponse(success, response);
    if (json === null) {
      return;
    }

    config = json.content;
    callbackQueue.execute([config]);
  },

  addNewColumn: function(config) {
    var column, dom;

    config.columnId = this.columns.length;
    config.columnManager = this;

    column = Ext.create('Epsitec.EntityColumn', config);

    this.addExistingColumn(column);

    // Scroll all the way to the right, in case there are more columns than the
    // screen is able to show
    dom = this.rightPanel.getEl().child('.x-panel-body').dom;
    dom.scrollLeft = 1000000;
    dom.scrollTop = 0;
  },

  addExistingColumn: function(column) {
    this.rightPanel.add(column);

    this.columns.push(column);
  },

  replaceExistingColumns: function(firstColumnId, lastColumnId, configs) {
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
    var i, dom, scrollLeft, scrollTop, savedColumns, columnStates, config;

    // Remember the scroll position.
    dom = this.rightPanel.getEl().child('.x-panel-body').dom;
    scrollLeft = dom.scrollLeft;
    scrollTop = dom.scrollTop;

    // Copy the columns that we want to readd later.
    savedColumns = this.columns.slice(lastColumnId + 1);

    // Save the column state.
    columnStates = [];
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
      config = configs[i - firstColumnId];
      config.columnId = i;
      config.columnManager = this;
      this.addExistingColumn(Ext.create('Epsitec.EntityColumn', config));
    }

    // Add the column that we removed before so that they are displayed again.
    for (i = 0; i < savedColumns.length; i += 1) {
      this.addExistingColumn(savedColumns[i]);
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
    this.removeColumns(0, this.columns.length - 1, true);
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
