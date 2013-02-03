Ext.require([
  'Epsitec.cresus.webcore.entityList.EditableEntityList',
  'Epsitec.cresus.webcore.tools.Callback',
  'Epsitec.cresus.webcore.tools.ErrorHandler',
  'Epsitec.cresus.webcore.tools.Texts',
  'Epsitec.cresus.webcore.tools.Tools'
],
function() {
  Ext.define('Epsitec.cresus.webcore.entityList.DatabaseEditableEntityList', {
    extend: 'Epsitec.cresus.webcore.entityList.EditableEntityList',
    alternateClassName: ['Epsitec.DatabaseEditableEntityList'],

    /* Properties */

    databaseName: null,
    entityTypeId: null,

    /* Constructor */

    constructor: function(options) {
      var newOptions = {
        getUrl: 'proxy/database/get/' + options.databaseName,
        addLabel: Epsitec.Texts.getCreateLabel(),
        removeLabel: Epsitec.Texts.getDeleteLabel()
      };
      Ext.applyIf(newOptions, options);

      this.callParent([newOptions]);
      return this;
    },

    /* Additional methods */

    handleAdd: function() {
      if (this.filters.getFilterData().length > 0) {
        Ext.MessageBox.confirm(
            Epsitec.Texts.getWarningTitle(),
            Epsitec.Texts.getEntityCreationWarningMessage(),
            this.handleAddCallback,
            this
        );
      }
      else {
        this.createEntity();
      }
    },

    handleAddCallback: function(buttonId) {
      if (buttonId === 'yes') {
        this.createEntity();
      }
    },

    createEntity: function() {
      if (this.creationViewId === null) {
        this.createEntityWithoutView();
      }
      else {
        this.createEntityWithView();
      }
    },

    createEntityWithView: function() {
      var callback = Epsitec.Callback.create(
          this.createEntityWithViewCallback, this);

      Epsitec.TypeAction.showDialog(
          this.creationViewId, this.entityTypeId, callback
      );
    },

    createEntityWithViewCallback: function(entityId) {
      this.selectEntity(entityId);
    },

    createEntityWithoutView: function() {
      this.setLoading();
      Ext.Ajax.request({
        url: 'proxy/database/create',
        method: 'POST',
        params: {
          databaseName: this.databaseName
        },
        callback: this.createEntityWithoutViewCallback,
        scope: this
      });
    },

    createEntityWithoutViewCallback: function(options, success, response) {
      var json, entityId;

      this.setLoading(false);

      json = Epsitec.Tools.processResponse(success, response);
      if (json === null) {
        return;
      }

      entityId = json.content.id;
      this.selectEntity(entityId);
    },

    handleRemove: function(entityItems) {
      this.deleteEntities(entityItems);
    },


    deleteEntities: function(entityItems) {
      if (this.creationViewId === null) {
        this.deleteEntitiesWithoutView(entityItems);
      }
      else {
        if (entityItems.length !== 1) {
          throw 'Invalid selection length.';
        }
        this.deleteEntityWithView(entityItems[0]);
      }
    },

    deleteEntityWithView: function(entityItem) {
      var callback = Epsitec.Callback.create(
          this.deleteEntityWithViewCallback, this);

      Epsitec.EntityAction.showDialog(
          '9', this.creationViewId, entityItem.id, null, callback
      );
    },

    deleteEntityWithViewCallback: function() {
      this.reloadStore();
    },


    deleteEntitiesWithoutView: function(entityItems) {
      this.setLoading();
      Ext.Ajax.request({
        url: 'proxy/database/delete',
        method: 'POST',
        params: {
          databaseName: this.databaseName,
          entityIds: entityItems.map(function(e) { return e.id; }).join(';')
        },
        callback: this.deleteEntitiesWithoutViewCallback,
        scope: this
      });
    },

    deleteEntitiesWithoutViewCallback: function(options, success, response) {
      var json;

      this.setLoading(false);

      json = Epsitec.Tools.processResponse(success, response);
      if (json === null) {
        return;
      }

      this.reloadStore();
    },

    selectEntity: function(entityId) {
      this.clearStore();
      this.setLoading();

      Ext.Ajax.request({
        url: this.buildGetIndexUrl(entityId),
        method: 'GET',
        callback: function(options, success, response) {
          this.selectEntityCallback(success, response, entityId);
        },
        scope: this
      });
    },

    buildGetIndexUrl: function(entityId) {
      var base, sorters, filters, parameters, key, value;

      base = 'proxy/database/getindex' +
          '/' + this.databaseName +
          '/' + entityId;

      parameters = [];

      sorters = this.store.getSorters();
      if (sorters.length > 0) {
        key = 'sort';
        value = this.store.proxy.encodeSorters(sorters);
        parameters.push([key, value]);
      }

      filters = this.filters.getFilterData();
      if (filters.length > 0) {
        key = 'filter';
        value = this.filters.buildQuery(filters).filter;
        parameters.push([key, value]);
      }

      return Epsitec.Tools.createUrl(base, parameters);
    },

    selectEntityCallback: function(success, response, entityId) {
      var json, index, halfRange;

      this.setLoading(false);

      json = Epsitec.Tools.processResponse(success, response);
      if (json === null) {
        return;
      }

      index = json.content.index;

      if (index === null) {
        // The requested entity is not in the data set. This is probably because
        // it does not match the filters.
        Epsitec.ErrorHandler.showError(
            Epsitec.Texts.getErrorTitle(),
            Epsitec.Texts.getEntitySelectionErrorMessage()
        );
        this.reloadStore();
        return;
      }

      halfRange = this.store.pageSize / 2;

      this.store.guaranteeRange(
          Math.max(0, index - halfRange),
          index + halfRange,
          function() {
            this.selectEntityCallback2(entityId);
          },
          this
      );
    },

    selectEntityCallback2: function(entityId) {
      // We don't look for the record by its index but by its id. This is
      // the index might have changed if another user has added or removed
      // entities. We hope that our record has not been shifted too far away
      // from the index that we got. If that's the case, we'll still be able to
      // find it in the data that has been loaded by the call to guaranteeRage.
      var record = this.store.getById(entityId);

      if (record === null) {
        // the record was not found, it is outside the range that was loaded by
        // the call to guaranteeRange. Therefore, we start again, hoping that
        // this time the index won't change.
        this.selectEntity(entityId);
        return;
      }

      this.getSelectionModel().select(record, false, false);
    }
  });
});
