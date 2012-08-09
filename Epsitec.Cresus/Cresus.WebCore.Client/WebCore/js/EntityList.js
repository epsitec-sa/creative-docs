Ext.define('Epsitec.cresus.webcore.EntityList', {
  extend: 'Ext.grid.Panel',
  alternateClassName: ['Epsitec.EntityList'],

  /* Config */

  border: true,
  selModel: {
    selType: 'rowmodel',
    allowDeselect: true,
    mode: 'SINGLE'
  },
  viewConfig: {
    emptyText: 'Nothing to display'
  },
  sortableColumns: false,
  columns: [
    {
      xtype: 'rownumberer',
      width: 35
    },
    {
      text: 'Summary',
      flex: 1,
      dataIndex: 'summary'
    }
  ],

  /* Properties */

  databaseName: null,

  /* Constructor */

  constructor: function(options) {
    var newOptions = {
      store: this.getStore(options.databaseName),
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

  getStore: function(databaseName) {
    return Ext.create('Ext.data.Store', {
      model: 'Epsitec.cresus.webcore.EntityListItem',
      autoLoad: true,
      pageSize: 100,
      remoteSort: true,
      buffered: true,
      proxy: {
        type: 'ajax',
        url: 'proxy/database/get/' + databaseName,
        reader: {
          type: 'json',
          root: 'entities',
          totalProperty: 'total'
        }
      }
    });
  },

  onSelectionChangeHandler: function(view, selection, options) {
    var entityItems = this.getItems(selection);
    this.onSelectionChange(entityItems);
  },

  // To be overriden in derived classes.
  onSelectionChange: function(entityItems) { },

  getSelectedItems: function() {
    var selection = this.getSelectionModel().getSelection();
    return this.getItems(selection);
  },

  getItems: function(selection) {
    return selection.map(function(e) { return e.toItem(); });
  }
});
