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

  /* Constructor */

  constructor: function(options) {
    var newOptions = {
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
      fields: options.fields,
      sorters: options.sorters,
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

  encodeSorters: function(sorters) {
    var sorterStrings = sorters.map(function(s) {
      return s.property + ':' + s.direction;
    });

    return sorterStrings.join(';');
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
