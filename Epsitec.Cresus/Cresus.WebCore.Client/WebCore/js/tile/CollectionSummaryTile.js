// This class represents the tile of an entity which is part of a collection,
// when the template auto grouping has not been enabled. In addition to the
// regular stuff of summary tiles, these tiles also contains an add button that
// can be used to add an entity to the collection, and a remove button to remove
// the current element.

Ext.require([
  'Epsitec.cresus.webcore.tile.SummaryTile',
  'Epsitec.cresus.webcore.tools.Texts',
  'Epsitec.cresus.webcore.tools.Tools'
],
function() {
  Ext.define('Epsitec.cresus.webcore.tile.CollectionSummaryTile', {
    extend: 'Epsitec.cresus.webcore.tile.SummaryTile',
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

    /* Methods */

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
        url: 'proxy/list/create',
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
      var json, entityId, state;

      this.setLoading(false);

      json = Epsitec.Tools.processResponse(success, response);
      if (json === null) {
        return;
      }

      entityId = json.content.key;
      state = this.getTileState(entityId, this.propertyAccessorId);

      this.column.selectState(state);
      this.column.addEntityColumn(
          this.subViewMode, this.subViewId, entityId, true
      );
    },

    deleteEntityHandler: function() {
      this.setLoading();
      Ext.Ajax.request({
        url: 'proxy/list/delete',
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
      this.column.refreshToLeft(true);
    },

    // Overrides the method defined in EntityTile.
    showAction: function(viewId, callback) {
      this.column.showTemplateAction(viewId, this.entityId, callback);
    },

    // Overrides the method defined in Tile and SummaryTile.
    getState: function() {
      return this.getTileState(this.entityId, this.propertyAccessorId);
    },

    getTileState: function(entityId, propertyAccessorId) {
      return {
        type: 'collectionSummaryTile',
        entityId: entityId,
        propertyAccessorId: propertyAccessorId
      };
    },

    // Overrides the method defined in Tile and SummaryTile.
    setState: function(state) {
      this.select(true);
    },

    // Overrides the method defined in Tile and SummaryTile.
    isStateApplicable: function(state) {
      return state.type === 'collectionSummaryTile' &&
          state.entityId === this.entityId &&
          state.propertyAccessorId === this.propertyAccessorId;
    }
  });
});
