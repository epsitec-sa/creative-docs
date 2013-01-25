Ext.require([
  'Epsitec.cresus.webcore.tile.CollectionSummaryTile',
  'Epsitec.cresus.webcore.tile.EditionTile',
  'Epsitec.cresus.webcore.tile.EmptySummaryTile',
  'Epsitec.cresus.webcore.tile.GroupedSummaryTile',
  'Epsitec.cresus.webcore.tile.SummaryTile',
  'Epsitec.cresus.webcore.tile.Tile'
],
function() {
  Ext.define('Epsitec.cresus.webcore.entityUi.TileColumn', {
    extend: 'Epsitec.cresus.webcore.entityUi.EntityColumn',
    alternateClassName: ['Epsitec.TileColumn'],

    /* Config */

    border: false,
    width: 350,
    style: {
      borderTop: '1px solid #99BCE8'
    },

    /* Properties */

    selectedTile: null,
    selectedState: null,

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
      var tileIndex, tileState;

      tileIndex = null;
      tileState = null;

      if (this.selectedTile !== null) {
        tileIndex = this.items.items.indexOf(this.selectedTile);
        tileState = this.selectedTile.getState();
      }
      else if (this.selectedState !== null) {
        tileIndex = 0;
        tileState = this.selectedState;
      }

      return {
        tileIndex: tileIndex,
        tileState: tileState
      };
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

    // This method should be called when you want to select a tile that already
    // exists.
    selectTile: function(tile) {
      this.selectStateInternal(null);
      this.selectTileInternal(tile);
    },

    // This method should be called when you want to select a tile that does not
    // exist yet, such as when you create a collection tile for a new entity and
    // want it selected when the ui is refreshed.
    selectState: function(state) {
      this.selectStateInternal(state);
      this.selectTileInternal(null);
    },

    selectTileInternal: function(tile) {
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

    selectStateInternal: function(state) {
      this.selectedState = state;
    }
  });
});
