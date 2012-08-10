Ext.define('Epsitec.cresus.webcore.EditableEntityList', {
  extend: 'Epsitec.cresus.webcore.EntityList',
  alternateClassName: ['Epsitec.EditableEntityList'],

  /* Constructor */

  constructor: function(options) {
    var newOptions = {
      tbar: this.getTBar()
    };
    Ext.applyIf(newOptions, options);

    this.callParent([newOptions]);
    return this;
  },

  /* Additional methods */

  getTBar: function() {
    var buttonCreate, buttonDelete, buttonRefresh;

    buttonCreate = Ext.create('Ext.Button', {
      text: 'Create',
      iconCls: 'icon-add',
      listeners: {
        click: this.onCreateHandler,
        scope: this
      }
    });

    buttonDelete = Ext.create('Ext.Button', {
      text: 'Delete',
      iconCls: 'icon-remove',
      listeners: {
        click: this.onDeleteHandler,
        scope: this
      }
    });

    buttonRefresh = Ext.create('Ext.Button', {
      text: 'Refresh',
      iconCls: 'icon-refresh',
      listeners: {
        click: this.onRefreshHandler,
        scope: this
      }
    });

    return [buttonCreate, buttonDelete, buttonRefresh];
  },

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
    var entityItems, callback;

    entityItems = this.getSelectedItems();
    if (entityItems.length === 0) {
      return;
    }

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
  }
});
