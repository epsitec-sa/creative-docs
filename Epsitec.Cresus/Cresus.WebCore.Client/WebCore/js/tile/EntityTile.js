// This class is the abstract class that represents tiles that are bound to a
// single entity.

Ext.require([
  'Epsitec.cresus.webcore.tile.Tile',
  'Epsitec.cresus.webcore.tools.Tools'
],
function() {
  Ext.define('Epsitec.cresus.webcore.tile.EntityTile', {
    extend: 'Epsitec.cresus.webcore.tile.Tile',
    alternateClassName: ['Epsitec.EntityTile'],

    /* Properties */

    entityId: null,
    selectedClass: 'tile-selected',

    /* Methods */

    // Overrides the method defined in Tile.
    showAction: function(viewId, callback) {
      this.column.showAction(viewId, this.entityId, callback);
    },

    showTemplateAction: function(viewId, aEntityId,callback) {
      this.column.showTemplateAction(viewId, aEntityId, callback);
    },

    select: function(selected) {
      if (this.isSelected() !== selected) {
        this.setSelected(selected);
        if (selected) {
          this.addCls(this.selectedClass);
        }
        else {
          this.removeCls(this.selectedClass);
        }
      }
    }
  });
});
