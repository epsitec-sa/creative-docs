Ext.define('Epsitec.cresus.webcore.EntityList', {
  extend: 'Ext.grid.Panel',
  alternateClassName: ['Epsitec.EntityList'],

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

  /* Properties */

  entityListPanel: null,
  databaseName: null,

  /* Constructor */

  constructor: function(entityListPanel, databaseName) {
    this.entityListPanel = entityListPanel;
    this.databaseName = databaseName;

    this.store = this.getStore(this.databaseName);
    this.store.guaranteeRange(0, 100);

    this.tbar = this.getTBar();

    this.callParent(arguments);

    this.addListener('selectionchange', this.onSelectionChange, this);

    return this;
  },

  /* Additional methods */

  onCreateClick: function() {
    var callback = Epsitec.Callback.create(
        function(entityId) {
          this.entityListPanel.onCreate(entityId);
        },
        this
        );

    this.createEntity(callback);
  },

  onDeleteClick: function() {
    var selection = this.getSelectionModel().selected.items;

    if (selection.length === 0) {
      return;
    }

    var entityIds = this.getEntityIds(selection);

    var callback = Epsitec.Callback.create(
        function(entityIds) {
          this.entityListPanel.onDelete(entityIds);
        },
        this
        );

    this.deleteEntities(entityIds, callback);
  },

  onRefreshClick: function() {
    this.store.load();
    this.entityListPanel.onRefresh();
  },

  onSelectionChange: function(view, selection, options) {
    var entityIds = this.getEntityIds(selection);
    this.entityListPanel.onSelectionChange(entityIds);
  },

  getStore: function(databaseName) {
    return Ext.create('Ext.data.Store', {
      model: 'Epsitec.cresus.webcore.EntityListItem',
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
          options.failure.apply(arguments);
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
      handler: this.onCreateClick
    });

    var buttonDelete = this.getTBarButton({
      tooltip: 'Delete',
      iconCls: 'epsitec-cresus-core-images-edition-cancel-icon32',
      handler: this.onDeleteClick
    });

    var buttonRefresh = this.getTBarButton({
      tooltip: 'Refresh',
      iconCls: 'epsitec-cresus-core-images-data-workflowevent-icon32',
      handler: this.onRefreshClick
    });

    return [buttonCreate, buttonDelete, buttonRefresh];
  },

  getTBarButton: function(options) {
    var button = Ext.create('Ext.Button', {
      xtype: 'button',
      scale: 'large',
      iconAlign: 'top',
      tooltip: options.tooltip,
      iconCls: options.iconCls
    });

    button.setHandler(options.handler, this);

    return button;
  }
});
