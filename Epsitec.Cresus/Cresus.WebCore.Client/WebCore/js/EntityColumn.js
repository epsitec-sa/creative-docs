Ext.require([
  'Epsitec.cresus.webcore.CallbackQueue',
  'Epsitec.cresus.webcore.CollectionSummaryTile',
  'Epsitec.cresus.webcore.EditionTile',
  'Epsitec.cresus.webcore.EmptySummaryTile',
  'Epsitec.cresus.webcore.SummaryTile',
  'Epsitec.cresus.webcore.Tile'
],
function() {
  Ext.define('Epsitec.cresus.webcore.EntityColumn', {
    extend: 'Ext.Panel',
    alternateClassName: ['Epsitec.EntityColumn'],

    /* Config */

    border: false,
    style: {
      borderTop: '1px solid #99BCE8'
    },
    margin: '0 0 0 1',

    /* Properties */

    columnId: null,
    columnManager: null,
    entityId: null,
    viewMode: '1',
    viewId: 'null',
    selectedTile: null,
    selectedEntityId: null,

    /* Constructor */

    constructor: function(options) {
      var newOptions = Ext.clone(options);

      Ext.Array.forEach(newOptions.items,
          function(tile) {
            tile.column = this;
          },
          this
      );

      this.callParent([newOptions]);
      return this;
    },

    /* Additional methods */

    getState: function() {
      return {
        selectedEntityId: this.selectedEntityId
      };
    },

    setState: function(state) {
      if (state.selectedEntityId !== null) {
        this.selectTileWithEntityId(state.selectedEntityId);
      }
    },

    selectTileWithEntityId: function(entityId) {
      var tileToSelect = this.getTileWithEntityId(entityId);
      this.selectTile(tileToSelect);
      if (tileToSelect === null) {
        this.selectedEntityId = entityId;
      }
    },

    getTileWithEntityId: function(entityId) {
      var tiles, tile, i;
      tiles = this.items.items;
      for (i = 0; i < tiles.length; i += 1) {
        tile = tiles[i];
        if (tile.entityId === entityId) {
          return tile;
        }
      }
      return null;
    },

    selectTile: function(tile) {
      if (this.selectedTile !== tile) {
        if (this.selectedTile !== null) {
          this.selectedTile.select(false);
        }
        this.selectedTile = tile;
        if (this.selectedTile !== null) {
          this.selectedTile.select(true);
          this.selectedEntityId = this.selectedTile.entityId;
        }
      }
    },

    refresh: function() {
      this.columnManager.refreshColumn(this);
    },

    refreshToLeft: function() {
      this.columnManager.refreshColumnsToLeft(this);
    },

    refreshAll: function() {
      this.columnManager.refreshAllColumns();
    },

    addEntityColumn: function(viewMode, viewId, entityId, refreshToLeft) {
      var callbackQueue = null;
      if (refreshToLeft) {
        callbackQueue = Epsitec.CallbackQueue.create(
            function() { this.refreshToLeft(); },
            this
            );
      }
      this.selectTileWithEntityId(entityId);
      this.columnManager.addEntityColumn(
          viewMode, viewId, entityId, this, callbackQueue
      );
    },

    removeToRight: function() {
      this.columnManager.removeRightColumns(this);
    },

    showAction: function(viewId, entityId, callback) {
      this.columnManager.showAction(viewId, entityId, callback);
    }
  });
});
