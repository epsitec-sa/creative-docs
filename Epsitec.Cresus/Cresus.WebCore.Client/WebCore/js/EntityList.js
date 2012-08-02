Ext.define('Epsitec.cresus.webcore.EntityList', {
  extend: 'Ext.grid.Panel',
  alternateClassName: ['Epsitec.EntityList'],

  /* Config */

  allowDeselect: true,
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
      dataIndex: 'summary'
    }
  ],

  /* Properties */

  columnManager: null,
  databaseName: null,

  /* Constructor */

  constructor: function(options) {
    options.store = this.getStore(options.databaseName);
    options.tbar = this.getTBar();
    this.callParent(arguments);
    this.addListener('selectionchange', this.onSelectionChangeHandler, this);
    return this;
  },

  /* Additional methods */

  onCreateHandler: function() {
    var callback = Epsitec.Callback.create(
        function(entityItem) { this.onCreate(entityItem); },
        this
        );
    this.createEntity(callback);
  },

  // To be overriden in derived classes.
  onCreate: function(entityItem) { },

  onDeleteHandler: function() {
    var selection, entityItems, callback;

    selection = this.getSelectionModel().selected.items;
    if (selection.length === 0) {
      return;
    }

    entityItems = this.getItems(selection);
    callback = Epsitec.Callback.create(
        function(items) { this.onDelete(items); },
        this
        );
    this.deleteEntities(entityItems, callback);
  },

  // To be overriden in derived classes.
  onDelete: function(entityItems) { },

  onRefreshHandler: function() {
    this.reloadStore();
    this.onRefresh();
  },

  // To be overriden in derived classes.
  onRefresh: function() { },

  onSelectionChangeHandler: function(view, selection, options) {
    var entityItems = this.getItems(selection);
    this.onSelectionChange(entityItems);
  },

  // To be overriden in derived classes.
  onSelectionChange: function(entityItems) { },

  getItems: function(selection) {
    return selection.map(function(e) { return e.toItem(); });
  },

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

  reloadStore: function() {
    this.store.reload();
  },

  createEntity: function(callback) {
    this.setLoading();
    Ext.Ajax.request({
      url: 'proxy/database/create/' + this.databaseName,
      method: 'POST',
      callback: function(options, success, response) {
        this.createEntityCallback(success, response, callback);
      },
      scope: this
    });
  },

  createEntityCallback: function(success, response, callback) {
    var json, entityItem;

    this.setLoading(false);

    if (!success) {
      Epsitec.ErrorHandler.handleError(response);
      return;
    }

    this.reloadStore();

    json = Epsitec.Tools.decodeJson(response.responseText);
    if (json === null) {
      return;
    }

    entityItem = json.content;
    callback.execute([entityItem]);
  },

  deleteEntities: function(entityItems, callback) {
    this.setLoading();
    Ext.Ajax.request({
      url: 'proxy/database/delete',
      method: 'POST',
      params: {
        entityIds: entityItems.map(function(e) { return e.id; }).join(';')
      },
      callback: function(options, success, response) {
        this.deleteEntitiesCallback(success, response, callback, entityItems);
      },
      scope: this
    });
  },

  deleteEntitiesCallback: function(success, response, callback, entityItems) {
    this.setLoading(false);

    if (!success) {
      Epsitec.ErrorHandler.handleError(response);
      return;
    }

    this.reloadStore();
    callback.execute([entityItems]);
  },

  getTBar: function() {
    var buttonCreate, buttonDelete, buttonRefresh;

    buttonCreate = this.getTBarButton({
      tooltip: 'Create',
      iconCls: 'epsitec-cresus-core-images-edition-new-icon32',
      listeners: {
        click: this.onCreateHandler,
        scope: this
      }
    });

    buttonDelete = this.getTBarButton({
      tooltip: 'Delete',
      iconCls: 'epsitec-cresus-core-images-edition-cancel-icon32',
      listeners: {
        click: this.onDeleteHandler,
        scope: this
      }
    });

    buttonRefresh = this.getTBarButton({
      tooltip: 'Refresh',
      iconCls: 'epsitec-cresus-core-images-data-workflowevent-icon32',
      listeners: {
        click: this.onRefreshHandler,
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
