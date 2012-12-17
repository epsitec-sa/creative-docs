Ext.require([
  'Epsitec.cresus.webcore.Callback',
  'Epsitec.cresus.webcore.BooleanNullableColumn',
  'Epsitec.cresus.webcore.Enumeration',
  'Epsitec.cresus.webcore.ErrorHandler',
  'Epsitec.cresus.webcore.ListColumn',
  'Epsitec.cresus.webcore.SortWindow',
  'Epsitec.cresus.webcore.Texts',
  'Epsitec.cresus.webcore.Tools',
  'Ext.ux.grid.FiltersFeature'
],
function() {
  Ext.define('Epsitec.cresus.webcore.EntityList', {
    extend: 'Ext.grid.Panel',
    alternateClassName: ['Epsitec.EntityList'],

    /* Config */

    border: false,
    viewConfig: {
      emptyText: Epsitec.Texts.getEmptyListText()
    },

    /* Properties */

    onSelectionChangeCallback: null,
    columnDefinitions: null,
    sorterDefinitions: null,

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
        },
        features: [{
          ftype: 'filters',
          encode: true
        }]
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
          sortable: false,
          resizable: true
        }
      ];

      if (Epsitec.Tools.isArrayEmpty(columnDefinitions)) {
        basicColumns.push({
          text: Epsitec.Texts.getSummaryHeader(),
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
          .map(this.createDynamicColumn, this);
    },

    createDynamicColumn: function(columnDefinition) {
      var column = {
        text: columnDefinition.title,
        flex: 1,
        dataIndex: columnDefinition.name,
        sortable: columnDefinition.sortable,
        filter: this.createFilter(columnDefinition)
      };

      switch (columnDefinition.type.type) {
        case 'boolean':
          column.xtype = 'booleannullablecolumn';
          break;

        case 'date':
          column.xtype = 'datecolumn';
          column.format = 'd.m.Y';
          break;

        case 'int':
          column.xtype = 'numbercolumn';
          column.format = '0,000';
          break;

        case 'float':
          column.xtype = 'numbercolumn';
          break;

        case 'list':
          column.xtype = 'listcolumn';
          column.enumerationName = columnDefinition.type.enumerationName;
          break;

        case 'string':
          break;
      }

      return column;
    },

    createFilter: function(columnDefinition) {
      var typeDefinition = columnDefinition.type;

      if (!columnDefinition.filter.filterable) {
        return false;
      }

      switch (typeDefinition.type) {
        case 'boolean':
          return {
            type: 'boolean'
          };

        case 'date':
          return {
            type: 'date',
            dateFormat: 'd.m.Y'
          };

        case 'int':
        case 'float':
          return {
            type: 'numeric'
          };

        case 'list':
          return {
            type: 'list',
            store: Epsitec.Enumeration.getStore(typeDefinition.enumerationName),
            labelField: 'text'
          };

        case 'string':
          return {
            type: 'string'
          };

        default:
          return false;
      }
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
        sorters: this.createSorters(options.sorterDefinitions),
        autoLoad: true,
        pageSize: 100,
        remoteSort: true,
        buffered: true,
        proxy: {
          type: 'ajax',
          url: options.getUrl,
          reader: {
            type: 'json',
            root: 'content.entities',
            totalProperty: 'content.total'
          },
          encodeSorters: this.encodeSorters,
          listeners: {
            exception: function(proxy, response, operation, eOpts) {
              Epsitec.Tools.processProxyError(response);
            }
          }
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
          type: c.type.type
        };

        switch (c.type.type) {
          case 'int':
          case 'float':
          case 'boolean':
            field.useNull = true;
            break;

          case 'date':
            field.dateFormat = 'd.m.Y';
            break;

          case 'list':
            field.type = 'string';
            break;
        }

        return field;
      });
    },

    createSorters: function(sorterDefinitions) {
      return sorterDefinitions.map(function(s) {
        return {
          property: s.name,
          direction: s.sortDirection
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
      var buttons = Ext.Array.clone(options.toolbarButtons || []);

      buttons.push(Ext.create('Ext.Button', {
        text: Epsitec.Texts.getSortLabel(),
        iconCls: 'icon-sort',
        listeners: {
          click: this.onSortHandler,
          scope: this
        }
      }));

      buttons.push(Ext.create('Ext.Button', {
        text: Epsitec.Texts.getRefreshLabel(),
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

      // A call to this.store.load() should work here, but it has two bugs. It
      // is also complicated, but if there are enough rows of data, and a new
      // row is added, the scroll bar will bump when it reaches the bottom and
      // we can never see the last row. And if we delete an row and click where
      // it was, it is the deleted element that is selected internally, instead
      // of being the one that is displayed. A call to this.store.removeAll()
      // corrects those 2 bugs. But there is a third one. A call to
      // this.store.load() resets the position of the scroll bar to the top,
      // whereas a call to this.store.reload() would keep it. I did not find any
      // workaround for this yet.
      this.clearStore();
      this.store.load();
    },

    clearStore: function() {
      this.store.removeAll();

      // The removeAll() method does not delete the totalCount, but we need to
      // do it if the total count has changed on the server, otherwise this
      // value would not be updated this leads to bugs where new rows are not
      // shown or selected because they are above the total count.
      // This stuff is done automatically in this.store.load() for instance, but
      // it is not done in this.store.guaranteeRange() or other methods. We do
      // it manually here so that we are sure that it is done at some point.
      delete this.store.totalCount;
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

      usedSorters = this.getUsedSorters(this.store.getSorters());
      unusedSorters = this.getUnusedSorters(usedSorters);

      return usedSorters.concat(unusedSorters);
    },

    getInitialSorters: function() {
      var usedSorters, unusedSorters;

      usedSorters = this.getUsedSorters(
          this.createSorters(this.sorterDefinitions)
          );

      unusedSorters = this.getUnusedSorters(usedSorters);

      return usedSorters.concat(unusedSorters);
    },

    getUsedSorters: function(sorters) {
      return sorters.map(
          function(s) {
            return {
              title: this.columnDefinitions.filter(function(c) {
                return s.property === c.name;
              })[0].title,
              name: s.property,
              sortDirection: s.direction
            };
          },
          this
      );
    },

    getUnusedSorters: function(usedSorters) {
      return this.columnDefinitions
          .filter(function(c) {
            return c.sortable === true && !usedSorters.some(function(s) {
              return c.name === s.name;
            });
          })
          .map(function(c) {
            return {
              title: c.title,
              name: c.name,
              sortDirection: null
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

      // The store.sort(...) method requires at least one sorter to do its job.
      // So if the user has removed all the sort criteria, we must do the job by
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
});
