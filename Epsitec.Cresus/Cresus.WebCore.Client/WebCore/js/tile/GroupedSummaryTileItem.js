Ext.require([
  'Epsitec.cresus.webcore.tools.Tools'
],
function() {
  Ext.define('Epsitec.cresus.webcore.tile.GroupedSummaryTileItem', {
    extend: 'Ext.Panel',
    alternateClassName: ['Epsitec.GroupedSummaryTileItem'],
    alias: 'widget.epsitec.groupedsummarytileitem',

    /* Config */

    border: false,
    bodyCls: 'tile',
    overCls: 'tile-over',
    selectedClass: 'tile-selected',

    /* Properties */

    entityId: null,
    groupedSummaryTile: null,
    selected: false,

    /* Constructor */

    constructor: function(options) {
      var newOptions = { };
      Ext.applyIf(newOptions, options);

      if (!newOptions.isLast) {
        newOptions.style = newOptions.style || {};
        newOptions.style.borderBottom = '1px solid #99BCE8';
      }

      this.callParent([newOptions]);
      return this;
    },

    /* Listeners */

    listeners: {
      render: function() {
        this.body.on('click', this.handleClick, this);
      }
    },

    handleClick: function() {
      this.groupedSummaryTile.handleItemClick(this);
    },

    /* Additional methods */

    select: function(selected) {
      if (this.selected !== selected) {
        this.selected = selected;
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
