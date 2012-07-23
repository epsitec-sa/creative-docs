Ext.define('Epsitec.Cresus.Core.Static.EntityList', {
  extend: 'Ext.grid.Panel',

  /* Config */

  multiSelect: false,
  hideHeaders: true,
  border: false,
  viewConfig: {
    emptyText: 'Nothing to display'
  },
  verticalScrollerType: 'paginggridscroller',
  loadMask: false,
  invalidateScrollerOnRefresh: false,
  columns: [
    {
      xtype: 'rownumberer',
      width: 35
    },
    {
      text: 'Name',
      flex: 1,
      dataIndex: 'name'
    }
  ],
  tbar: [
    {
      xtype: 'button',
      scale: 'large',
      iconAlign: 'top',
      tooltip: 'New',
      iconCls: 'epsitec-cresus-core-images-edition-new-icon32',
      handler: function()
      {
        var list = this.up('panel');

        list.createEntity();
      }
    },
    {
      xtype: 'button',
      scale: 'large',
      iconAlign: 'top',
      tooltip: 'Delete',
      iconCls: 'epsitec-cresus-core-images-edition-cancel-icon32',
      handler: function() {
        var list = this.up('panel');
        var selected = list.getSelectionModel().selected.items;

        if (selected.length !== 1) {
          var title = 'Not selected';
          var content = 'You need to selected an entity to perfom this action';
          Ext.Msg.alert(title, content);
          return;
        }

        var item = selected[0];
        var id = item.get('uniqueId');

        list.deleteEntity(id);
      }
    },
    {
      xtype: 'button',
      scale: 'large',
      iconAlign: 'top',
      tooltip: 'Refresh',
      iconCls: 'epsitec-cresus-core-images-data-workflowevent-icon32',
      handler: function() {
        var list = this.up('panel');
        list.store.load();
      }
    }
  ],

  /* Properties */

  databaseName: null,
  columnManager: null,

  /* Constructor */

  constructor: function(databaseName, columnManager) {
    this.databaseName = databaseName;
    this.columnManager = columnManager;

    this.store = this.getStore(this.databaseName);
    this.store.guaranteeRange(0, 100);

    this.callParent(arguments);

    this.addListener('selectionchange', this.onSelectionChange, this);

    return this;
  },

  /* Additional methods */

  onSelectionChange: function(view, selections, options) {
    if (selections.length !== 1) {
      return;
    }

    var record = selections[0];
    var entityId = record.get('uniqueId');

    this.columnManager.clearColumns();
    this.columnManager.addEntityColumn('summary', 'null', entityId);
  },

  deleteEntity: function(id) {
    this.setLoading();

    Ext.Ajax.request({
      url: 'proxy/database/delete',
      method: 'POST',
      params: {
        entityId: id
      },
      success: function(response, options) {
        this.setLoading(false);
        this.store.load();
      },
      failure: function(response, options) {
        this.setLoading(false);
        Epsitec.Cresus.Core.Static.ErrorHandler.handleError(response);
      },
      scope: this
    });
  },

  createEntity: function() {
    this.setLoading();

    Ext.Ajax.request({
      url: 'proxy/database/create/' + this.databaseName,
      method: 'POST',
      success: function(response, options) {
        this.setLoading(false);
        this.store.load();
      },
      failure: function(response, options)  {
        this.setLoading(false);
        Epsitec.Cresus.Core.Static.ErrorHandler.handleError(response);
      },
      scope: this
    });
  },

  getStore: function(databaseName) {
    return Ext.create('Ext.data.Store', {
      model: 'Epsitec.Cresus.Core.Static.EntityListItem',
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
  }
});
