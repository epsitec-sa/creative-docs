// This class is used to manage the columns that are displayed on the right part
// of the main window. It queries the server to get their layout when the user
// makes an action that triggers the addition of a new column, parses the
// response and displays the resulting instances of EntityColumn.

Ext.require([
  'Epsitec.cresus.webcore.entityList.EntityListPanel',
  'Epsitec.cresus.webcore.entityUi.BrickWallParser',
  'Epsitec.cresus.webcore.entityUi.EntityAction',
  'Epsitec.cresus.webcore.entityUi.SetColumn',
  'Epsitec.cresus.webcore.entityUi.TileColumn',
  'Epsitec.cresus.webcore.entityUi.TypeAction',
  'Epsitec.cresus.webcore.tools.Callback',
  'Epsitec.cresus.webcore.tools.CallbackQueue',
  'Epsitec.cresus.webcore.tools.Tools',
  'Epsitec.cresus.webcore.tools.ViewMode'
],
function() {
  Ext.define('Epsitec.cresus.webcore.entityUi.ColumnManager', {
    extend: 'Ext.panel.Panel',
    alternateClassName: ['Epsitec.ColumnManager'],

    /* Configuration */

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
      this.leftList = this.createLeftList(this.database);
      this.rightPanel = this.createRightPanel();

      this.add([this.leftList, this.rightPanel]);

      return this;
    },

    /* Methods */

    createLeftList: function(database) {
      return Ext.create('Epsitec.EntityListPanel', {
        list: {
          entityListTypeName: 'Epsitec.DatabaseEditableEntityList',
          databaseName: database.name,
          multiSelect: false,
          onSelectionChange: Epsitec.Callback.create(
              this.onEntityListSelectionChange,
              this
          )
        },
        container: {
          title: database.title,
          iconCls: database.iconSmall,
          region: 'west',
          bodyCls: 'left-list',
          width: 400,
          resizable: true,
          resizeHandles: 'e'
        }
      });
    },

    createRightPanel: function() {
      return Ext.create('Ext.panel.Panel', {
        region: 'center',
        border: false,
        layout: 'hbox',
        autoScroll: true
      });
    },

    addEntityColumnWithCallback: function(
        viewMode,
        viewId,
        entityId,
        parentColumn,
        callback) {
      // Here we make a new callback so as to call callback with the proper
      // arguments: the instance of EntityColumn instead of its configuration.
      var callbackQueue = Epsitec.CallbackQueue.empty();
      if (Ext.isDefined(callback)&&callback!==null) {
        callbackQueue = callbackQueue.enqueueCallback(
            function() {
              var lastColumn = this.columns[this.columns.length - 1];
              callback.execute([lastColumn]);
            },
            this);
      }

      this.addEntityColumn(
          viewMode, viewId, entityId, parentColumn, callbackQueue
      );
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

    refreshAllColumns: function() {
      var columnId;
      if (this.columns.length > 0) {
        columnId = this.columns[this.columns.length - 1].columnId;
        this.refreshColumns(0, columnId);
      }
    },

    refreshColumns: function(firstColumnId, lastColumnId) {
      var configs, nbConfigs, callbackQueue, callbackQueueCreator, i, column;

      // The idea of this method is to make one request for each column that
      // must be refreshed. We will store the decoded json responses of these
      // requests in the configs variable. The last response that is processed
      // will trigger the column replacement.

      // Note that the configs array stores the responses ordered by their
      // index. This is why we need a separate variable nbConfigs to count the
      // number of elements that are returned.

      // We need to have this callback queue creation embedded in a new function
      // because otherwise the closure would capture the i variable within the
      // loop, and all callbacks would execute with the same value for i. This
      // is the classical problem of variables captures in closures within
      // loops.

      configs = [];
      nbConfigs = 0;
      callbackQueueCreator = function(index) {
        return Epsitec.CallbackQueue.create(
            function(config) {
              nbConfigs += 1;
              configs[index] = config;
              if (nbConfigs === lastColumnId - firstColumnId + 1) {
                this.replaceExistingColumns(
                    firstColumnId, lastColumnId, configs
                );
              }
            },
            this
        );
      };

      for (i = firstColumnId; i <= lastColumnId; i += 1) {
        column = this.columns[i];
        callbackQueue = callbackQueueCreator.call(this, i - firstColumnId);
        this.execute(
            column.viewMode, column.viewId, column.entityId, column,
            callbackQueue
        );
      }
    },

    execute: function(vMode, vId, entityId, loadingColumn, callbackQueue) {
      if (loadingColumn !== null) {
        loadingColumn.setLoading();
      }
      Ext.Ajax.request({
        url: 'proxy/layout/entity/' + vMode + '/' + vId + '/' + entityId,
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

      column = this.createColumn(config, this.columns.length);
      this.addExistingColumn(column);

      // Scroll all the way to the right, in case there are more columns than
      // the screen is able to show
      dom = this.rightPanel.getEl().child('.x-panel-body').dom;
      dom.scrollLeft = 1000000;
      dom.scrollTop = 0;
    },

    createColumn: function(config, columnId) {
      var typeName, parsedConfig;

      parsedConfig = Epsitec.BrickWallParser.parseColumn(config);
      parsedConfig.columnId = columnId;
      parsedConfig.columnManager = this;

      typeName = parsedConfig.typeName;
      delete parsedConfig.typeName;

      return Ext.create(typeName, parsedConfig);
    },

    addExistingColumn: function(column) {
      this.rightPanel.add(column);

      this.columns.push(column);
    },

    replaceExistingColumns: function(firstColumnId, lastColumnId, configs) {
      // The table layout used to show the columns does not allow to replace
      // some column in the middle of the table. The only solution I've found is
      // the following:
      // 1) Remove all the columns to the right of the ones that we want to
      //    replace but keep them around for later reuse.
      // 2) Remove and delete the columns that we want to replace.
      // 3) Add the new version of the columns that we wanted to replace.
      // 4) Add the columns that were to the right of the columns that we have
      //    replaced.

      // We have to remember the state of the columns. so we can re-apply it
      // once they are replaced.

      // We have to remember the scroll, to re-apply it once the layout has been
      // rebuilt.

      var i, dom, scrollLeft, scrollTop, savedColumns, columnStates, config,
          column;

      // Remember the scroll position.
      dom = this.rightPanel.getEl().child('.x-panel-body').dom;
      scrollLeft = dom.scrollLeft;
      scrollTop = dom.scrollTop;

      // Copy the columns that we want to readd later.
      savedColumns = this.columns.slice(lastColumnId + 1);

      // Save the column state.
      columnStates = [];
      if(this.columns.length > 0)
      {
        for (i = firstColumnId; i <= lastColumnId; i += 1) {
          columnStates.push(this.columns[i].getState());
        }
        // Remove the columns at the right ot the ones that we want to replace,
        // but don't delete them as we need them later.
        this.removeColumns(lastColumnId + 1, this.columns.length - 1, false);

        // Removes the columns that we will replace and deletes them.
        this.removeColumns(firstColumnId, lastColumnId, true);

        // Replace the columns with their new version.
        for (i = firstColumnId; i <= lastColumnId; i += 1) {
          config = configs[i - firstColumnId];
          column = this.createColumn(config, i);
          this.addExistingColumn(column);
        }

        // Add the column that we removed before so that they are displayed again.
        for (i = 0; i < savedColumns.length; i += 1) {
          this.addExistingColumn(savedColumns[i]);
        }

        // Re-apply the state on the columns that we have just added.
        for (i = firstColumnId; i <= lastColumnId; i += 1) {
          this.columns[i].setState(columnStates[i - firstColumnId]);
        }
      
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
    },

    showAction: function(viewMode, viewId, entityId, aEntityId, inQueue, callback) {
      Epsitec.EntityAction.showDialog(
          viewMode, viewId, entityId, aEntityId, inQueue, callback
      );
    },

    onEntityListSelectionChange: function(entityItems) {
      var entityId = entityItems.length === 1 ? entityItems[0].id : null;
      this.showFirstEntityColumn(entityId, null);
    },

    selectEntity: function(entityId, callback) {
      // Here, we tell the list not to fire the selection event which would
      // trigger the display of the first column by executing the method
      // onEntityListSelectionChange(...). The reason is that we want to display
      // the first entity column ourselves. This allows us to have more control
      // over the callback sequence and thus we can have another callback that
      // will be executed once the first entity column has been created and not
      // when the entity has been selected, at the point where the AJAX request
      // that gets the column data has been started but not yet finished.
      this.leftList.selectEntity(entityId, true);
      this.showFirstEntityColumn(entityId, callback);
    },

    showFirstEntityColumn: function(entityId, callback) {
      this.removeAllColumns();
      if (entityId !== null) {
        this.addEntityColumnWithCallback(
            Epsitec.ViewMode.summary, 'null', entityId, null, callback
        );
      }
    }
  });
});
