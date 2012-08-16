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
    this.createEntity();
  },

  onDeleteHandler: function() {
    var entityItems;

    entityItems = this.getSelectedItems();
    if (entityItems.length === 0) {
      return;
    }

    this.deleteEntities(entityItems);
  },

  onRefreshHandler: function() {
    this.reloadStore();
  },

  reloadStore: function() {
    this.store.reload();
  },

  createEntity: function() {
    this.setLoading();
    Ext.Ajax.request({
      url: 'proxy/database/create',
      method: 'POST',
      params: {
        databaseName: this.databaseName
      },
      callback: this.createEntityCallback,
      scope: this
    });
  },

  createEntityCallback: function(options, success, response) {
    var json;

    this.setLoading(false);

    json = Epsitec.Tools.processResponse(success, response);
    if (json === null) {
      return;
    }

    this.reloadStore();
  },

  deleteEntities: function(entityItems) {
    this.setLoading();
    Ext.Ajax.request({
      url: 'proxy/database/delete',
      method: 'POST',
      params: {
        databaseName: this.databaseName,
        entityIds: entityItems.map(function(e) { return e.id; }).join(';')
      },
      callback: this.deleteEntitiesCallback,
      scope: this
    });
  },

  deleteEntitiesCallback: function(options, success, response) {
    var json;

    this.setLoading(false);

    json = Epsitec.Tools.processResponse(success, response);
    if (json === null) {
      return;
    }

    this.reloadStore();
  }
});
