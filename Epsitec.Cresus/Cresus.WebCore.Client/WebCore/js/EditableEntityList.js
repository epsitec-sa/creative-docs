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

    // A call to this.store.reload() should work here, but it has a bug. It is
    // complicated, but if there are not enough rows of data, and a new row is
    // added, this row is not displayed, even with several calls to this
    // method.
    //this.store.reload();

    // A call to this.store.load() should work here, but it has two bugs. It is
    // also complicated, but if there are enough rows of data, and a new row is
    // added, the scroll bar will bump when it reaches the bottom and we can
    // never see the last row. And if we delete an row and click where it was,
    // it is the deleted element that is selected internally, instead of being
    // the one that is displayed. A call to this.store.removeAll() corrects
    // those 2 bugs. But there is a third one. A call to this.store.load()
    // resets the position of the scroll bar to the top, whereas a call to
    // this.store.reload() would keep it. I did not find any workaround for this
    // yet.
    this.store.removeAll();
    this.store.load();
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
