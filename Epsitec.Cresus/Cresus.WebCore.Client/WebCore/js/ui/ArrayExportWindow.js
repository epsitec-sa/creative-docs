// This class is a window that lets the used configure an exportation of
// entities to an array file (.csv or .pdf for instance). Mostly, it is used to
// select the columns that will be exported to the file. But in the future, it
// might include other options as well.

Ext.require([
  'Epsitec.cresus.webcore.tools.Texts',
  'Epsitec.cresus.webcore.tools.Tools',
  'Epsitec.cresus.webcore.ui.ArrayExportItem'
],
function() {
  Ext.define('Epsitec.cresus.webcore.ui.ArrayExportWindow', {
    extend: 'Ext.window.Window',
    alternateClassName: ['Epsitec.ArrayExportWindow'],

    /* Configuration */

    width: 400,
    height: 300,
    border: false,
    layout: {
      type: 'hbox',
      align: 'stretch'
    },
    modal: true,
    plain: true,
    title: Epsitec.Texts.getExportTitle(),

    /* Properties */

    leftGrid: null,
    rightGrid: null,
    exportUrl: null,
    columnDefinitions: null,

    /* Constructor */

    constructor: function(options) {
      var newOptions;

      this.leftGrid = this.createLeftGrid(options);
      this.rightGrid = this.createRightGrid(options);

      newOptions = {
        items: [this.leftGrid, this.rightGrid],
        buttons: [
          this.createOkButton(),
          this.createCancelButton()
        ]
      };
      Ext.applyIf(newOptions, options);

      this.callParent([newOptions]);
      return this;
    },

    /* Methods */

    createLeftGrid: function(options) {
      var title = Epsitec.Texts.getExportExportedColumns();
      return this.createGrid(false, title, {
        margin: '0 5 0 0',
        store: this.createStore(options.columnDefinitions, false)
      });
    },

    createRightGrid: function(options) {
      var title = Epsitec.Texts.getExportDiscardedColumns();
      return this.createGrid(true, title, {
        store: this.createStore(options.columnDefinitions, true),
        listeners: {
          viewready: this.sortRightGrid,
          scope: this
        }
      });
    },

    createGrid: function(sortOnDrop, columnTitle, options) {
      var newOptions = {
        flex: 1,
        sortableColumns: false,
        viewConfig: {
          plugins: {
            ptype: 'gridviewdragdrop',
            ddGroup: 'sortPanelDDGroup'
          }
        },
        columns: [{
          text: columnTitle,
          dataIndex: 'title',
          flex: 1
        }],
        selModel: {
          selType: 'rowmodel',
          allowDeselect: true,
          mode: 'MULTI'
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

    createStore: function(columnDefinitions, hidden) {
      var data = columnDefinitions
          .filter(function(c) {
            return c.hidden === hidden;
          });

      return Ext.create('Ext.data.JsonStore', {
        model: 'Epsitec.cresus.webcore.ui.ArrayExportItem',
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

    onCancelClick: function() {
      this.close();
    },

    onOkClick: function() {
      var url, columnIds;

      columnIds = this.getSelectedColumnIds().join(';');

      url = this.exportUrl;
      url = Epsitec.Tools.addParameterToUrl(url, 'type', 'array');
      url = Epsitec.Tools.addParameterToUrl(url, 'columns', columnIds);

      window.open(url);
      this.close();
    },

    getSelectedColumnIds: function() {
      return this.leftGrid.store
          .getRange()
          .map(function(i) {
            return i.get('name');
          });
    }
  });
});
