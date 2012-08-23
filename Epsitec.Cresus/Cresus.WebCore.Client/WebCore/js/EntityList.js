Ext.define('Epsitec.cresus.webcore.EntityList', {
  extend: 'Ext.grid.Panel',
  alternateClassName: ['Epsitec.EntityList'],

  /* Config */

  border: false,
  viewConfig: {
    emptyText: 'Nothing to display'
  },

  /* Properties */

  databaseName: null,
  onSelectionChangeCallback: null,
  columnDefinitions: null,

  /* Constructor */

  constructor: function(options) {
    var newOptions = {
      dockedItems: [
        this.createToolbar(options)
      ],
      columns: this.createColumns(options),
      store: this.createStore(options),
      selModel: this.createSelModel(options),
      onSelectionChangeCallback: options.onSelectionChange,
      listeners: {
        selectionchange: this.onSelectionChangeHandler,
        scope: this
      }
    };
    Ext.applyIf(newOptions, options);

    this.callParent([newOptions]);
    return this;
  },

  /* Additional methods */

  createColumns: function(options) {
    var basicColumns = this.createBasicColumns(options.columnDefinitions),
        dynamicColumns = this.createDynamicColumns(options.columnDefinitions);

    return basicColumns.concat(dynamicColumns);
  },

  createBasicColumns: function(columnDefinitions) {
    var basicColumns = [
      {
        xtype: 'rownumberer',
        width: 35,
        sortable: false
      }
    ];

    if (Epsitec.Tools.isArrayEmpty(columnDefinitions)) {
      basicColumns.push({
        text: 'Summary',
        flex: 1,
        dataIndex: 'summary',
        sortable: false
      });
    }

    return basicColumns;
  },

  createDynamicColumns: function(columnDefinitions) {
    return columnDefinitions
        .filter(function(c) {
          return c.hidden === false;
        })
        .map(function(c) {
          var column = {
            text: c.title,
            flex: 1,
            dataIndex: c.name,
            sortable: c.sortable
          };

          switch (c.type) {
            case 'boolean':
              column.xtype = 'booleancolumn';
              break;

            case 'date':
              column.xtype = 'datecolumn';
              column.format = 'd/m/Y';
              break;

            case 'int':
              column.xtype = 'numbercolumn';
              column.format = '0,000';
              break;

            case 'float':
              column.xtype = 'numbercolumn';
              break;

            case 'string':
              break;
          }

          return column;
        });
  },

  createSelModel: function(options) {
    if (options.multiSelect) {
      return {
        type: 'rowmodel',
        mode: 'MULTI'
      };
    }
    else {
      return {
        selType: 'rowmodel',
        allowDeselect: true,
        mode: 'SINGLE'
      };
    }
  },

  createStore: function(options) {
    return Ext.create('Ext.data.Store', {
      fields: this.createFields(options.columnDefinitions),
      sorters: this.createSorters(options.columnDefinitions),
      autoLoad: true,
      pageSize: 100,
      remoteSort: true,
      buffered: true,
      proxy: {
        type: 'ajax',
        url: 'proxy/database/get/' + options.databaseName,
        reader: {
          type: 'json',
          root: 'content.entities',
          totalProperty: 'content.total'
        },
        encodeSorters: this.encodeSorters
      }
    });
  },

  createFields: function(columnDefinitions) {
    var basicFields = this.createBasicFields(),
        dynamicFields = this.createDynamicFields(columnDefinitions);

    return basicFields.concat(dynamicFields);
  },

  createBasicFields: function() {
    return [
      {
        name: 'id',
        type: 'string'
      },
      {
        name: 'summary',
        type: 'string'
      }
    ];
  },

  createDynamicFields: function(columnDefinitions) {
    return columnDefinitions.map(function(c) {
      var field = {
        name: c.name,
        type: c.type
      };

      if (c.type === 'date') {
        field.dateFormat = 'd/m/Y';
      }

      return field;
    });
  },

  createSorters: function(columnDefinitions) {
    return columnDefinitions
        .filter(function(c) {
          return c.sortDirection !== null;
        }).map(function(c) {
          return {
            property: c.name,
            direction: c.sortDirection
          };
        });
  },

  encodeSorters: function(sorters) {
    var sorterStrings = sorters.map(function(s) {
      return s.property + ':' + s.direction;
    });

    return sorterStrings.join(';');
  },

  createToolbar: function(options) {
    return Ext.create('Ext.Toolbar', {
      dock: 'top',
      items: this.createButtons(options)
    });
  },

  createButtons: function(options) {
    var buttons = options.toolbarButtons || [];

    buttons.push(Ext.create('Ext.Button', {
      text: 'Sort',
      iconCls: 'icon-sort',
      listeners: {
        click: this.onSortHandler,
        scope: this
      }
    }));

    buttons.push(Ext.create('Ext.Button', {
      text: 'Refresh',
      iconCls: 'icon-refresh',
      listeners: {
        click: this.onRefreshHandler,
        scope: this
      }
    }));

    return buttons;
  },

  onRefreshHandler: function() {
    this.reloadStore();
  },

  reloadStore: function() {

    // A call to this.store.reload() should work here, but it has a bug. It is
    // complicated, but if there are not enough rows of data, and a new row is
    // added, this row is not displayed, even with several calls to this
    // method.
    //this.store.reload();

    // A call to this.store.load() should work here, but it has two bugs. It is
    // also complicated, but if there are enough rows of data, and a new row is
    // added, the scroll bar will bump when it reaches the bottom and we can
    // never see the last row. And if we delete an row and click where it was,
    // it is the deleted element that is selected internally, instead of being
    // the one that is displayed. A call to this.store.removeAll() corrects
    // those 2 bugs. But there is a third one. A call to this.store.load()
    // resets the position of the scroll bar to the top, whereas a call to
    // this.store.reload() would keep it. I did not find any workaround for this
    // yet.
    this.store.removeAll();
    this.store.load();
  },

  onSortHandler: function() {
    var sortWindow = Ext.create('Epsitec.SortWindow', {
      callback: Epsitec.Callback.create(this.setSorters, this),
      sorters: this.getCurrentSorters(),
      initialSorters: this.getInitialSorters()
    });

    sortWindow.show();
  },

  getCurrentSorters: function() {
    var usedSorters, unusedSorters;

    usedSorters = this.store.getSorters().map(
        function(s1) {
          return {
            title: this.columnDefinitions.filter(function(s2) {
              return s1.property === s2.name;
            })[0].title,
            name: s1.property,
            sortDirection: s1.direction
          };
        },
        this
        );

    unusedSorters = this.getInitialSorters().filter(function(s1) {
      return !usedSorters.some(function(s2) {
        return s1.name === s2.name;
      });
    });

    unusedSorters.forEach(function(s) {
      s.sortDirection = null;
    });

    return usedSorters.concat(unusedSorters);
  },

  getInitialSorters: function() {
    return this.columnDefinitions
        .filter(function(c) {
          return c.sortable === true;
        })
        .map(function(c) {
          return {
            title: c.title,
            name: c.name,
            sortDirection: c.sortDirection
          };
        });
  },

  setSorters: function(sorters) {
    var newSorters = sorters.map(function(s) {
      return {
        property: s.name,
        direction: s.sortDirection
      };
    });

    // The store.sort(...) method requires at least one sorter to do its job. So
    // if the user has removed all the sort criteria, we must do the job by
    // ourselves.

    if (sorters.length === 0)
    {
      this.store.sorters.clear();
      this.reloadStore();
    }
    else {
      this.store.sort(newSorters);
    }
  },

  onSelectionChangeHandler: function(view, selection, options) {
    var entityItems = this.getItems(selection);

    if (this.onSelectionChangeCallback !== null) {
      this.onSelectionChangeCallback.execute([entityItems]);
    }
  },

  getSelectedItems: function() {
    var selection = this.getSelectionModel().getSelection();
    return this.getItems(selection);
  },

  getItems: function(selection) {
    return selection.map(this.getItem);
  },

  getItem: function(row) {
    return {
      id: row.get('id'),
      summary: row.get('summary')
    };
  }
});
