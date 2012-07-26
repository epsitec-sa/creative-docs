Ext.define('Epsitec.cresus.webcore.EntityList', {
  extend: 'Ext.grid.Panel',
  alternateClassName: ['Epsitec.EntityList'],

  /* Config */

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

  /* Properties */

  columnManager: null,
  databaseName: null,

  /* Constructor */

  constructor: function(options) {
    this.store = this.getStore(options.databaseName);
    this.tbar = this.getTBar();

    this.callParent(arguments);

    this.addListener('selectionchange', this.onSelectionChangeInternal, this);

    return this;
  },

  /* Additional methods */

  onCreateInternal: function() {
    var callback = Epsitec.Callback.create(
        function(entityId) {
          this.onCreate(entityId);
        },
        this
        );

    this.createEntity(callback);
  },

  // To be overriden in derived classes.
  onCreate: function(entityId) { },

  onDeleteInternal: function() {
    var selection = this.getSelectionModel().selected.items;

    if (selection.length === 0) {
      return;
    }

    var entityIds = this.getEntityIds(selection);

    var callback = Epsitec.Callback.create(
        function(entityIds) {
          this.onDelete(entityIds);
        },
        this
        );

    this.deleteEntities(entityIds, callback);
  },

  // To be overriden in derived classes.
  onDelete: function(entityIds) { },

  onRefreshInternal: function() {
    this.store.reload();
    this.onRefresh();
  },

  // To be overriden in derived classes.
  onRefresh: function() { },

  onSelectionChangeInternal: function(view, selection, options) {
    var entityIds = this.getEntityIds(selection);
    this.onSelectionChange(entityIds);
  },

  // To be overriden in derived classes.
  onSelectionChange: function(entityIds) { },

  getStore: function(databaseName) {
    var store, pageSize;

    pageSize = 100;
    store = Ext.create('Ext.data.Store', {
      model: 'Epsitec.cresus.webcore.EntityListItem',
      pageSize: pageSize,
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

    store.guaranteeRange(0, pageSize);

    return store;
  },

  getEntityIds: function(selection) {
    return selection.map(function(e) { return e.get('uniqueId'); });
  },

  createEntity: function(callback) {
    this.setLoading();
    Ext.Ajax.request({
      url: 'proxy/database/create/' + this.databaseName,
      method: 'POST',
      success: function(response, options) {
        this.setLoading(false);
        this.store.load();
        try {
          var json = Ext.decode(response.responseText);
          var entityId = json.content;
          callback.execute([entityId]);
        }
        catch (err) {
          options.failure.apply(this, arguments);
        }
      },
      failure: function(response, options) {
        this.setLoading(false);
        Epsitec.ErrorHandler.handleError(response);
      },
      scope: this
    });
  },

  deleteEntities: function(entityIds, callback) {
    this.setLoading();
    Ext.Ajax.request({
      url: 'proxy/database/delete',
      method: 'POST',
      params: {
        entityIds: entityIds.join(';')
      },
      success: function(response, options) {
        this.setLoading(false);
        this.store.load();
        callback.execute([entityIds]);
      },
      failure: function(response, options) {
        this.setLoading(false);
        Epsitec.ErrorHandler.handleError(response);
      },
      scope: this
    });
  },

  getTBar: function() {

    var buttonCreate = this.getTBarButton({
      tooltip: 'Create',
      iconCls: 'epsitec-cresus-core-images-edition-new-icon32',
      listeners: {
        click: this.onCreateInternal,
        scope: this
      }
    });

    var buttonDelete = this.getTBarButton({
      tooltip: 'Delete',
      iconCls: 'epsitec-cresus-core-images-edition-cancel-icon32',
      listeners: {
        click: this.onDeleteInternal,
        scope: this
      }
    });

    var buttonRefresh = this.getTBarButton({
      tooltip: 'Refresh',
      iconCls: 'epsitec-cresus-core-images-data-workflowevent-icon32',
      listeners: {
        click: this.onRefreshInternal,
        scope: this
      }
    });

    return [buttonCreate, buttonDelete, buttonRefresh];
  },

  getTBarButton: function(options) {
    options.scale = 'large';
    options.iconAlign = 'top';

    return Ext.create('Ext.Button', options);
  }
});
