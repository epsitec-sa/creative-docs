Ext.require([
  'Epsitec.cresus.webcore.tile.CollectionSummaryTile',
  'Epsitec.cresus.webcore.tools.Texts'
],
function() {
  Ext.define('Epsitec.cresus.webcore.tile.EmptySummaryTile', {
    extend: 'Epsitec.cresus.webcore.tile.CollectionSummaryTile',
    alternateClassName: ['Epsitec.EmptySummaryTile'],
    alias: 'widget.epsitec.emptysummarytile',

    /* Config */

    html: Epsitec.Texts.getEmptySummaryText(),

    /* Constructor */

    constructor: function(options) {
      var newOptions = {
        hideRemoveButton: true
      };
      Ext.applyIf(newOptions, options);

      this.callParent([newOptions]);
      return this;
    },

    /* Additional methods */

    bodyClickHandler: function() {
      this.addEntityHandler();
    }
  });
});
