Ext.require([
  'Epsitec.cresus.webcore.SummaryTile',
  'Epsitec.cresus.webcore.Texts',
  'Epsitec.cresus.webcore.Tools'
],
function() {
  Ext.define('Epsitec.cresus.webcore.CollectionSummaryTile', {
    extend: 'Epsitec.cresus.webcore.SummaryTile',
    alternateClassName: ['Epsitec.CollectionSummaryTile'],
    alias: 'widget.epsitec.collectionsummarytile',

    /* Properties */

    propertyAccessorId: null,

    /* Constructor */

    constructor: function(options) {
      var newOptions = {
        tools: this.createCollectionTools(options)
      };
      Ext.applyIf(newOptions, options);

      this.callParent([newOptions]);
      return this;
    },

    /* Additional methods */

    createCollectionTools: function(options) {
      var tools = Ext.Array.clone(options.tools || []);

      if (!options.hideAddButton) {
        tools.push({
          type: 'plus',
          tooltip: Epsitec.Texts.getAddTip(),
          handler: this.addEntityHandler,
          scope: this
        });
      }

      if (!options.hideRemoveButton) {
        tools.push({
          type: 'minus',
          tooltip: Epsitec.Texts.getRemoveTip(),
          handler: this.deleteEntityHandler,
          scope: this
        });
      }

      return tools;
    },

    addEntityHandler: function() {
      this.setLoading();
      Ext.Ajax.request({
        url: 'proxy/collection/create',
        method: 'POST',
        params: {
          parentEntityId: this.column.entityId,
          propertyAccessorId: this.propertyAccessorId
        },
        callback: this.addEntityCallback,
        scope: this
      });
    },

    addEntityCallback: function(options, success, response) {
      var json, entityId;

      this.setLoading(false);

      json = Epsitec.Tools.processResponse(success, response);
      if (json === null) {
        return;
      }

      entityId = json.content.key;
      this.handleEntityCreated(entityId);
      this.addEntityColumn(entityId, true);
    },

    handleEntityCreated: function(entityId) {
      // This method exists only to be derived in subclasses.
    },

    deleteEntityHandler: function() {
      this.setLoading();
      Ext.Ajax.request({
        url: 'proxy/collection/delete',
        method: 'POST',
        params: {
          parentEntityId: this.column.entityId,
          deletedEntityId: this.entityId,
          propertyAccessorId: this.propertyAccessorId
        },
        callback: this.deleteEntityCallback,
        scope: this
      });
    },

    deleteEntityCallback: function(options, success, response) {
      var json;

      this.setLoading(false);

      json = Epsitec.Tools.processResponse(success, response);
      if (json === null) {
        return;
      }

      if (this.isSelected()) {
        this.column.removeToRight();
      }
      this.column.refreshToLeft();
    },

    getState: function() {
      return {
        type: 'collectionSummaryTile',
        entityId: this.entityId,
        propertyAccessorId: this.propertyAccessorId
      };
    },

    setState: function(state) {
      this.select(true);
    },

    isStateApplicable: function(state) {
      return state.type === 'collectionSummaryTile' &&
          state.entityId === this.entityId &&
          state.propertyAccessorId === this.propertyAccessorId;
    }
  });
});
