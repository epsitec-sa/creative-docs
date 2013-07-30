// This class represents an entity list that can be edited and that is backed by
// a database on the server.

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
    toProcess: null,

    /* Constructor */

    constructor: function(options) {
      var newOptions = {
        getUrl: Epsitec.SetEntityList.getUrl(
            options.viewId, options.entityId, 'get/display'
        ),
        exportUrl: Epsitec.SetEntityList.getUrl(
            options.viewId, options.entityId, 'export/display'
        ),
        addLabel: Epsitec.Texts.getAddLabel(),
        removeLabel: Epsitec.Texts.getRemoveLabel(),
        onSelectionChange: Epsitec.Callback.create(
            this.onSelectionChange,
            this
        )
      };
      Ext.applyIf(newOptions, options);

      this.callParent([newOptions]);
      return this;
    },

    /* Methods */

    onSelectionChange: function() {
      // TODO: add the right column
    },

    // Overrides the method defined in EditableEntityList.
    handleAdd: function() {
      var callback = Epsitec.Callback.create(this.handleAddCallback, this);
      Epsitec.EntityListPicker.showSet(
          this.viewId, this.entityId, this.pickDatabaseDefinition, callback
      );
    },

    handleAddCallback: function(entityItems) {
      this.processEntities(entityItems, 'add');
    },

    // Overrides the method defined in EditableEntityList.
    handleRemove: function(entityItems) {
      var list = this;
      this.toProcess = entityItems;
      Ext.MessageBox.confirm(
          Epsitec.Texts.getWarningTitle(),
          Epsitec.Texts.getEntityRemoveWarningMessage(),
          this.handleRemoveCallback,
          list
      );
    },

    handleRemoveCallback: function(buttonId) {
      if (buttonId === 'yes') {
        this.processEntities(this.toProcess, 'remove');
      }
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

      this.resetStore(true);
      this.setColumn.refreshToLeft(false);
    }
  });
});
