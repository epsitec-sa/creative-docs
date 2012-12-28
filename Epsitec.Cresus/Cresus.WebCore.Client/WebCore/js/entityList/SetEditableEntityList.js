Ext.require([
  'Epsitec.cresus.webcore.entityList.EditableEntityList',
  'Epsitec.cresus.webcore.entityList.SetEntityList',
  'Epsitec.cresus.webcore.tools.Callback',
  'Epsitec.cresus.webcore.tools.Tools'
],
function() {
  Ext.define('Epsitec.cresus.webcore.entityList.SetEditableEntityList', {
    extend: 'Epsitec.cresus.webcore.entityList.EditableEntityList',
    alternateClassName: ['Epsitec.SetEditableEntityList'],

    /* Properties */

    setColumn: null,
    viewId: null,
    entityId: null,
    pickDatabaseDefinition: null,

    /* Constructor */

    constructor: function(options) {
      var newOptions = {
        getUrl: Epsitec.SetEntityList.getUrl(
            options.viewId, options.entityId, 'get/display'
        ),
        addLabel: Epsitec.Texts.getAddLabel(),
        removeLabel: Epsitec.Texts.getRemoveLabel()
      };
      Ext.applyIf(newOptions, options);

      this.callParent([newOptions]);
      return this;
    },

    /* Additional methods */

    handleAdd: function() {
      var callback = Epsitec.Callback.create(this.handleAddCallback, this);
      Epsitec.EntityListPicker.showSet(
          this.viewId, this.entityId, this.pickDatabaseDefinition, callback
      );
    },

    handleAddCallback: function(entityItems) {
      this.processEntities(entityItems, 'add');
    },

    handleRemove: function(entityItems) {
      this.processEntities(entityItems, 'remove');
    },

    processEntities: function(entityItems, urlSuffix) {
      this.setLoading();
      Ext.Ajax.request({
        url: Epsitec.SetEntityList.getUrl(
            this.viewId, this.entityId, urlSuffix
        ),
        method: 'POST',
        params: {
          entityIds: entityItems.map(function(e) { return e.id; }).join(';')
        },
        callback: this.processEntitiesCallback,
        scope: this
      });
    },

    processEntitiesCallback: function(options, success, response) {
      var json;

      this.setLoading(false);

      json = Epsitec.Tools.processResponse(success, response);
      if (json === null) {
        return;
      }

      this.reloadStore();
      this.setColumn.refreshToLeft(false);
    }
  });
});
