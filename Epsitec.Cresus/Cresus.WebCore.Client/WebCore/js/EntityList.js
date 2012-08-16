Ext.define('Epsitec.cresus.webcore.EntityList', {
  extend: 'Ext.grid.Panel',
  alternateClassName: ['Epsitec.EntityList'],

  /* Config */

  border: true,
  viewConfig: {
    emptyText: 'Nothing to display'
  },
  sortableColumns: false,

  /* Properties */

  databaseName: null,
  onSelectionChangeCallback: null,

  /* Constructor */

  constructor: function(options) {
    var newOptions = {
      store: this.getStore(options.databaseName, options.fields),
      selModel: this.getSelModel(options),
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

  getSelModel: function(options) {
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

  getStore: function(databaseName, fields) {
    return Ext.create('Ext.data.Store', {
      fields: fields,
      autoLoad: true,
      pageSize: 100,
      remoteSort: true,
      buffered: true,
      proxy: {
        type: 'ajax',
        url: 'proxy/database/get/' + databaseName,
        reader: {
          type: 'json',
          root: 'content.entities',
          totalProperty: 'content.total'
        }
      }
    });
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
