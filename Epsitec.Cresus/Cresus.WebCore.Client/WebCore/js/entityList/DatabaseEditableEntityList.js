// This class represents entity lists that are backed by a database on the
// webcore server and that can be edited.

Ext.require([
  'Epsitec.cresus.webcore.entityList.EditableEntityList',
  'Epsitec.cresus.webcore.tools.Callback',
  'Epsitec.cresus.webcore.tools.ErrorHandler',
  'Epsitec.cresus.webcore.tools.Texts',
  'Epsitec.cresus.webcore.tools.Tools',
  'Epsitec.cresus.webcore.tools.ViewMode'
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
        exportUrl: 'proxy/database/export/' + options.databaseName,
        addLabel: Epsitec.Texts.getCreateLabel(),
        removeLabel: Epsitec.Texts.getDeleteLabel()
      };
      Ext.applyIf(newOptions, options);

      this.callParent([newOptions]);
      return this;
    },

    /* Methods */

    // Overrides the method defined in EditableEntityList.
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
      // If we have a creation view, we use this view to create the new entity,
      // otherwise we use the regular creation process.

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
          this.creationViewId, this.entityTypeId, null, callback
      );
    },

    createEntityWithViewCallback: function(entityId) {
        this.getSelectionModel().deselectAll();
        this.selectEntity(entityId, false);
        Epsitec.Cresus.Core.app.reloadCurrentDatabase(false);
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


      json = Epsitec.Tools.processResponse(success, response);
      if (json === null) {
        return;
      }

      entityId = json.content.id;
      this.selectEntity(entityId, true);
    },

    // Overrides the method defined in EditableEntityList.
    handleRemove: function(entityItems) {
      this.deleteEntities(entityItems);
    },

    deleteEntities: function(entityItems) {
      // If we have a deletion view, we use this view to delete the entity,
      // otherwise we use the regular deletion process.

      if (this.deletionViewId === null) {
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
      var callback, viewMode;

      callback = Epsitec.Callback.create(
          this.deleteEntityWithViewCallback, this);

      viewMode = Epsitec.ViewMode.brickDeletion;

      Epsitec.EntityAction.showDialog(
          viewMode, this.deletionViewId, entityItem.id, null, callback
      );
    },

    deleteEntityWithViewCallback: function() {
      this.resetStore(true);
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

      this.resetStore(true);
    },

    reload: function(columnManager) {
      columnManager.removeAllColumns();
      this.setLoading(false,true);
      this.resetStore(false);
      this.store.load();
    },

    reloadAndScrollToEntity: function(columnManager,entityId,entityIndex,samePage) {
      var result, scroll;


      this.resetStore(false);
      this.setLoading(false,true);
      columnManager.removeAllColumns();


      result = this.store.data.findBy(function(record) {
          return record.getId() === entityId;
      });

      if (!result && !samePage) {
        Ext.Ajax.request({
          url: this.buildGetIndexUrl(entityId),
          method: 'GET',
          callback: function(options, success, response) {
            this.selectEntityCallback(success, response, entityId, false);
          },
          scope: this
        });

        return;
      }

      scroll = entityIndex;

      this.store.reload({
        callback: function() {
          this.view.bufferedRenderer.scrollTo(
              scroll,
              false,
              function() {
                this.setLoading(false);
                this.getSelectionModel().select(entityIndex);
              },
              this
          );
        },
        scope: this
      });

    },

    selectEntity: function(entityId, suppressEvent) {
      this.resetStore(false);
      this.setLoading(false);

      // The first step of the entity selection is to get its index within the
      // list. The selectEntityCallback method will be called once the server
      // gives us this index.

      Ext.Ajax.request({
        url: this.buildGetIndexUrl(entityId),
        method: 'GET',
        callback: function(options, success, response) {
          this.selectEntityCallback(success, response, entityId, suppressEvent);
        },
        scope: this
      });
    },

    buildGetIndexUrl: function(entityId) {
      var base = 'proxy/database/getindex' +
          '/' + this.databaseName +
          '/' + entityId;

      return this.buildUrlWithSortersAndFilters(base);
    },

    selectEntityCallback: function(success, response, entityId, suppressEvent) {
      var json, index;

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
        this.resetStore(true);
        return;
      }

      // Now that we have the index of the entity, we load the store and scroll
      // to the given index. The scrollTo method will handle the loading of the
      // data that is not yet loaded, if it is outside of the range of entities
      // that are currently loaded by the store. Once the view will show the
      // range of entities around the index, the selectEntityCallback2 will be
      // called and we'll be able to proceed.

      this.store.load({
        callback: function() {
          this.view.bufferedRenderer.scrollTo(
              index,
              false,
              function() {
                this.selectEntityCallback2(entityId, suppressEvent);
              },
              this
          );
        },
        scope: this
      });
    },

    selectEntityCallback2: function(entityId, suppressEvent) {
      // Now we try to select the entity that we want to.

      // We don't look for the record by its index but by its id. This is
      // because the index might have changed if another user has added or
      // removed entities. We hope that our record has not been shifted too far
      // away from the index that we got. If that's the case, we'll still be
      // able to find it in the data that has been loaded by the call to the
      // scrollTo method.

      this.setLoading(false);

      var result = this.store.data.findBy(function(record) {
            return record.getId() === entityId;
        });

      if (!result) {
          this.selectEntity(entityId, suppressEvent);
          return;
      }

      // At last, we can select the entity record.
      this.getSelectionModel().select(result, false, suppressEvent);
    }

  });
});
