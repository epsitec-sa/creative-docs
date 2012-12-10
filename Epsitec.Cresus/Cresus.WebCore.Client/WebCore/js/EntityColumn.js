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
      if (this.selectedTile === null) {
        return {
          tileIndex: null,
          tileState: null
        };
      }
      else {
        return {
          tileIndex: this.items.items.indexOf(this.selectedTile),
          tileState: this.selectedTile.getState()
        };
      }
    },

    setState: function(state) {
      var tileIndex, tileState;

      tileIndex = state.tileIndex;
      tileState = state.tileState;

      if (tileIndex === null || tileState === null) {
        return;
      }
      this.selectedTile = this.getTileForState(tileIndex, tileState);
      if (this.selectedTile !== null) {
        this.selectedTile.setState(tileState);
      }
    },

    getTileForState: function(tileIndex, tileState) {

      // Looks for a tile for which the state is applicable. Let i be the index
      // of the selected tile in the previous entity column, then the tiles are
      // examined in the following order: i, i-1, i+1, i-2, i+2, ... until it
      // finds a tile for which the state is applicable or all tiles have been
      // examined.

      var up, down, tiles, tile;

      up = tileIndex;
      down = tileIndex - 1;
      tiles = this.items.items;

      while (down >= 0 || up < tiles.length) {
        if (up < tiles.length) {
          tile = tiles[up];
          if (tile.isStateApplicable(tileState)) {
            return tile;
          }
        }
        if (down >= 0) {
          tile = tiles[down];
          if (tile.isStateApplicable(tileState)) {
            return tile;
          }
        }
        up += 1;
        down -= 1;
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
