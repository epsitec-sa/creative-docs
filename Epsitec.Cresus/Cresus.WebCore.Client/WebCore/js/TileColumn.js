Ext.require([
  'Epsitec.cresus.webcore.CollectionSummaryTile',
  'Epsitec.cresus.webcore.EditionTile',
  'Epsitec.cresus.webcore.EmptySummaryTile',
  'Epsitec.cresus.webcore.GroupedSummaryTile',
  'Epsitec.cresus.webcore.SummaryTile',
  'Epsitec.cresus.webcore.Tile'
],
function() {
  Ext.define('Epsitec.cresus.webcore.TileColumn', {
    extend: 'Epsitec.cresus.webcore.EntityColumn',
    alternateClassName: ['Epsitec.TileColumn'],

    /* Config */

    border: false,
    width: 350,
    style: {
      borderTop: '1px solid #99BCE8'
    },

    /* Properties */

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
    }
  });
});
