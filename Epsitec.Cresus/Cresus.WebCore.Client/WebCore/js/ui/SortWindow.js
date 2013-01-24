Ext.require([
  'Epsitec.cresus.webcore.tools.Texts',
  'Epsitec.cresus.webcore.ui.SortItem'
],
function() {
  Ext.define('Epsitec.cresus.webcore.ui.SortWindow', {
    extend: 'Ext.window.Window',
    alternateClassName: ['Epsitec.SortWindow'],

    /* Config */

    width: 400,
    height: 300,
    border: false,
    layout: {
      type: 'hbox',
      align: 'stretch'
    },
    modal: true,
    plain: true,
    title: Epsitec.Texts.getSortTitle(),

    /* Properties */

    leftGrid: null,
    rightGrid: null,
    callback: null,
    initialSorters: null,
    sorters: null,

    /* Constructor */

    constructor: function(options) {
      var newOptions;

      this.leftGrid = this.createLeftGrid(options);
      this.rightGrid = this.createRightGrid(options);

      newOptions = {
        items: [this.leftGrid, this.rightGrid],
        buttons: [
          this.createOkButton(),
          this.createResetButton(),
          this.createCancelButton()
        ]
      };
      Ext.applyIf(newOptions, options);

      this.callParent([newOptions]);
      return this;
    },

    /* Additional methods */

    createLeftGrid: function(options) {
      return this.createGrid(false, {
        margin: '0 5 0 0',
        columns: [
          this.createTitleColumn(),
          this.createSortDirectionColumn()
        ],
        store: this.createLeftStore(options.sorters)
      });
    },

    createRightGrid: function(options) {
      return this.createGrid(true, {
        store: this.createRightStore(options.sorters),
        columns: [
          this.createTitleColumn()
        ],
        listeners: {
          viewready: this.sortRightGrid,
          scope: this
        }
      });
    },

    createGrid: function(sortOnDrop, options) {
      var newOptions = {
        hideHeaders: true,
        flex: 1,
        sortableColumns: false,
        viewConfig: {
          plugins: {
            ptype: 'gridviewdragdrop',
            ddGroup: 'sortPanelDDGroup'
          }
        },
        selModel: {
          selType: 'rowmodel',
          allowDeselect: true,
          mode: 'SINGLE'
        }
      };

      if (sortOnDrop) {
        newOptions.viewConfig.listeners = {
          drop: this.sortRightGrid,
          scope: this
        };
      }

      Ext.applyIf(newOptions, options);

      return Ext.create('Ext.grid.Panel', newOptions);
    },

    createTitleColumn: function() {
      return {
        dataIndex: 'title',
        flex: 1
      };
    },

    createSortDirectionColumn: function() {
      return {
        xtype: 'actioncolumn',
        width: 20,
        items: [{
          getClass: function(v, meta, record) {
            return record.get('sortDirection') === 'ASC' ?
                'icon-sort-ascending' : 'icon-sort-descending';
          },
          handler: function(grid, rowIndex, colIndex, item, e, record) {
            var newDirection = record.get('sortDirection') === 'ASC' ?
                'DESC' : 'ASC';
            record.set('sortDirection', newDirection);
          }
        }]
      };
    },

    createLeftStore: function(sorters) {
      var data = sorters
          .filter(function(s) {
            return s.sortDirection !== null;
          });

      return this.createStore(data);
    },

    createRightStore: function(sorters) {
      var data = sorters
          .filter(function(s) {
            return s.sortDirection === null;
          })
          .map(function(s) {
            return {
              title: s.title,
              name: s.name,
              sortDirection: 'ASC'
            };
          });

      return this.createStore(data);
    },

    createStore: function(data) {
      return Ext.create('Ext.data.JsonStore', {
        model: 'Epsitec.cresus.webcore.ui.SortItem',
        proxy: {
          type: 'memory',
          reader: {
            type: 'json'
          }
        },
        data: data
      });
    },

    sortRightGrid: function() {
      this.rightGrid.store.sort({
        property: 'title',
        direction: 'ASC'
      });
    },

    createResetButton: function() {
      return Ext.create('Ext.Button', {
        text: Epsitec.Texts.getResetLabel(),
        handler: this.onResetClick,
        scope: this
      });
    },

    createCancelButton: function() {
      return Ext.create('Ext.Button', {
        text: Epsitec.Texts.getCancelLabel(),
        handler: this.onCancelClick,
        scope: this
      });
    },

    createOkButton: function() {
      return Ext.create('Ext.Button', {
        text: Epsitec.Texts.getOkLabel(),
        handler: this.onOkClick,
        scope: this
      });
    },

    onResetClick: function() {
      var leftStore = this.createLeftStore(this.initialSorters),
          rightStore = this.createRightStore(this.initialSorters);

      this.leftGrid.reconfigure(leftStore);
      this.rightGrid.reconfigure(rightStore);
      this.sortRightGrid();
    },

    onCancelClick: function() {
      this.close();
    },

    onOkClick: function() {
      var sorters = this.leftGrid.store
          .getRange()
          .map(function(i) {
            return {
              title: i.get('title'),
              name: i.get('name'),
              sortDirection: i.get('sortDirection')
            };
          });

      this.callback.execute([sorters]);
      this.close();
    }
  });
});
