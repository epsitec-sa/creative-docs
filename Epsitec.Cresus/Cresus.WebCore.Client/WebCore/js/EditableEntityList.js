Ext.require([
  'Epsitec.cresus.webcore.EntityList',
  'Epsitec.cresus.webcore.Texts',
  'Epsitec.cresus.webcore.Tools'
],
function() {
  Ext.define('Epsitec.cresus.webcore.EditableEntityList', {
    extend: 'Epsitec.cresus.webcore.EntityList',
    alternateClassName: ['Epsitec.EditableEntityList'],

    /* Constructor */

    constructor: function(options) {
      var newOptions = {
        toolbarButtons: this.createEditionButtons()
      };
      Ext.applyIf(newOptions, options);

      this.callParent([newOptions]);
      return this;
    },

    /* Additional methods */

    createEditionButtons: function() {
      var buttonCreate, buttonDelete;

      buttonCreate = Ext.create('Ext.Button', {
        text: Epsitec.Texts.getCreateLabel(),
        iconCls: 'icon-add',
        listeners: {
          click: this.onCreateHandler,
          scope: this
        }
      });

      buttonDelete = Ext.create('Ext.Button', {
        text: Epsitec.Texts.getDeleteLabel(),
        iconCls: 'icon-remove',
        listeners: {
          click: this.onDeleteHandler,
          scope: this
        }
      });

      return [buttonCreate, buttonDelete];
    },

    onCreateHandler: function() {
      if (this.filters.getFilterData().length > 0) {
        Ext.MessageBox.confirm(
            Epsitec.Texts.getWarningTitle(),
            Epsitec.Texts.getEntityCreationWarningMessage(),
            this.onCreateHandlerCallback,
            this
        );
      }
      else {
        this.createEntity();
      }
    },

    onCreateHandlerCallback: function(buttonId) {
      if (buttonId === 'yes') {
        this.createEntity();
      }
    },

    onDeleteHandler: function() {
      var entityItems;

      entityItems = this.getSelectedItems();
      if (entityItems.length === 0) {
        return;
      }

      this.deleteEntities(entityItems);
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
      var json, entityId;

      this.setLoading(false);

      json = Epsitec.Tools.processResponse(success, response);
      if (json === null) {
        return;
      }

      entityId = json.content.id;
      this.selectEntity(entityId);
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
});
