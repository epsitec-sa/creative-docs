// This class represents the empty summary tile that is displayed for the empty
// collections whose template has the auto grouping disabled. This is usefull so
// that the use can click on the empty summary tile to create the first item in
// the collection.

Ext.require([
  'Epsitec.cresus.webcore.tile.CollectionSummaryTile',
  'Epsitec.cresus.webcore.tools.Texts'
],
function() {
  Ext.define('Epsitec.cresus.webcore.tile.EmptySummaryTile', {
    extend: 'Epsitec.cresus.webcore.tile.CollectionSummaryTile',
    alternateClassName: ['Epsitec.EmptySummaryTile'],
    alias: 'widget.epsitec.emptysummarytile',

    /* Configuration */

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

    /* Methods */

    bodyClickHandler: function() {
      this.addEntityHandler();
    }
  });
});
